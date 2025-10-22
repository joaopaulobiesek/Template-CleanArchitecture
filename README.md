# 🏗️ Clean Architecture Template

<div align="center">

**Template moderno de Clean Architecture com DDD, CQRS para ASP.NET Core**

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](CONTRIBUTING.md)

• [Reportar Bug](https://github.com/joaopaulobiesek/Template-CleanArchitecture-MultiTenancy/issues) • [Solicitar Feature](https://github.com/joaopaulobiesek/Template-CleanArchitecture-MultiTenancy/issues)

</div>

---

## 📋 Índice

- [Sobre o Projeto](#-sobre-o-projeto)
- [Diferenciais](#-diferenciais)
- [Arquitetura](#-arquitetura)
- [Features Principais](#-features-principais)
- [Quick Start](#-quick-start)
- [Exemplo Prático](#-exemplo-prático)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Tecnologias](#-tecnologias)
- [Documentação](#-documentação)
- [Contribuindo](#-contribuindo)
- [Licença](#-licença)

---

## 🎯 Sobre o Projeto

Este template implementa uma **Clean Architecture** robusta e moderna, combinando os melhores padrões de **Domain-Driven Design (DDD)** e **CQRS**, com suporte nativo a **Multi-Tenancy**. Desenvolvido para ser o ponto de partida ideal para aplicações empresariais escaláveis.

### Por que usar este template?

✅ **Arquitetura limpa e organizada** - Separação clara de responsabilidades
✅ **Produtividade máxima** - Menos código boilerplate, mais foco no negócio
✅ **Multi-Tenancy nativo** - Suporte completo a múltiplos clientes
✅ **Testável por design** - Estrutura que facilita testes unitários e de integração
✅ **Pronto para produção** - Logging, validação, autorização e tratamento de erros inclusos
✅ **Extensível** - Fácil adicionar novas features seguindo os padrões estabelecidos

---

## 🚀 Diferenciais

### 1️⃣ Sistema de Handlers Customizado

**Não usa MediatR** - implementação própria que oferece maior controle e simplicidade.

```csharp
public class CreateClientCommandHandler : HandlerBase<CreateClientCommand, ClientVM>
{
    public CreateClientCommandHandler(HandlerDependencies<CreateClientCommand, ClientVM> dependencies)
        : base(dependencies) { }

    protected override async Task<ApiResponse<ClientVM>> RunCore(
        CreateClientCommand request,
        CancellationToken cancellationToken,
        object? additionalData = null)
    {
        var client = new Client();
        client.CreateClient(request);

        await _context.Clients.AddAsync(client, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<ClientVM>("Cliente criado!", new ClientVM(...));
    }
}
```

### 2️⃣ Autorização Declarativa no Command/Query

**Não autoriza na Controller** - autorização é declarada diretamente nos Commands/Queries.

```csharp
[Authorize(Roles = Roles.Admin)]
[Authorize(Policy = Policies.CanCreate)]
public class CreateClientCommand : ICreateClient
{
    public string FullName { get; set; }
    public string DocumentNumber { get; set; }
    // ...
}
```

### 3️⃣ Behaviors Pipeline Automático

Cada requisição passa por um pipeline de behaviors automaticamente:

```
📝 Logging → ⏱️ Performance → ⚠️ Exception Handling →
✅ Validation → 🔒 Authorization → 🏢 Module Validation →
🎯 Lógica de Negócio
```

### 4️⃣ HandlerDependencies - Injeção Automática

Dependências comuns são injetadas automaticamente:

```csharp
protected ITenantContext _context;  // Contexto do banco
protected ICurrentUser _user;       // Usuário autenticado (com _user.Id)
// + Logger, IdentityService, Environment, ServiceProvider
```

### 5️⃣ InternalAuthContext - Bypass de Autorização

Permite que serviços internos confiáveis (WhatsApp Bot, Hangfire Jobs) executem Commands com bypass de autorização:

```csharp
// WhatsApp Bot pode criar um cliente sem ter permissão de Admin
var result = await InternalAuthContext.ExecuteAsTrustedAsync(userId, async () =>
    await _createClientHandler.Execute(command, CancellationToken.None));
```

### 6️⃣ Paginação Otimizada com IQueryable

Repositórios sempre retornam `IQueryable<T>` para paginação eficiente:

```csharp
public IQueryable<Client> SearchIQueryable(string? src, Dictionary<string, string>? customFilter)
{
    IQueryable<Client> query = _context.Clients.AsQueryable();

    // Filtros aplicados sem materializar
    if (!string.IsNullOrWhiteSpace(src))
        query = query.Where(x => x.FullName.Contains(src));

    return query; // Retorna IQueryable, não ToList()!
}
```

### 7️⃣ Multi-Tenancy Integrado

Suporte completo a múltiplos clientes com isolamento de dados:

```csharp
[ApiExplorerSettings(GroupName = "Tenant.Api.v1")]
public class ClientController : BaseController { }
```

---

## 🏛️ Arquitetura

### Fluxo de uma Requisição

```
┌─────────────────────────────────────────────┐
│           HTTP Request                      │
│     POST /tenant/api/v1/clients            │
└────────────────┬────────────────────────────┘
                 ↓
┌─────────────────────────────────────────────┐
│         TenantMiddleware                    │
│   Identifica o Tenant (Tenant.Api.v1)      │
└────────────────┬────────────────────────────┘
                 ↓
┌─────────────────────────────────────────────┐
│          ClientController                   │
│   Resolve IHandlerBase<Command>            │
└────────────────┬────────────────────────────┘
                 ↓
┌─────────────────────────────────────────────┐
│      CreateClientCommandHandler             │
│        handler.Execute(command)             │
└────────────────┬────────────────────────────┘
                 ↓
┌─────────────────────────────────────────────┐
│         BEHAVIORS PIPELINE                  │
│  Logging → Performance → Exception →        │
│  Validation → Authorization → Module        │
└────────────────┬────────────────────────────┘
                 ↓
┌─────────────────────────────────────────────┐
│      Lógica de Negócio (RunCore)           │
│   Domain Entity → Validation → Repository  │
└────────────────┬────────────────────────────┘
                 ↓
┌─────────────────────────────────────────────┐
│       ApiResponse<ClientVM>                 │
│         HTTP Response                       │
└─────────────────────────────────────────────┘
```

### Camadas

```
┌──────────────────────────────────────────────┐
│          Template.Api (Apresentação)         │
│    Controllers • Middlewares • Program.cs    │
└──────────────────────────────────────────────┘
                    ↓
┌──────────────────────────────────────────────┐
│      Template.Application (Aplicação)        │
│  Commands • Queries • Handlers • Behaviors   │
└──────────────────────────────────────────────┘
                    ↓
┌──────────────────────────────────────────────┐
│        Template.Domain (Domínio)             │
│     Entities • Validations • Interfaces      │
└──────────────────────────────────────────────┘
                    ↓
┌──────────────────────────────────────────────┐
│      Template.Infra (Infraestrutura)         │
│  DB • Identity • External Services • Repos   │
└──────────────────────────────────────────────┘
```

---

## ✨ Features Principais

### 🔐 Autenticação e Autorização

- ✅ JWT Authentication
- ✅ Role-based Authorization (Admin, User, Exhibitor, ServiceProvider)
- ✅ Policy-based Authorization (18 policies granulares)
- ✅ Autorização declarativa nos Commands/Queries
- ✅ InternalAuthContext para bypass de autorização (serviços confiáveis)

### ✅ Validação em Dois Níveis

- ✅ **FluentValidation** - Validação de entrada (Commands/Queries)
- ✅ **DomainExceptionValidation** - Validação de regras de negócio (Entities)

### 📊 Paginação e Filtros

- ✅ `BasePaginatedQuery` - Base para queries paginadas
- ✅ `PaginatedList<T>` - Resposta paginada padronizada
- ✅ Ordenação dinâmica (ASC/DESC por qualquer coluna)
- ✅ Busca textual integrada
- ✅ Filtros customizados via JSON (com whitelist de segurança)

### 🏢 Multi-Tenancy

- ✅ Isolamento de dados por tenant
- ✅ Identificação automática via GroupName
- ✅ Validação de módulos habilitados por tenant
- ✅ Middleware de contexto de tenant

### 📝 Logging e Monitoramento

- ✅ Logging automático de todas as requisições
- ✅ Medição de performance (alerta se > 500ms)
- ✅ Rastreamento de usuário e operação
- ✅ Structured logging com Serilog

### 🔧 Integração com Serviços Externos

- ✅ Azure Blob Storage (upload/download de arquivos)
- ✅ SendGrid (envio de e-mails)
- ✅ Google OAuth (autenticação externa)
- ✅ Google Calendar API (integração de calendário)

### 🎨 Swagger/OpenAPI

- ✅ Documentação automática da API
- ✅ Suporte a versionamento
- ✅ Documentação XML integrada
- ✅ Testes interativos

---

## 🚦 Quick Start

### Pré-requisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server) ou [PostgreSQL](https://www.postgresql.org/)
- (Opcional) [Azure Storage Account](https://azure.microsoft.com/services/storage/)

### Instalação

1. **Clone o repositório**

```bash
git clone https://github.com/joaopaulobiesek/Template-CleanArchitecture-MultiTenancy.git
cd Template-CleanArchitecture-MultiTenancy
```

2. **Configure o `appsettings.json`**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TemplateDb;User Id=sa;Password=YourPassword;"
  },
  "JwtConfiguration": {
    "Secret": "your-super-secret-key-min-32-chars",
    "Issuer": "YourApp",
    "Audience": "YourAppUsers"
  }
}
```

3. **Execute as migrations**

```bash
dotnet ef database update --project Template.Infra --startup-project Template.Api
```

4. **Execute o projeto**

```bash
dotnet run --project Template.Api
```

5. **Acesse a documentação Swagger**

```
https://localhost:7048/swagger
```

---

## 💡 Exemplo Prático

### Criar um novo CRUD completo

#### 1. Crie a Entidade de Domínio

**`Template.Domain/Entity/Tenant/Product.cs`**

```csharp
public sealed class Product : Entity
{
    public string Name { get; private set; }
    public decimal Price { get; private set; }

    public void CreateProduct(ICreateProduct p)
    {
        DomainExceptionValidation.ValidateRequiredString(p.Name, "Nome obrigatório.");
        DomainExceptionValidation.When(p.Price <= 0, "Preço inválido.");

        Name = p.Name;
        Price = p.Price;
    }
}
```

#### 2. Crie o Command

**`Template.Application/Domains/Tenant/V1/Products/Commands/CreateProduct/CreateProductCommand.cs`**

```csharp
[Authorize(Roles = Roles.Admin)]
[Authorize(Policy = Policies.CanCreate)]
public class CreateProductCommand : ICreateProduct
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}
```

#### 3. Crie o Handler

**`CreateProductCommandHandler.cs`**

```csharp
public class CreateProductCommandHandler : HandlerBase<CreateProductCommand, ProductVM>
{
    public CreateProductCommandHandler(
        HandlerDependencies<CreateProductCommand, ProductVM> dependencies)
        : base(dependencies) { }

    protected override async Task<ApiResponse<ProductVM>> RunCore(
        CreateProductCommand request,
        CancellationToken cancellationToken,
        object? additionalData = null)
    {
        var product = new Product();
        product.CreateProduct(request);

        await _context.Products.AddAsync(product, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<ProductVM>(
            "Produto criado com sucesso!",
            new ProductVM(product.Id, product.Name, product.Price)
        );
    }
}
```

#### 4. Crie o Validator

**`CreateProductCommandValidator.cs`**

```csharp
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(200);

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Preço deve ser maior que zero.");
    }
}
```

#### 5. Crie o Controller

**`Template.Api/Controllers/Tenant/V1/Products/ProductController.cs`**

```csharp
[ApiController]
[Route("tenant/api/v1/[controller]")]
[ApiExplorerSettings(GroupName = "Tenant.Api.v1")]
public class ProductController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> CreateAsync(
        [FromServices] IHandlerBase<CreateProductCommand, ProductVM> handler,
        [FromBody] CreateProductCommand command,
        CancellationToken cancellationToken)
        => HandleResponse(await handler.Execute(command, cancellationToken));
}
```

**Pronto!** Você criou um endpoint completo com:
- ✅ Validação de entrada (FluentValidation)
- ✅ Validação de domínio (DomainExceptionValidation)
- ✅ Autorização (Roles + Policies)
- ✅ Logging automático
- ✅ Tratamento de exceções
- ✅ Medição de performance
- ✅ Resposta padronizada

---

## 📁 Estrutura do Projeto

```
Template.CleanArchitecture/
│
├── 📄 README.md                    # Este arquivo
│
├── 🎨 Template.Api/
│   ├── Controllers/
│   │   ├── Tenant/V1/              # Controllers multi-tenant
│   │   ├── V1/                     # Controllers globais
│   │   └── System/                 # Base + Health Check
│   ├── Middlewares/
│   │   ├── ExceptionHandlingMiddleware.cs
│   │   └── TenantMiddleware.cs
│   └── Program.cs
│
├── 🧩 Template.Application/
│   ├── Common/
│   │   ├── Behaviours/             # Pipeline (7 behaviors)
│   │   ├── Interfaces/             # Contratos de Repos e Services
│   │   ├── Models/                 # ApiResponse, PaginatedList
│   │   └── Security/               # Authorize, InternalAuthContext
│   ├── Domains/
│   │   ├── Tenant/V1/              # Features multi-tenant
│   │   └── V1/                     # Features globais
│   └── ViewModels/
│
├── 🏛️ Template.Domain/
│   ├── Constants/
│   │   ├── Roles.cs                # 2 roles padrão
│   │   └── Policies.cs             # 18 policies granulares
│   ├── Entity/
│   │   └── Entity.cs               # Base class (Id, Active, Timestamps)
│   ├── Interfaces/                 # Contratos de domínio
│   └── Validations/
│       ├── DomainExceptionValidation.cs
│       ├── CPFValidationAttribute.cs
│       └── CNPJValidationAttribute.cs
│
└── 🔧 Template.Infra/
    ├── ExternalServices/
    │   ├── Google/                 # OAuth + Calendar API
    │   ├── SendEmails/             # SendGrid
    │   └── Storage/                # Azure Blob Storage
    ├── Identity/
    │   ├── IdentityService.cs      # Gerenciamento de usuários
    │   ├── TokenService.cs         # Geração de JWT
    │   └── CurrentUser.cs          # Usuário autenticado
    ├── Persistence/
    │   ├── Contexts/               # EF Core + Dapper
    │   ├── Migrations/
    │   └── Repositories/
    └── Settings/
        ├── Configurations/         # JWT, SendGrid, Azure, etc
        └── Maps/                   # EF Core mappings
```

---

## 🛠️ Tecnologias

### Core

- **[.NET 8.0](https://dotnet.microsoft.com/)** - Framework principal
- **[ASP.NET Core](https://docs.microsoft.com/aspnet/core)** - Web API
- **[Entity Framework Core](https://docs.microsoft.com/ef/core)** - ORM
- **[Dapper](https://github.com/DapperLib/Dapper)** - Micro ORM para queries otimizadas

### Validação e Segurança

- **[FluentValidation](https://fluentvalidation.net/)** - Validação de entrada
- **[JWT Bearer](https://jwt.io/)** - Autenticação
- **[ASP.NET Core Identity](https://docs.microsoft.com/aspnet/core/security/authentication/identity)** - Gerenciamento de usuários

### Integração

- **[Azure.Storage.Blobs](https://azure.microsoft.com/services/storage/)** - Armazenamento de arquivos
- **[SendGrid](https://sendgrid.com/)** - Envio de e-mails
- **[Google.Apis](https://developers.google.com/api-client-library/dotnet)** - OAuth + Calendar API

### Documentação e Testes

- **[Swagger/OpenAPI](https://swagger.io/)** - Documentação interativa
- **[Serilog](https://serilog.net/)** - Structured logging
- **[xUnit](https://xunit.net/)** - Framework de testes (recomendado)

### Background Jobs

- **[Hangfire](https://www.hangfire.io/)** - Processamento em background

---

## 🤝 Contribuindo

Contribuições são muito bem-vindas! Este template foi criado para ser um ponto de partida sólido para aplicações empresariais.

### Como Contribuir

1. **Fork** o projeto
2. Crie uma **branch** para sua feature (`git checkout -b feature/AmazingFeature`)
3. **Commit** suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. **Push** para a branch (`git push origin feature/AmazingFeature`)
5. Abra um **Pull Request**

### Diretrizes

- Adicione testes para novas features
- Atualize a documentação se necessário
- Mantenha o código limpo e bem comentado

---

## 📝 Roadmap

- [ ] Adicionar suporte a PostgreSQL
- [ ] Implementar CQRS completo com Event Sourcing
- [ ] Adicionar testes unitários e de integração
- [ ] Adicionar suporte a Docker
- [ ] Implementar cache distribuído (Redis)
- [ ] Adicionar exemplos de Notification Pattern
- [ ] Implementar Soft Delete global
- [ ] Adicionar suporte a Audit Logs

---

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

## 👤 Autor

**João Paulo Biesek**

- GitHub: [@joaopaulobiesek](https://github.com/joaopaulobiesek)
- LinkedIn: [João Paulo Biesek](https://www.linkedin.com/in/joaopaulobiesek)

---

## 🌟 Mostre seu Apoio

Se este template foi útil para você, dê uma ⭐️ no projeto!

---

## 🙏 Agradecimentos

- Comunidade .NET
- Clean Architecture by Robert C. Martin
- Domain-Driven Design by Eric Evans
- Todos os contribuidores do projeto

---

<div align="center">

**[⬆ Voltar ao topo](#-clean-architecture-multi-tenancy-template)**

Feito com ❤️ por [João Paulo Biesek](https://github.com/joaopaulobiesek)

</div>
