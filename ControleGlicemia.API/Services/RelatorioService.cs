using System.ComponentModel.DataAnnotations;
using System.Globalization;
using ControleGlicemia.API.DTOs.Relatorio;
using ControleGlicemia.API.Models;
using ControleGlicemia.API.Repositories;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace ControleGlicemia.API.Services;

public class RelatorioService : IRelatorioService
{
    private readonly IRelatorioRepository _relatorioRepository;
    private readonly ILogger<RelatorioService> _logger;

    public RelatorioService(IRelatorioRepository relatorioRepository, ILogger<RelatorioService> logger)
    {
        _relatorioRepository = relatorioRepository;
        _logger = logger;
    }

    public async Task<byte[]> GerarRelatorioPdfAsync(int userId, RelatorioRequestDto request)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        if (request.DataInicio == default || request.DataFim == default)
            throw new ValidationException("Data de início e data de fim são obrigatórias.");

        var dataInicio = request.DataInicio.Date;
        var dataFim = request.DataFim.Date;

        if (dataFim < dataInicio)
            throw new ValidationException("A data final não pode ser menor que a data inicial.");

        var user = await _relatorioRepository.GetUserByIdAsync(userId)
            ?? throw new KeyNotFoundException("Usuário não encontrado.");

        var registrosGlicose = await _relatorioRepository.GetRegistrosGlicoseByPeriodAsync(userId, dataInicio, dataFim);

        var medicamentos = await _relatorioRepository.GetMedicamentosByPeriodAsync(userId, dataInicio, dataFim);

        var registrosDiarios = await _relatorioRepository.GetRegistrosDiariosByPeriodAsync(userId, dataInicio, dataFim);

        var refeicoes = (await _relatorioRepository.GetRefeicoesByPeriodAsync(userId, dataInicio, dataFim)) ?? [];

        var media = registrosGlicose.Any() ? registrosGlicose.Average(r => r.Valor) : 0;
        var maior = registrosGlicose.Any() ? registrosGlicose.Max(r => r.Valor) : 0;
        var menor = registrosGlicose.Any() ? registrosGlicose.Min(r => r.Valor) : 0;

