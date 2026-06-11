# ControleGlicemia.API

API REST para controle glicêmico com autenticação JWT, gerenciamento de registros de glicose, refeições, medicamentos, registros diários e geração de relatório em PDF.

---

## 📚 Sumário

- [Visão geral](#-visão-geral)
- [Tecnologias](#-tecnologias)
- [Arquitetura](#-arquitetura)
- [Funcionalidades](#-funcionalidades)
- [Endpoints](#-endpoints)
- [Pré-requisitos](#-pré-requisitos)
- [Configuração local](#-configuração-local)
- [Variáveis de ambiente](#-variáveis-de-ambiente)
- [Validações de negócio](#-validações-de-negócio)
- [Estrutura de pastas](#-estrutura-de-pastas)
- [Autor](#-autor)

---

## 📌 Visão geral

O **ControleGlicemia.API** foi desenvolvido para ajudar no acompanhamento de saúde de pessoas com necessidade de monitoramento glicêmico, com foco em:

- Histórico de medições de glicose;
- Registro de rotina diária;
- Controle de refeições e medicamentos;
- Geração de relatório clínico em PDF por período.

---

## 🛠 Tecnologias

- **.NET 8** (ASP.NET Core Web API)
- **Entity Framework Core**
- **MySQL** com `Pomelo.EntityFrameworkCore.MySql`
- **JWT Bearer Authentication**
- **AutoMapper**
- **QuestPDF** (geração de PDF)
- **Swagger / OpenAPI**

---

## 🧱 Arquitetura

Projeto organizado em camadas:

- **Controllers**: entrada HTTP e respostas da API
- **Services**: regras de negócio e validações
- **Repositories**: persistência/acesso a dados
- **Models**: entidades de domínio
- **DTOs**: contratos de entrada e saída
- **Mappers**: mapeamentos com AutoMapper
- **Data**: contexto do banco (`AppDbContext`)

---

## ✅ Funcionalidades

- Cadastro e login com retorno de token JWT
- Consulta e atualização de perfil do usuário
- CRUD de:
  - Registro de glicose
  - Registro diário
  - Refeição
  - Medicamento
- Geração de relatório em PDF
- Isolamento de dados por usuário autenticado
- Índices no banco para consultas por usuário/data

---

## 🔗 Endpoints

### Auth
- `POST /api/Auth/register`
- `POST /api/Auth/login`

### Usuário
- `GET /api/User/profile`
- `PUT /api/User/profile`

### Registro de Glicose
- `GET /api/RegistroGlicose`
- `GET /api/RegistroGlicose/{id}`
- `POST /api/RegistroGlicose`
- `PUT /api/RegistroGlicose/{id}`
- `DELETE /api/RegistroGlicose/{id}`

### Registro Diário
- `GET /api/RegistroDiario`
- `GET /api/RegistroDiario/{id}`
- `POST /api/RegistroDiario`
- `PUT /api/RegistroDiario/{id}`
- `DELETE /api/RegistroDiario/{id}`

### Refeição
- `GET /api/Refeicao`
- `GET /api/Refeicao/{id}`
- `POST /api/Refeicao`
- `PUT /api/Refeicao/{id}`
- `DELETE /api/Refeicao/{id}`

### Medicamento
- `GET /api/Medicamento`
- `GET /api/Medicamento/{id}`
- `POST /api/Medicamento`
- `PUT /api/Medicamento/{id}`
- `DELETE /api/Medicamento/{id}`

### Relatório
- `POST /api/Relatorio/gerar` → retorna arquivo PDF

---

## 📦 Pré-requisitos

- .NET SDK 8+
- MySQL
- `dotnet-ef` (ferramenta de migrations)

Instalação do EF CLI (se necessário):

```bash
dotnet tool install --global dotnet-ef
```

---

## ⚙️ Configuração local

1. Clone o repositório:

```bash
git clone https://github.com/SEU-USUARIO/SEU-REPOSITORIO.git
cd SEU-REPOSITORIO
```

2. Configure as variáveis no `appsettings.Development.json`.

3. Restaure e compile:

```bash
dotnet restore
dotnet build
```

4. Execute migrations:

```bash
dotnet ef database update
```

5. Rode a API:

```bash
dotnet run
```

---

## 🔐 Variáveis de ambiente

Exemplo de configuração:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=controle_glicemia;User=seu_usuario;Password=sua_senha;"
  },
  "Jwt": {
    "Key": "SUA_CHAVE_JWT_FORTE",
    "Issuer": "ControleGlicemiaAPI",
    "Audience": "ControleGlicemiaApp"
  }
}
```

> Nunca versionar credenciais reais.

---

## 🧪 Exemplo rápido de autenticação

### Cadastro

`POST /api/Auth/register`

```json
{
  "username": "Victor",
  "email": "victor@email.com",
  "password": "123456"
}
```

### Login

`POST /api/Auth/login`

```json
{
  "email": "victor@email.com",
  "password": "123456"
}
```

Resposta esperada:

```json
{
  "token": "..."
}
```

Use o token no header:

```http
Authorization: Bearer SEU_TOKEN
```

---

## 📏 Validações de negócio

Algumas regras implementadas:

- Datas não podem ser futuras (com tolerância de 5 minutos);
- Email único para usuário;
- `GlicemiaMinima < GlicemiaMaxima`;
- Limites de tamanho para campos de observação e nome;
- Faixa de valores para glicose e dose de medicamento;
- Validação de enum para `MomentoMedicao`.

---

## 🗂 Estrutura de pastas

```text
ControleGlicemia.API/
├── Controllers/
├── Services/
├── Repositories/
├── Models/
├── DTOs/
├── Mappers/
├── Data/
├── Program.cs
└── ControleGlicemia.API.csproj
```

---

## 👨‍💻 Autor

Desenvolvido por **Victor Turques**.
