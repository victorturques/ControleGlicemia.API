# ControleGlicemia.API

![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4)
![MySQL](https://img.shields.io/badge/MySQL-8.0-4479A1)
![JWT](https://img.shields.io/badge/Auth-JWT%20Bearer-000000)
![xUnit Tests](https://img.shields.io/badge/Tests-xUnit-007ACC)

API REST para controle glicêmico com autenticação JWT, gerenciamento de registros de glicose, refeições, medicamentos, registros diários e geração de relatório em PDF.

---

## API em produção

**`[EM BREVE: link do deploy]`**

---

## Visão geral

O **ControleGlicemia.API** foi desenvolvido para ajudar no acompanhamento de saúde de pessoas com necessidade de monitoramento glicêmico, com foco em:

- Histórico de medições de glicose com classificação por momento do dia
- Registro de rotina diária
- Controle de refeições e medicamentos
- Geração de relatório clínico em PDF por período
- Isolamento completo de dados por usuário autenticado

---

## Tecnologias

| Categoria | Tecnologia |
|---|---|
| **Framework** | .NET 8 (ASP.NET Core Web API) |
| **ORM** | Entity Framework Core 8.0 |
| **Banco** | MySQL 8.0 (produção) / SQLite (desenvolvimento) |
| **Autenticação** | JWT Bearer + Refresh Token + BCrypt |
| **Mapeamento** | AutoMapper 12 |
| **Validação** | FluentValidation 11 |
| **PDF** | QuestPDF 2026 |
| **Logging** | Serilog (console + arquivo diário) |
| **Documentação** | Swagger / OpenAPI (Swashbuckle) |
| **Testes** | xUnit + Moq + Coverlet |
| **Saúde** | Health Checks com verificação de banco |
| **Rate Limiting** | Sistema de limite de requisições integrado |

---

## Arquitetura

```
┌────────────────────────────────────────────────────────────┐
│                       Controllers                          │
│  (Auth, User, RegistroGlicose, RegistroDiario, Refeicao,   │
│   Medicamento, Relatorio)                                  │
├────────────────────────────────────────────────────────────┤
│                        Services                            │
│  (Regras de negócio, validações, orquestração)             │
├────────────────────────────────────────────────────────────┤
│                      Repositories                          │
│  (GenericRepository + específicos + soft delete)            │
├────────────────────────────────────────────────────────────┤
│                    AppDbContext (EF Core)                   │
├────────────────────────────────────────────────────────────┤
│                      MySQL / SQLite                        │
└────────────────────────────────────────────────────────────┘
        ▲                     ▲                    ▲
        │                     │                    │
┌───────┴───────┐   ┌────────┴────────┐   ┌──────┴──────┐
│  Middlewares   │   │   Validators    │   │   Mappers   │
│ (Exception,   │   │  (FluentValidation) │  (AutoMapper)│
│  TokenBlacklist)│   │                │   │             │
└───────────────┘   └─────────────────┘   └─────────────┘
```

**Padrões utilizados:**
- Repository Pattern (genérico + específico)
- Service Layer
- DTOs com AutoMapper
- Middleware Pipeline (exception handling, token blacklist)
- Background Service (expurgo automático de soft deletes)
- Soft Delete com Global Query Filters
- API Response Envelope padronizado (`ApiResponse<T>`)

---

## Funcionalidades

### Autenticação e segurança
- Cadastro de usuário com senha hash (BCrypt)
- Login com retorno de JWT + Refresh Token
- Refresh token com rotação (7 dias de validade)
- Logout com blacklist de token (JTI)
- Exclusão de conta com revogação de token
- Rate limiting: 5 req/min em auth, 100 req/min no geral
- Políticas de autorização: `AdminOnly` e `UserOrAdmin`

### Usuário
- Consulta de perfil
- Atualização de perfil (nome, email, limites glicêmicos)
- Exclusão de conta (soft delete)

### Registros de glicose (CRUD)
- Criar, listar, visualizar, atualizar e excluir medições
- Valores entre 1 e 999 mg/dL
- Classificação por momento da medição (7 momentos: antes/depois de cada refeição + antes de dormir)
- Paginação nos resultados

### Registros diários (CRUD)
- Criar, listar, visualizar, atualizar e excluir
- Observações de até 1000 caracteres

### Refeições (CRUD)
- Criar, listar, visualizar, atualizar e excluir
- Nome, descrição e observações

### Medicamentos (CRUD)
- Criar, listar, visualizar, atualizar e excluir
- Dose entre 0.1 e 1000

### Relatório
- Geração de PDF com dados de glicose por período
- Relatório pronto para impressão/download
- Gráfico donut TIR (Time in Range) por mês com resumo estatístico
- Grade glicêmica com código de cores por momento do dia
- **Links interativos**: clique nos valores da grade para saltar aos detalhes do dia
- Blocos de detalhes por dia com glicemias, medicamentos, refeições e observações
- Legenda visual dentro/fora da meta glicêmica

### Infraestrutura
- Soft delete com `DeletedAt` em todas as entidades
- Expurgo automático (background service) remove registros deletados há mais de 90 dias
- Índices compostos `(UserId, Data)` para consultas eficientes
- Migrations automáticas no startup
- Health check endpoint (`/health`)
- Logging estruturado com Serilog
- CORS configurável por ambiente
- Suporte a proxy reverso (Forwarded Headers)
- Seed data com 378 registros de exemplo

---

## Endpoints

### Auth (rate limit: 5 requisições/minuto)

| Método | Rota | Descrição | Autenticação |
|---|---|---|---|
| `POST` | `/api/Auth/register` | Cadastro de novo usuário | Não |
| `POST` | `/api/Auth/login` | Login, retorna JWT + refresh token | Não |
| `POST` | `/api/Auth/refresh` | Renova o token usando refresh token | Não |
| `POST` | `/api/Auth/logout` | Revoga o token atual | Sim |

### User

| Método | Rota | Descrição | Autenticação |
|---|---|---|---|
| `GET` | `/api/User/profile` | Retorna perfil do usuário logado | Sim |
| `PUT` | `/api/User/profile` | Atualiza perfil | Sim |
| `DELETE` | `/api/User/account` | Exclui conta (soft delete) | Sim |

### Registro de Glicose

| Método | Rota | Descrição | Autenticação |
|---|---|---|---|
| `GET` | `/api/RegistroGlicose` | Lista registros paginados | Sim |
| `GET` | `/api/RegistroGlicose/{id}` | Obtém registro por ID | Sim |
| `POST` | `/api/RegistroGlicose` | Cria novo registro | Sim |
| `PUT` | `/api/RegistroGlicose/{id}` | Atualiza registro | Sim |
| `DELETE` | `/api/RegistroGlicose/{id}` | Remove registro (soft delete) | Sim |

### Registro Diário

| Método | Rota | Descrição | Autenticação |
|---|---|---|---|
| `GET` | `/api/RegistroDiario` | Lista registros paginados | Sim |
| `GET` | `/api/RegistroDiario/{id}` | Obtém registro por ID | Sim |
| `POST` | `/api/RegistroDiario` | Cria novo registro | Sim |
| `PUT` | `/api/RegistroDiario/{id}` | Atualiza registro | Sim |
| `DELETE` | `/api/RegistroDiario/{id}` | Remove registro (soft delete) | Sim |

### Refeição

| Método | Rota | Descrição | Autenticação |
|---|---|---|---|
| `GET` | `/api/Refeicao` | Lista refeições paginadas | Sim |
| `GET` | `/api/Refeicao/{id}` | Obtém refeição por ID | Sim |
| `POST` | `/api/Refeicao` | Cria nova refeição | Sim |
| `PUT` | `/api/Refeicao/{id}` | Atualiza refeição | Sim |
| `DELETE` | `/api/Refeicao/{id}` | Remove refeição (soft delete) | Sim |

### Medicamento

| Método | Rota | Descrição | Autenticação |
|---|---|---|---|
| `GET` | `/api/Medicamento` | Lista medicamentos paginados | Sim |
| `GET` | `/api/Medicamento/{id}` | Obtém medicamento por ID | Sim |
| `POST` | `/api/Medicamento` | Cria novo medicamento | Sim |
| `PUT` | `/api/Medicamento/{id}` | Atualiza medicamento | Sim |
| `DELETE` | `/api/Medicamento/{id}` | Remove medicamento (soft delete) | Sim |

### Relatório

| Método | Rota | Descrição | Autenticação |
|---|---|---|---|
| `POST` | `/api/Relatorio/gerar` | Gera PDF com dados por período | Sim |

### Health Check

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/health` | Status da API e conexão com banco |

---

## Pré-requisitos

- .NET SDK 8.0+
- MySQL 8.0 (para produção ou teste com MySQL)
- `dotnet-ef` (para migrations)

Instalação do EF CLI:

```bash
dotnet tool install --global dotnet-ef
```

---

## Configuração local

### Desenvolvimento (SQLite)

O projeto já usa SQLite automaticamente em ambiente `Development`. Basta rodar:

```bash
# Clone
git clone https://github.com/SEU-USUARIO/SEU-REPOSITORIO.git
cd ControleGlicemia.API

# Restaure dependências
dotnet restore

# Rode a API (migrations são aplicadas automaticamente)
dotnet run
```

### Produção (MySQL)

1. Configure a connection string no ambiente ou no `appsettings.Production.json`
2. Execute as migrations:

```bash
dotnet ef database update
```

3. Rode a API:

```bash
dotnet run --configuration Release
```

---

## Variáveis de ambiente

| Variável | Obrigatória | Descrição | Padrão |
|---|---|---|---|
| `Jwt__Key` / `JWT__KEY` | Sim | Chave secreta para assinar o JWT (mínimo 32 caracteres) | — |
| `Jwt__Issuer` / `JWT__ISSUER` | Não | Emissor do token | `ControleGlicemiaAPI` |
| `Jwt__Audience` / `JWT__AUDIENCE` | Não | Audiência do token | `ControleGlicemiaApp` |
| `ConnectionStrings__DefaultConnection` | Sim (produção) | String de conexão MySQL | — |
| `Cors__AllowedOrigins__0` | Não | Origem permitida no CORS | — |

> **Nunca versionar credenciais reais.** Use variáveis de ambiente ou `User Secrets` em desenvolvimento.

---

## Autenticação

O fluxo completo de autenticação funciona assim:

```
1. POST /api/Auth/register → cadastro com email + senha
2. POST /api/Auth/login    → retorna { token, refreshToken }
3. Use o token no header:  Authorization: Bearer <token>
4. Quando expirar (1h):   POST /api/Auth/refresh { refreshToken }
5. Para sair:              POST /api/Auth/logout (blacklist do token)
```

### Exemplo

**Cadastro:**

```json
POST /api/Auth/register
{
  "nome": "Victor",
  "email": "victor@email.com",
  "password": "Senha123",
  "confirmPassword": "Senha123"
}
```

**Login:**

```json
POST /api/Auth/login
{
  "email": "victor@email.com",
  "password": "Senha123"
}
```

**Resposta:**

```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "a1b2c3d4e5f6..."
  }
}
```

### Claims do JWT

| Claim | Descrição |
|---|---|
| `nameid` | ID do usuário |
| `email` | Email do usuário |
| `name` | Nome do usuário |
| `role` | Papel (User / Admin) |
| `jti` | ID único do token (para blacklist) |
| `exp` | Expiração (1 hora) |

---

## Testes

O projeto possui testes com **xUnit + Moq**. Execute com:

```bash
dotnet test
```

---

## Validações de negócio

- **Email único** — não permite cadastro com email já existente
- **Senha forte** — mínimo 8 caracteres, 1 letra maiúscula, 1 dígito
- **Datas não podem ser futuras** — tolerância de 5 minutos
- **Faixa de glicose** — valor entre 1 e 999 mg/dL
- **Faixa de dose** — medicamento entre 0.1 e 1000
- **Limites de texto** — nome (100 chars), observações (300-1000 chars)
- **Enum válido** — `MomentoMedicao` validado (7 valores possíveis)
- **GlicemiaMinima < GlicemiaMaxima** — validação na atualização de perfil
- **Isolamento por usuário** — cada usuário vê apenas seus próprios dados

---

## Estrutura de pastas

```
ControleGlicemia.API/
├── Controllers/          # Endpoints HTTP
│   ├── AuthController.cs
│   ├── UserController.cs
│   ├── RegistroGlicoseController.cs
│   ├── RegistroDiarioController.cs
│   ├── RefeicaoController.cs
│   ├── MedicamentoController.cs
│   └── RelatorioController.cs
├── Services/             # Regras de negócio
│   ├── I{...}Service.cs / {Entity}Service.cs
│   └── ExpurgoService.cs          # Background service
├── Repositories/         # Acesso a dados
│   ├── IGenericRepository.cs / GenericRepository.cs
│   ├── I{...}Repository.cs / {Entity}Repository.cs
│   ├── IRelatorioRepository.cs / RelatorioRepository.cs
│   └── ITokenBlacklistRepository.cs / TokenBlacklistRepository.cs
├── Models/               # Entidades de domínio
│   ├── User.cs
│   ├── RegistroGlicose.cs
│   ├── RegistroDiario.cs
│   ├── Refeicao.cs
│   ├── Medicamento.cs
│   ├── TokenBlacklistEntry.cs
│   ├── PagedResult.cs
│   ├── MomentoMedicao.cs          # Enum
│   └── ISoftDeletable.cs         # Interface
├── DTOs/                 # Contratos de entrada/saída
│   ├── User/
│   ├── RegistroGlicose/
│   ├── RegistroDiario/
│   ├── Refeicao/
│   ├── Medicamento/
│   └── Relatorio/
├── Mappers/              # AutoMapper
│   └── MappingProfile.cs
├── Validators/           # FluentValidation
│   ├── Create{...}Validator.cs
│   ├── Update{...}Validator.cs
│   ├── RegisterDtoValidator.cs
│   ├── LoginDtoValidator.cs
│   └── UpdateUserProfileDtoValidator.cs
├── Middlewares/          # Pipeline HTTP
│   ├── ExceptionHandlingMiddleware.cs
│   └── TokenBlacklistMiddleware.cs
├── Extensions/           # Métodos de extensão
│   └── ClaimsPrincipalExtensions.cs
├── Data/                 # EF Core
│   └── AppDbContext.cs
├── Program.cs            # Entry point
├── ApiResponse.cs        # Response envelope genérico
├── appsettings.json
├── appsettings.Development.json
├── appsettings.Production.json
├── seed_glicose.sql      # 378 registros de exemplo
└── ControleGlicemia.API.csproj

ControleGlicemia.API.Tests/        # Projeto de testes
├── Controllers/
├── Services/
├── Repositories/
├── Middlewares/
├── Validators/
├── Mappers/
├── Extensions/
└── ControleGlicemia.API.Tests.csproj
```

---

## Seed data

O arquivo `seed_glicose.sql` contém **378 registros de glicose** (01/06/2026 a 24/07/2026) para popular o banco durante desenvolvimento/testes.

Para importar no MySQL:

```bash
mysql -u usuario -p controle_glicemia < seed_glicose.sql
```

---

## Autor

Desenvolvido por **Victor Turques**.
