# ControleGlicemia — Backend

API para controle e monitoramento da glicemia, desenvolvida para auxiliar pessoas com diabetes a registrar medições, refeições, medicamentos e acompanhar sua evolução por meio de relatórios clínicos em PDF.

## Sobre o projeto

Gerenciar a glicemia exige disciplina e registro constante. Esta API oferece uma camada segura e estruturada para que aplicações (web ou mobile) possam armazenar e consultar dados de saúde de forma confiável, com autenticação JWT, isolamento por usuário e geração de relatórios prontos para impressão.

### Funcionalidades principais

- **Autenticação segura** — Cadastro e login com BCrypt, JWT + refresh token com rotação e blacklist de tokens
- **Registro de medições** — Valores de glicose com classificação por momento do dia (7 categorias)
- **Diário pessoal** — Anotações diárias associadas aos registros
- **Refeições e medicamentos** — Controle individualizado com validações de negócio
- **Relatório PDF** — Geração de relatório clínico por período com gráficos e grade glicêmica
- **Expurgo automático** — Limpeza programada de registros com soft delete

### Diferenciais técnicos

- Rate limiting por endpoint (5 req/min em autenticação, 100 req/min no geral)
- Migrations automáticas na inicialização
- Health checks com verificação do banco
- Soft delete com query filters globais
- Testes unitários com cobertura abrangente (159 testes)

## Stack

| Camada | Tecnologia |
|---|---|
| Framework | .NET 8 (ASP.NET Core) |
| ORM | Entity Framework Core 8 |
| Banco | MySQL 8 / SQLite (dev) |
| Autenticação | JWT Bearer + BCrypt |
| PDF | QuestPDF |
| Testes | xUnit + Moq |

## Quick start

```bash
dotnet restore
dotnet run --project ControleGlicemia.API
```

A API sobe em `http://localhost:5223` com SQLite e migrations automáticas.

## Testes

```bash
dotnet test
```

---

**Documentação completa dos endpoints em:** [`ControleGlicemia.API/README.md`](ControleGlicemia.API/README.md)
