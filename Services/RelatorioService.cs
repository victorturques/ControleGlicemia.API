using System.ComponentModel.DataAnnotations;
using System.Globalization;
using ControleGlicemia.API.Data;
using ControleGlicemia.API.DTOs.Relatorio;
using ControleGlicemia.API.Models;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ControleGlicemia.API.Services;

public class RelatorioService : IRelatorioService
{
    private readonly AppDbContext _context;

    public RelatorioService(AppDbContext context)
    {
        _context = context;
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

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new KeyNotFoundException("Usuário não encontrado.");

        var registrosGlicose = await _context.RegistrosGlicose
            .AsNoTracking()
            .Where(r => r.UserId == userId && r.MedidoEm.Date >= dataInicio && r.MedidoEm.Date <= dataFim)
            .OrderBy(r => r.MedidoEm)
            .ToListAsync();

        var medicamentos = await _context.Medicamentos
            .AsNoTracking()
            .Where(m => m.UserId == userId)
            .OrderBy(m => m.TomadoEm)
            .ToListAsync();

        var registrosDiarios = await _context.RegistrosDiarios
            .AsNoTracking()
            .Where(r => r.UserId == userId && r.Data.Date >= dataInicio && r.Data.Date <= dataFim)
            .OrderBy(r => r.Data)
            .ToListAsync();

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

                    col.Item().PaddingTop(8)
                        .Text(mes.ToString("MMMM yyyy", new CultureInfo("pt-BR")).ToUpper())
                        .FontSize(12)
                        .Bold();

                    col.Item().PaddingTop(3).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(22); // DIA
                            columns.RelativeColumn();   // PRÉ CAFÉ
                            columns.RelativeColumn();   // PÓS CAFÉ
                            columns.RelativeColumn();   // PRÉ ALMOÇO
                            columns.RelativeColumn();   // PÓS ALMOÇO
                            columns.RelativeColumn();   // PRÉ JANTAR
                            columns.RelativeColumn();   // PÓS JANTAR
                            columns.RelativeColumn();   // ANTES DE DORMIR
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

                if (registros.Any())
                {
                    col.Item().PaddingTop(8)
                        .Text($"Média: {media:F1} mg/dL  |  Maior: {maior:F1} mg/dL  |  Menor: {menor:F1} mg/dL")
                        .Bold().FontSize(12);
                }

                col.Item().PaddingTop(10).Text("Medicamentos em uso").FontSize(12).Bold();

                if (!medicamentos.Any())
                {
                    col.Item().PaddingTop(4).Text("Nenhum medicamento cadastrado.").FontSize(12);
                }
                else
                {
                    col.Item().PaddingTop(4).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background("EEEEEE").Padding(3).Text("Nome").Bold().FontSize(9);
                            header.Cell().Background("EEEEEE").Padding(3).Text("Dose").Bold().FontSize(9);
                            header.Cell().Background("EEEEEE").Padding(3).Text("Data de uso").Bold().FontSize(9);
                        });

                        foreach (var m in medicamentos)
                        {
                            table.Cell().Padding(3).Text(m.Nome).FontSize(9);
                            table.Cell().Padding(3).Text($"{m.Dose} mg").FontSize(9);
                            table.Cell().Padding(3).Text(m.TomadoEm.ToString("dd/MM/yyyy HH:mm")).FontSize(9);
                        }
                    });
                }

                var observacoesDiarias = registrosDiarios
                    .Where(r => !string.IsNullOrWhiteSpace(r.Observacoes))
                    .ToList();

                var observacoesGlicose = registros
                    .Where(r => !string.IsNullOrWhiteSpace(r.Observacoes))
                    .OrderBy(r => r.MedidoEm)
                    .ToList();

                if (observacoesDiarias.Any() || observacoesGlicose.Any())
                {
                    col.Item().PaddingTop(10).Text("Observações do período").FontSize(12).Bold();

                    col.Item().PaddingTop(4).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(90);
                            columns.ConstantColumn(95);
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background("EEEEEE").Padding(3).Text("Data").Bold().FontSize(9);
                            header.Cell().Background("EEEEEE").Padding(3).Text("Tipo").Bold().FontSize(9);
                            header.Cell().Background("EEEEEE").Padding(3).Text("Observação").Bold().FontSize(9);
                        });

                        var linhaIndex = 0;

                        foreach (var rd in observacoesDiarias)
                        {
                            var rowBg = linhaIndex % 2 == 0 ? "FFFFFF" : "F5F5F5";
                            table.Cell().Background(rowBg).Padding(3).Text(rd.Data.ToString("dd/MM/yyyy")).FontSize(9);
                            table.Cell().Background(rowBg).Padding(3).Text("Registro diário").FontSize(9);
                            table.Cell().Background(rowBg).Padding(3).Text(rd.Observacoes ?? string.Empty).FontSize(9);
                            linhaIndex++;
                        }

                        foreach (var rg in observacoesGlicose)
                        {
                            var momento = FormatarMomento(rg.MomentoMedicao);
                            var rowBg = linhaIndex % 2 == 0 ? "FFFFFF" : "F5F5F5";

                            table.Cell().Background(rowBg).Padding(3).Text(rg.MedidoEm.ToString("dd/MM/yyyy")).FontSize(9);
                            table.Cell().Background(rowBg).Padding(3).Text(momento).FontSize(9);
                            table.Cell().Background(rowBg).Padding(3).Text(rg.Observacoes ?? string.Empty).FontSize(9);
                            linhaIndex++;
                        }
                    });
                }
            });
        };
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
            .Text($"{registro.Valor:F0}").FontSize(8).Bold();
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