        var meses = Enumerable.Range(0, (dataFim.Year - dataInicio.Year) * 12 + dataFim.Month - dataInicio.Month + 1)
            .Select(i => new DateTime(dataInicio.Year, dataInicio.Month, 1).AddMonths(i))
            .ToList();

        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.0f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Element(ComposeHeader(user, request, dataInicio, dataFim));
                page.Content().Element(ComposeContent(
                    user,
                    registrosGlicose,
                    medicamentos,
                    refeicoes,
                    registrosDiarios,
                    meses,
                    dataInicio,
                    dataFim,
                    media,
                    maior,
                    menor));

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Página ");
                    x.CurrentPageNumber();
                    x.Span(" de ");
                    x.TotalPages();
                });
            });
        });

        return pdf.GeneratePdf();
    }

    private Action<IContainer> ComposeHeader(User user, RelatorioRequestDto request, DateTime dataInicio, DateTime dataFim)
    {
        return container =>
        {
            container.Column(col =>
            {
                col.Item().Text("Relatório de Controle Glicêmico")
                    .FontSize(18).Bold().AlignCenter();

                col.Item().PaddingTop(5).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text($"Paciente: {user.Nome}").FontSize(12);
                        c.Item().Text($"Médico: {request.NomeMedico ?? "Não informado"}").FontSize(12);
                    });

                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text($"Período: {dataInicio:dd/MM/yyyy} a {dataFim:dd/MM/yyyy}").FontSize(12);
                        c.Item().Text($"Meta glicêmica: {user.GlicemiaMinima} - {user.GlicemiaMaxima} mg/dL").FontSize(12);
                    });
                });

                col.Item().PaddingTop(5).Table(t =>
                {
                    t.ColumnsDefinition(c =>
                    {
                        c.ConstantColumn(58);
                        c.RelativeColumn();
                        c.RelativeColumn();
                        c.RelativeColumn();
                    });

                    t.Cell().AlignMiddle().Text("Legenda:").Bold().FontSize(12);
                    t.Cell().Background("C8E6C9").Padding(2).AlignCenter().Text("Dentro do alvo").FontSize(12);
                    t.Cell().Background("FFCDD2").Padding(2).AlignCenter().Text("Abaixo do mínimo").FontSize(12);
                    t.Cell().Background("FFE0B2").Padding(2).AlignCenter().Text("Acima da meta").FontSize(12);
                });

                col.Item().PaddingTop(4).LineHorizontal(1);
            });
        };
    }

    private Action<IContainer> ComposeContent(
        User user,
        List<RegistroGlicose> registros,
        List<Medicamento> medicamentos,
        List<Refeicao> refeicoes,
        List<RegistroDiario> registrosDiarios,
        List<DateTime> meses,
        DateTime dataInicio,
        DateTime dataFim,
        double media,
        double maior,
        double menor)
    {
        return container =>
        {
            container.Column(col =>
            {
                foreach (var mes in meses)
                {
                    var primeiroDia = (mes.Month == dataInicio.Month && mes.Year == dataInicio.Year)
                        ? dataInicio
                        : new DateTime(mes.Year, mes.Month, 1);

                    var ultimoDia = (mes.Month == dataFim.Month && mes.Year == dataFim.Year)
                        ? dataFim
                        : new DateTime(mes.Year, mes.Month, DateTime.DaysInMonth(mes.Year, mes.Month));

                    var registrosMes = registros
                        .Where(r => r.MedidoEm.Date >= primeiroDia && r.MedidoEm.Date <= ultimoDia)
                        .ToList();

                    ComposeResumoMensal(col, user, registrosMes, mes);
                    ComposeGradeGlicose(col, registrosMes, user, primeiroDia, ultimoDia);
                }

                if (registros.Any())
                {
                    col.Item().PaddingTop(8)
                        .Text($"Média: {media:F0} mg/dL  |  Maior: {maior:F0} mg/dL  |  Menor: {menor:F0} mg/dL")
                        .Bold().FontSize(12);
                }

                ComposeDetalhesDiarios(col, registros, medicamentos, refeicoes, registrosDiarios, user);
            });
        };
    }

    private void ComposeDetalhesDiarios(
        ColumnDescriptor col,
        List<RegistroGlicose> registros,
        List<Medicamento> medicamentos,
        List<Refeicao> refeicoes,
        List<RegistroDiario> registrosDiarios,
        User user)
    {
        var diasComDados = registros
            .Select(r => r.MedidoEm.Date)
            .Concat(medicamentos.Select(m => m.TomadoEm.Date))
            .Concat(refeicoes.Select(r => r.DataHora.Date))
            .Concat(registrosDiarios.Select(rd => rd.Data.Date))
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        if (!diasComDados.Any())
            return;

        col.Item().PaddingTop(8).Text("Detalhes por dia").FontSize(14).Bold();

        col.Item().PaddingTop(4).Table(table =>
        {
            table.ColumnsDefinition(c =>
            {
                c.RelativeColumn();
                c.RelativeColumn();
                c.RelativeColumn();
            });

            var cultura = new CultureInfo("pt-BR");

            foreach (var dia in diasComDados)
            {
                var glicemiasDia = registros
                    .Where(r => r.MedidoEm.Date == dia)
                    .OrderBy(r => r.MomentoMedicao)
                    .ToList();

                var medsDia = medicamentos
                    .Where(m => m.TomadoEm.Date == dia)
                    .ToList();

                var refsDia = refeicoes
                    .Where(r => r.DataHora.Date == dia)
                    .OrderBy(r => r.DataHora)
                    .ToList();

                var rd = registrosDiarios.FirstOrDefault(r => r.Data.Date == dia);

                table.Cell()
                    .Border(0.5f).BorderColor("DDDDDD")
                    .Padding(4)
                    .Column(c =>
                    {
                        c.Item().Section($"dia_{dia:yyyyMMdd}");
                        c.Item().Text(dia.ToString("dd/MM/yyyy (ddd)", cultura))
                            .FontSize(9).Bold();

                        foreach (var g in glicemiasDia)
                        {
                            var cor = g.Valor < user.GlicemiaMinima
                                ? "FFCDD2"
                                : g.Valor > user.GlicemiaMaxima
                                    ? "FFE0B2"
                                    : "C8E6C9";

                            c.Item().PaddingTop(1).Row(row =>
                            {
                                row.AutoItem().Background(cor).Width(6).Height(6).AlignMiddle();
                                row.AutoItem().PaddingLeft(3)
                                    .Text($"{FormatarMomento(g.MomentoMedicao)}: {g.Valor:F0}")
                                    .FontSize(9);
                            });

                            if (!string.IsNullOrWhiteSpace(g.Observacoes))
                            {
                                c.Item().PaddingTop(1)
                                    .Text($"→ {g.Observacoes}")
                                    .FontSize(9).Italic();
                            }
                        }

                        foreach (var m in medsDia)
                        {
                            c.Item().PaddingTop(1)
                                .Text($"Med: {m.Nome} {m.Dose:F0}mg - {m.TomadoEm:HH:mm}")
                                .FontSize(9);
                        }

                        foreach (var r in refsDia)
                        {
                            c.Item().PaddingTop(1)
                                .Text($"Ref: {r.Nome}{(r.Descricao != null ? $" - {r.Descricao}" : "")}")
                                .FontSize(9);
                        }

                        if (rd?.Observacoes != null)
                        {
                            c.Item().PaddingTop(1)
                                .Text($"Reg: {rd.Observacoes}")
                                .FontSize(9);
                        }
                    });
            }
        });
    }

    private static (int dentro, int acima, int abaixo, double tir) CalcularTir(
        List<RegistroGlicose> registros, double glicemiaMinima, double glicemiaMaxima)
    {
        if (!registros.Any())
            return (0, 0, 0, 0);

        var dentro = registros.Count(r => r.Valor >= glicemiaMinima && r.Valor <= glicemiaMaxima);
        var acima = registros.Count(r => r.Valor > glicemiaMaxima);
        var abaixo = registros.Count(r => r.Valor < glicemiaMinima);
        var total = dentro + acima + abaixo;
        var tir = total > 0 ? (double)dentro / total * 100 : 0;

        return (dentro, acima, abaixo, tir);
    }

    private static byte[] GerarDonutPng(int dentro, int acima, int abaixo, double tir)
    {
        var total = dentro + acima + abaixo;
        if (total == 0) return [];

        var size = 60;

        using var surface = SKSurface.Create(new SKImageInfo(size, size));
        var canvas = surface.Canvas;

        var rect = new SKRect(0, 0, size, size);

        using var paint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };

        float startAngle = -90;

        if (dentro > 0)
        {
            var sweep = (float)dentro / total * 360f;
            paint.Color = new SKColor(76, 175, 80);
            canvas.DrawArc(rect, startAngle, sweep, true, paint);
            startAngle += sweep;
        }

        if (acima > 0)
        {
            var sweep = (float)acima / total * 360f;
            paint.Color = new SKColor(255, 152, 0);
            canvas.DrawArc(rect, startAngle, sweep, true, paint);
            startAngle += sweep;
        }

        if (abaixo > 0)
        {
            var sweep = (float)abaixo / total * 360f;
            paint.Color = new SKColor(244, 67, 54);
            canvas.DrawArc(rect, startAngle, sweep, true, paint);
        }

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    private void ComposeResumoMensal(ColumnDescriptor col, User user, List<RegistroGlicose> registrosMes, DateTime mes)
    {
        col.Item().PaddingTop(8)
            .Text(mes.ToString("MMMM yyyy", new CultureInfo("pt-BR")).ToUpper())
            .FontSize(12).Bold();

        if (!registrosMes.Any())
            return;

        var (dentro, acima, abaixo, tir) = CalcularTir(registrosMes, user.GlicemiaMinima, user.GlicemiaMaxima);
        var donutBytes = GerarDonutPng(dentro, acima, abaixo, tir);

        var mediaMes = registrosMes.Average(r => r.Valor);
        var maiorMes = registrosMes.Max(r => r.Valor);
        var menorMes = registrosMes.Min(r => r.Valor);
        var total = dentro + acima + abaixo;

        col.Item().PaddingTop(4).Row(row =>
        {
            row.ConstantItem(60).Image(donutBytes);

            row.RelativeItem().PaddingLeft(6).AlignBottom().Column(stats =>
            {
                stats.Item().Row(legend =>
                {
                    legend.AutoItem().Element(x => x.Width(8).Height(8).Background("4CAF50"));
                    legend.AutoItem().PaddingLeft(3).Text($"Dentro: {dentro} ({tir:F0}%)").FontSize(11);
                });

                stats.Item().PaddingTop(1).Row(legend =>
                {
                    legend.AutoItem().Element(x => x.Width(8).Height(8).Background("FF9800"));
                    legend.AutoItem().PaddingLeft(3).Text($"Acima: {acima} ({(double)acima / total * 100:F0}%)").FontSize(11);
                });

                stats.Item().PaddingTop(1).Row(legend =>
                {
                    legend.AutoItem().Element(x => x.Width(8).Height(8).Background("F44336"));
                    legend.AutoItem().PaddingLeft(3).Text($"Abaixo: {abaixo} ({(double)abaixo / total * 100:F0}%)").FontSize(11);
                });

                stats.Item().PaddingTop(2).Text($"Total: {total} | Média: {mediaMes:F0} | Maior: {maiorMes:F0} | Menor: {menorMes:F0}").FontSize(11);
            });
        });
    }

    private void ComposeGradeGlicose(ColumnDescriptor col, List<RegistroGlicose> registrosMes, User user, DateTime primeiroDia, DateTime ultimoDia)
    {
        if (!registrosMes.Any())
            return;

        col.Item().PaddingTop(4).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(22);
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
            });

            table.Header(header =>
            {
                var headerBg = "37474F";
                var headerStyle = TextStyle.Default.FontSize(8).Bold().FontColor(Colors.White);

                header.Cell().Background(headerBg).Padding(2).AlignCenter().AlignMiddle().Text("DIA").Style(headerStyle);
                header.Cell().Background(headerBg).Padding(2).AlignCenter().AlignMiddle().Text("PRÉ\nCAFÉ").Style(headerStyle);
                header.Cell().Background(headerBg).Padding(2).AlignCenter().AlignMiddle().Text("PÓS\nCAFÉ").Style(headerStyle);
                header.Cell().Background(headerBg).Padding(2).AlignCenter().AlignMiddle().Text("PRÉ\nALMOÇO").Style(headerStyle);
                header.Cell().Background(headerBg).Padding(2).AlignCenter().AlignMiddle().Text("PÓS\nALMOÇO").Style(headerStyle);
                header.Cell().Background(headerBg).Padding(2).AlignCenter().AlignMiddle().Text("PRÉ\nJANTAR").Style(headerStyle);
                header.Cell().Background(headerBg).Padding(2).AlignCenter().AlignMiddle().Text("PÓS\nJANTAR").Style(headerStyle);
                header.Cell().Background(headerBg).Padding(2).AlignCenter().AlignMiddle().Text("ANTES DE\nDORMIR").Style(headerStyle);
            });

            for (var dia = primeiroDia; dia <= ultimoDia; dia = dia.AddDays(1))
            {
                var registrosDia = registrosMes.Where(r => r.MedidoEm.Date == dia.Date).ToList();
                var rowBg = dia.Day % 2 == 0 ? "F5F5F5" : "FFFFFF";

                table.Cell()
                    .Background(rowBg)
                    .BorderBottom(0.5f).BorderColor("DDDDDD")
                    .MinHeight(16)
                    .Padding(2)
                    .AlignCenter().AlignMiddle()
                    .Text(dia.Day.ToString("D2")).Bold().FontSize(8);

                RenderCelula(table, registrosDia, MomentoMedicao.PreCafe, user, rowBg);
                RenderCelula(table, registrosDia, MomentoMedicao.PosCafe, user, rowBg);
                RenderCelula(table, registrosDia, MomentoMedicao.PreAlmoco, user, rowBg);
                RenderCelula(table, registrosDia, MomentoMedicao.PosAlmoco, user, rowBg);
                RenderCelula(table, registrosDia, MomentoMedicao.PreJantar, user, rowBg);
                RenderCelula(table, registrosDia, MomentoMedicao.PosJantar, user, rowBg);
                RenderCelula(table, registrosDia, MomentoMedicao.AntesDeDormir, user, rowBg);
            }
        });
    }

    private void ComposeMedicamentos(ColumnDescriptor col, List<Medicamento> medicamentos)
    {
        col.Item().PaddingTop(6).Text("Medicamentos em uso").FontSize(10).Bold();

        if (!medicamentos.Any())
        {
            col.Item().PaddingTop(2).Text("Nenhum medicamento cadastrado.").FontSize(8);
            return;
        }

        col.Item().PaddingTop(2).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(3);
                columns.RelativeColumn(2);
                columns.RelativeColumn(2);
            });

            table.Header(header =>
            {
                header.Cell().Background("EEEEEE").Padding(1).Text("Nome").Bold().FontSize(7);
                header.Cell().Background("EEEEEE").Padding(1).Text("Dose").Bold().FontSize(7);
                header.Cell().Background("EEEEEE").Padding(1).Text("Data de uso").Bold().FontSize(7);
            });

            foreach (var m in medicamentos)
            {
                table.Cell().Padding(1).Text(m.Nome).FontSize(7);
                table.Cell().Padding(1).Text($"{m.Dose:F0} mg").FontSize(7);
                table.Cell().Padding(1).Text(m.TomadoEm.ToString("dd/MM/yyyy HH:mm")).FontSize(7);
            }
        });
    }

    private void ComposeRefeicoes(ColumnDescriptor col, List<Refeicao> refeicoes)
    {
        col.Item().PaddingTop(6).Text("Refeições").FontSize(10).Bold();

        if (!refeicoes.Any())
        {
            col.Item().PaddingTop(2).Text("Nenhuma refeição cadastrada.").FontSize(8);
            return;
        }

        col.Item().PaddingTop(2).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(65);
                columns.RelativeColumn(2);
                columns.RelativeColumn(3);
            });

            table.Header(header =>
            {
                header.Cell().Background("EEEEEE").Padding(1).Text("Data/Hora").Bold().FontSize(7);
                header.Cell().Background("EEEEEE").Padding(1).Text("Refeição").Bold().FontSize(7);
                header.Cell().Background("EEEEEE").Padding(1).Text("Descrição").Bold().FontSize(7);
            });

            foreach (var r in refeicoes)
            {
                table.Cell().Padding(1).Text(r.DataHora.ToString("dd/MM HH:mm")).FontSize(7);
                table.Cell().Padding(1).Text(r.Nome).FontSize(7);
                table.Cell().Padding(1).Text(r.Descricao ?? "-").FontSize(7);
            }
        });
    }

    private void ComposeRegistrosDiarios(ColumnDescriptor col, List<RegistroDiario> registrosDiarios)
    {
        col.Item().PaddingTop(6).Text("Registros Diários").FontSize(10).Bold();

        if (!registrosDiarios.Any())
        {
            col.Item().PaddingTop(2).Text("Nenhum registro diário cadastrado.").FontSize(8);
            return;
        }

        col.Item().PaddingTop(2).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(65);
                columns.RelativeColumn();
            });

            table.Header(header =>
            {
                header.Cell().Background("EEEEEE").Padding(1).Text("Data").Bold().FontSize(7);
                header.Cell().Background("EEEEEE").Padding(1).Text("Observações").Bold().FontSize(7);
            });

            foreach (var r in registrosDiarios)
            {
                table.Cell().Padding(1).Text(r.Data.ToString("dd/MM/yyyy")).FontSize(7);
                table.Cell().Padding(1).Text(r.Observacoes ?? "-").FontSize(7);
            }
        });
    }

    private void ComposeObservacoes(ColumnDescriptor col, List<RegistroDiario> registrosDiarios, List<RegistroGlicose> registros)
    {
        var observacoesDiarias = registrosDiarios
            .Where(r => !string.IsNullOrWhiteSpace(r.Observacoes))
            .ToList();

        var observacoesGlicose = registros
            .Where(r => !string.IsNullOrWhiteSpace(r.Observacoes))
            .OrderBy(r => r.MedidoEm)
            .ToList();

        if (!observacoesDiarias.Any() && !observacoesGlicose.Any())
            return;

        col.Item().PaddingTop(6).Text("Observações do período").FontSize(10).Bold();

        col.Item().PaddingTop(2).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(65);
                columns.RelativeColumn();
            });

            table.Header(header =>
            {
                header.Cell().Background("EEEEEE").Padding(1).Text("Data").Bold().FontSize(7);
                header.Cell().Background("EEEEEE").Padding(1).Text("Observação").Bold().FontSize(7);
            });

            var linhaIndex = 0;

            foreach (var rd in observacoesDiarias)
            {
                var rowBg = linhaIndex % 2 == 0 ? "FFFFFF" : "F5F5F5";
                table.Cell().Background(rowBg).Padding(1).Text(rd.Data.ToString("dd/MM/yyyy")).FontSize(7);
                table.Cell().Background(rowBg).Padding(1).Text($"[Registro diário] {rd.Observacoes}").FontSize(7);
                linhaIndex++;
            }

            foreach (var rg in observacoesGlicose)
            {
                var momento = FormatarMomento(rg.MomentoMedicao);
                var rowBg = linhaIndex % 2 == 0 ? "FFFFFF" : "F5F5F5";

                table.Cell().Background(rowBg).Padding(1).Text(rg.MedidoEm.ToString("dd/MM/yyyy")).FontSize(7);
                table.Cell().Background(rowBg).Padding(1).Text($"[{momento}] {rg.Observacoes}").FontSize(7);
                linhaIndex++;
            }
        });
    }

    private static void RenderCelula(
        TableDescriptor table,
        List<RegistroGlicose> registrosDia,
        MomentoMedicao momento,
        User user,
        string rowBg)
    {
        var registro = registrosDia.FirstOrDefault(r => r.MomentoMedicao == momento);

        if (registro is null)
        {
            table.Cell()
                .Background(rowBg)
                .BorderBottom(0.5f).BorderColor("DDDDDD")
                .MinHeight(16)
                .Padding(2)
                .Text("");
            return;
        }

        var cor = registro.Valor < user.GlicemiaMinima
            ? "FFCDD2"
            : registro.Valor > user.GlicemiaMaxima
                ? "FFE0B2"
                : "C8E6C9";

        table.Cell()
            .Background(cor)
            .BorderBottom(0.5f).BorderColor("DDDDDD")
            .MinHeight(16)
            .Padding(2)
            .AlignCenter().AlignMiddle()
            .SectionLink($"dia_{registro.MedidoEm:yyyyMMdd}")
            .Text($"{registro.Valor:F0}").FontSize(12).Bold().FontColor(Colors.Black);
    }

    private static string FormatarMomento(MomentoMedicao momento)
    {
        return momento switch
        {
            MomentoMedicao.PreCafe => "Pré-Café",
            MomentoMedicao.PosCafe => "Pós-Café",
            MomentoMedicao.PreAlmoco => "Pré-Almoço",
            MomentoMedicao.PosAlmoco => "Pós-Almoço",
            MomentoMedicao.PreJantar => "Pré-Jantar",
            MomentoMedicao.PosJantar => "Pós-Jantar",
            MomentoMedicao.AntesDeDormir => "Antes de dormir",
            _ => momento.ToString()
        };
    }
}