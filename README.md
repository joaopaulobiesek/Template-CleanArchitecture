# Template Project

Este projeto implementa uma arquitetura modular robusta com base em **Domain-Driven Design (DDD)** e **Command Query Responsibility Segregation (CQRS)**. Desenvolvido em C#, ele visa gerenciar operações de clientes, autenticação de usuários, armazenamento de arquivos e integrações com serviços externos. A estrutura é altamente desacoplada, permitindo a extensibilidade, fácil manutenção e testes eficazes.

## Arquitetura

Este projeto adota os seguintes princípios e padrões arquiteturais:

- **Domain-Driven Design (DDD)**: Estrutura o código em torno do domínio central, separando as responsabilidades e encapsulando a lógica de negócio em entidades e serviços.
- **CQRS (Command Query Responsibility Segregation)**: Separa as operações de leitura e escrita, otimizando a aplicação para desempenho e manutenibilidade.
- **Injeção de Dependência**: Facilita o desacoplamento de componentes, utilizando um contêiner de injeção de dependência para permitir flexibilidade e facilitar os testes.

A arquitetura do projeto está organizada em camadas distintas:

1. **Template.Api**: Expõe os endpoints da API e organiza os controladores e middlewares para comunicação com outras camadas.
2. **Template.Application**: Implementa a lógica de aplicação, seguindo CQRS para gerenciar comandos e consultas, além de comportamentos transversais como autorização e validação.
3. **Template.Domain**: Contém a lógica central de negócio e as entidades, com um design orientado pelo domínio.
4. **Template.Infra**: Lida com a persistência de dados, integração com serviços externos e gerencia configurações.

Cada camada é estruturada para facilitar a navegação, leitura e manutenção do código.

### 1. Template.Api

A camada **API** é responsável por expor os endpoints da aplicação, organizando os controladores e middlewares que permitem a interação com as camadas de domínio e infraestrutura. Abaixo está a estrutura da camada API, com detalhes sobre os controladores e configurações.

#### Estrutura da Camada API

```plaintext
Template.Api
├── appsettings.json               # Configurações de ambiente de produção.
├── appsettings.Development.json   # Configurações de desenvolvimento.
├── Program.cs                     # Arquivo principal que inicializa a aplicação.
├── Template.Api.csproj            # Arquivo de configuração do projeto.
├── Controllers
│   ├── ExternalServices
│   │   ├── Files
│   │   │    └── FileController.cs        # Controlador para operações de gerenciamento de arquivos.
│   │   └── Google
│   │       └── GoogleController.cs       # Controlador para operações de gerenciamento do google.
│   ├── Identity
│   │   ├── Auth
│   │   │   └── AuthController.cs         # Controlador para autenticação e geração de tokens.
│   │   └── Users
│   │       └── UsersController.cs        # Controlador para gerenciamento de usuários.
│   ├── InternalServices
│   │   └── Clients
│   │       └── ClientController.cs       # Controlador para operações de cliente.
│   └── System
│       ├── BaseController.cs             # Controlador base para endpoints comuns.
│       └── HealthController.cs           # Controlador de verificação de saúde da API.
├── Middlewares
│   └── ExceptionHandlingMiddleware.cs    # Middleware para tratamento centralizado de exceções.
└── Properties
    └── launchSettings.json               # Configurações de lançamento da aplicação.
```

#### Descrição dos Principais Componentes

- **appsettings.json** e **appsettings.Development.json**: Contêm configurações específicas para produção e desenvolvimento, respectivamente, incluindo conexões e variáveis de ambiente.
- **Controllers**:
  - `FileController`: Permite o gerenciamento e upload de arquivos.
  - `AuthController` e `UsersController`: Tratam autenticação e operações relacionadas a usuários.
  - `ClientController`: Gerencia operações específicas para a entidade Cliente.
  - `HealthController`: Endpoint de saúde para monitorar o status da API.
- **ExceptionHandlingMiddleware.cs**: Middleware responsável por capturar exceções e padronizar as respostas de erro.

Essa camada permite a interação direta com o sistema, organizando os recursos e endpoints para fácil acesso e manutenção.

### 2. Template.Application

A camada **Application** contém a lógica de aplicação e implementa os padrões de **CQRS (Command Query Responsibility Segregation)**, organizando os comandos e consultas que manipulam as operações principais do sistema. Abaixo está a estrutura da camada Application, com detalhes sobre os behaviors, serviços, interfaces e modelos.

#### Estrutura da Camada Application

```plaintext
Template.Application
├── DependencyInjection.cs               # Configurações para injeção de dependência.
├── Common
│   ├── Behaviours                       # Comportamentos que gerenciam o fluxo de execução.
│   │   ├── AuthorizationBehaviour.cs
│   │   ├── HandlerBase.cs
│   │   ├── HandlerDependencies.cs
│   │   ├── LoggingBehaviour.cs
│   │   ├── PerformanceBehaviour.cs
│   │   ├── UnhandledExceptionBehaviour.cs
│   │   └── ValidationBehaviour.cs
│   ├── Exceptions
│   │   └── ForbiddenAccessException.cs   # Exceção para acesso não autorizado.
│   ├── Interfaces                        # Interfaces de repositórios e serviços.
│   │   ├── IRepositories
│   │   │   ├── Base/IRepository.cs
│   │   │   └── Implementations/IClientRepository.cs
│   │   ├── Security
│   │   │   ├── ICurrentUser.cs
│   │   │   ├── IIdentityService.cs
│   │   │   └── IUser.cs
│   │   └── Services
│   │       ├── IGoogle.cs
│   │       ├── ISendGrid.cs
│   │       └── IStorage.cs
│   ├── Models                            # Modelos de dados comuns.
│   │   ├── ApiResponse.cs
│   │   ├── GoogleCalendarEvent.cs
│   │   ├── PaginatedList.cs
│   │   └── User.cs
│   ├── Persistence
│   │   ├── IContext.cs                   # Interface para contexto de dados.
│   │   └── IDapperConnection.cs          # Interface para conexão com Dapper.
│   └── Security
│       └── AuthorizeAttribute.cs         # Atributo de autorização.
├── Domains                               # Comandos e consultas organizados por domínio.
│   ├── ExternalServices
│   │   └── Storage
│   │       ├── Commands
│   │       │   ├── Delete/DeleteCommand.cs
│   │       │   └── Upload/UploadCommand.cs
│   │       └── Queries/DownloadFile/DownloadFileQuery.cs
│   ├── Identity
│   │   ├── Auth/Commands/LoginUser/LoginUserCommand.cs
│   │   ├── Users/Commands/CreateUsers/CreateUserCommand.cs
│   │   └── Users/Queries/GetAll/GetAllQuery.cs
│   └── InternalServices
│       └── Clients
│           ├── Commands
│           │   ├── CreateClient/CreateClientCommand.cs
│           │   ├── UpdateClient/UpdateClientCommand.cs
│           └── Queries/GetAll/GetAllQuery.cs
├── ViewModels                             # Modelos para visualização.
│   ├── Clients/ClientVM.cs
│   ├── Storage/UploadFileVM.cs
│   └── Users/LoginUserVm.cs
└── Template.Application.csproj            # Arquivo de configuração do projeto.
```

#### Descrição dos Principais Componentes

- **Behaviours**: Classes que aplicam comportamentos transversais (e.g., autenticação, validação) aos comandos e consultas, como `LoggingBehaviour` e `ValidationBehaviour`.
- **Interfaces**: Definem contratos para repositórios, segurança e serviços externos, como `IClientRepository`, `ICurrentUser`, `ISendGrid`, e `IStorage`.
- **Domains (Comandos e Consultas)**: Implementações de operações específicas para cada domínio, separadas por comandos (escrita) e consultas (leitura).
- **ViewModels**: Modelos utilizados para transferir dados para a camada de apresentação, como `ClientVM` e `UploadFileVM`.

Essa camada organiza a lógica da aplicação e define como os dados e comportamentos são processados, seguindo os princípios de **CQRS** e garantindo uma clara separação entre leitura e escrita.

### 3. Template.Domain

A camada **Domain** é responsável por encapsular as regras de negócio e a lógica central do sistema, implementando os conceitos de **Domain-Driven Design (DDD)**. Essa camada inclui entidades, validações, interfaces e constantes que são utilizados para gerenciar e validar informações específicas do domínio.

#### Estrutura da Camada Domain

```plaintext
Template.Domain
├── Constants
│   ├── Policies.cs       # Define políticas de acesso para a aplicação.
│   └── Roles.cs          # Define papéis (roles) usados para autorização.
├── Entity
│   ├── Client.cs         # Entidade de Cliente, representa o modelo de dados para um cliente.
│   └── Entity.cs         # Entidade base para o domínio, compartilhada entre outras entidades.
├── Interfaces
│   └── IClient.cs        # Interface para a entidade Client, define os métodos e propriedades necessários.
├── Validations
│   ├── CNPJValidationAttribute.cs  # Validação de CNPJ, usada para validar números de CNPJ.
│   ├── CPFValidationAttribute.cs   # Validação de CPF, usada para validar números de CPF.
│   └── DomainExceptionValidation.cs # Exceções específicas do domínio.
├── StringFormatter.cs    # Classe utilitária para manipulação e formatação de strings.
└── Template.Domain.csproj  # Arquivo de configuração do projeto.
```

#### Detalhes das Principais Classes e Estruturas

- **Client.cs**: Define a entidade `Client`, representando um cliente com propriedades específicas como `ID`, `Nome`, e outras informações relevantes.
- **Entity.cs**: Serve como uma entidade base para o domínio, proporcionando propriedades e métodos comuns a todas as entidades do projeto.
- **IClient.cs**: Interface para a entidade `Client`, assegurando que todos os métodos e propriedades necessários para lidar com clientes sejam implementados.
- **CNPJValidationAttribute.cs** e **CPFValidationAttribute.cs**: Atributos personalizados para validação de números de CNPJ e CPF, garantindo a conformidade dos identificadores brasileiros.
- **DomainExceptionValidation.cs**: Lida com exceções específicas do domínio, fornecendo uma estrutura para capturar e tratar erros de forma consistente.
- **Policies.cs** e **Roles.cs**: Definem as constantes relacionadas a políticas de acesso e papéis, centralizando a configuração de permissões.

Esses componentes são essenciais para manter a lógica do domínio isolada e organizada, de acordo com os princípios de **DDD**, permitindo que outras camadas interajam com o domínio sem depender diretamente de detalhes de implementação.
	
### 3. Template.Infra

A camada **Infra** é responsável pela infraestrutura da aplicação, incluindo a persistência de dados, integração com serviços externos, autenticação e configurações. Esta camada implementa as interações de baixo nível necessárias para suportar a lógica de domínio e a camada de apresentação.

#### Estrutura da Camada Infra

```plaintext
Template.Infra
├── DependencyInjection.cs          # Configurações de injeção de dependência.
├── ExternalServices
│   ├── Google
│   │   ├── DependencyInjection.cs  # Injeção de dependência para o serviço do Google.
│   │   └── Google.cs               # Implementação do serviço da API do Google.
│   ├── SendEmails
│   │   ├── DependencyInjection.cs  # Injeção de dependência para o serviço de envio de e-mails.
│   │   └── SendGrid.cs             # Implementação do serviço de envio de e-mails usando SendGrid.
│   └── Storage
│       ├── AzureStorage.cs         # Serviço para armazenamento na nuvem usando Azure Storage.
│       └── DependencyInjection.cs  # Injeção de dependência para o serviço de armazenamento.
├── Identity
│   ├── ContextUser.cs              # Classe para obter o contexto do usuário.
│   ├── CurrentUser.cs              # Classe para representar o usuário atual.
│   ├── IdentityResultExtensions.cs # Extensões para personalização de resultados de identidade.
│   ├── IdentityService.cs          # Serviço de autenticação e identidade.
│   ├── ITokenService.cs            # Interface para o serviço de tokens.
│   ├── LocalizedIdentityErrorDescriber.cs  # Descritor de erros personalizados para identidade.
│   └── TokenService.cs             # Serviço para criação e validação de tokens JWT.
├── Persistence
│   ├── Contexts
│   │   ├── Context.cs              # Contexto do banco de dados.
│   │   ├── Dapper.cs               # Contexto para utilização do Dapper.
│   │   ├── InicializarContext.cs   # Inicialização do contexto de dados.
│   │   └── Schema.cs               # Definição do esquema do banco de dados.
│   ├── Migrations
│   │   ├── 20241112201936_Initial.cs       # Migração inicial do banco de dados.
│   │   ├── 20241112202126_AddedClient.cs   # Migração para adição de entidade Cliente.
│   │   └── ContextModelSnapshot.cs         # Snapshot do modelo do contexto.
│   └── Repositories
│       ├── Base
│       │   └── Repository.cs        # Repositório base para operações de dados.
│       └── Implementations
│           └── ClientRepository.cs  # Implementação do repositório de clientes.
├── Settings
│   ├── Configurations
│   │   ├── GoogleConfiguration.cs       # Configurações para o serviço da Google.
│   │   ├── IdentityConfiguration.cs     # Configurações de identidade e segurança.
│   │   ├── JwtConfiguration.cs          # Configurações para JWT.
│   │   ├── SendGridConfiguration.cs     # Configurações para o serviço SendGrid.
│   │   └── StorageConfiguration.cs      # Configurações para o serviço de armazenamento.
│   └── Maps
│       ├── ClientMap.cs                # Mapeamento para a entidade Client.
│       └── UserContextMap.cs           # Mapeamento para o contexto de usuários.
└── Template.Infra.csproj               # Arquivo de configuração do projeto.
```

#### Descrição dos Principais Componentes

- **DependencyInjection.cs**: Configura as dependências para serem injetadas no contêiner DI.
- **External Services**: Integrações com serviços externos como envio de e-mails via SendGrid e armazenamento em Azure.
- **Identity**: Fornece autenticação, autorização e serviços de tokens JWT, além de personalizações para o gerenciamento de identidade.
- **Persistence**: Contém o contexto do banco de dados, migrações e repositórios para manipulação de dados.
- **Settings**: Armazena configurações específicas para serviços externos e segurança.

Esses componentes permitem que a camada `Infra` suporte a camada de domínio e a apresentação, mantendo as dependências externas e as configurações isoladas e facilmente gerenciáveis.

## Configuração do Google Authentication

### Passos para Configuração

1. Acesse o [Google Cloud Console](https://console.cloud.google.com/).
2. Crie um novo projeto ou selecione um existente.
3. Ative a **API do Google Calendar** navegando até a opção `APIs e Serviços` > `Biblioteca` e pesquisando por "Google Calendar API".
4. Configure a tela de consentimento OAuth em `APIs e Serviços` > `Tela de consentimento OAuth`.
   - Preencha todas as informações obrigatórias.
   - Adicione escopos necessários, como `openid`, `email`, `profile` e `https://www.googleapis.com/auth/calendar.readonly`.
5. Crie as credenciais OAuth em `APIs e Serviços` > `Credenciais` > `Criar Credenciais` > `ID do cliente OAuth`.
   - Escolha o tipo de aplicativo: **Aplicativo da Web**.
   - Adicione o URI de redirecionamento autorizado (ex.: `https://localhost:7048/auth/google/callback`).
6. Caso esteja em desenvolvimento, adicione seu e-mail na seção de **Testadores autorizados**.

### Colocando em Produção

1. Certifique-se de que todas as informações na tela de consentimento OAuth estão completas e corretas.
2. Solicite a verificação do aplicativo no Google Cloud Console.
   - Após a aprovação, o app poderá ser usado por qualquer usuário.

### Integração no Projeto

No projeto, a autenticação do Google foi configurada seguindo as etapas abaixo:

- A classe `Google` foi implementada em `Template.Infra.ExternalServices.Google` para lidar com o fluxo de autenticação OAuth.
- A função `AuthenticateUserAsync` é responsável por trocar o código de autorização por tokens e salvar o token de acesso no banco.
- A URL de autenticação é gerada pela função `GenerateAuthenticationUrl`.

### Recuperação de Eventos do Google Calendar

Com o token de acesso salvo, você pode recuperar os eventos do Google Calendar do usuário autenticado.

#### Passos:
1. Utilize a função `GetGoogleCalendarEventsAsync` na classe `Google`.
2. Esta função utiliza o token armazenado para acessar a API do Google Calendar e retorna os eventos.

Exemplo de URL da API usada:
```
https://www.googleapis.com/calendar/v3/calendars/primary/events
```

## Telas Necessárias no Google Cloud Console

- **Tela de consentimento OAuth**: Configure os escopos necessários e finalize a configuração para publicar o app.
- **Biblioteca de APIs**: Certifique-se de ativar a **API do Google Calendar**.
- **Credenciais**: Gere o ID do cliente OAuth e configure os URIs de redirecionamento.

Para mais detalhes, consulte a [documentação oficial do Google](https://developers.google.com/identity).

---

Caso precise de mais ajuda, abra uma issue no repositório!

## Padrões e Tecnologias Utilizadas

Este projeto foi desenvolvido com o uso de tecnologias e padrões modernos, como:

- **ASP.NET Core**: Framework principal para desenvolvimento da API RESTful.
- **Entity Framework Core**: ORM para manipulação de dados, facilitando a interação com o banco de dados.
- **FluentValidation**: Biblioteca de validação de dados, garantindo integridade nas entradas de dados.
- **Injeção de Dependência**: Implementada com o contêiner nativo do ASP.NET Core para facilitar o desacoplamento de componentes.
- **Swagger**: Documentação interativa da API; o projeto gera o arquivo `Template.Api.xml` automaticamente, que é integrado ao Swagger para exibir descrições detalhadas de métodos, parâmetros e respostas.

### Observação

Neste projeto, optou-se por não utilizar **MediatR** e **AutoMapper**, apesar de serem recomendados em boas práticas de desenvolvimento. Essa decisão reflete uma escolha intencional do design da aplicação para manter controle direto sobre os fluxos de dados e as operações, conforme preferências e necessidades específicas.

## Executando o Projeto

1. **Clone o repositório** e navegue até o diretório principal:
   ```bash
   git clone <url-do-repositorio>
   cd Template.Project
   ```
2. **Configuração**: Edite o arquivo `appsettings.json` para ajustar as strings de conexão e configurações do ambiente.
3. **Aplicação de Migrations**: Execute o seguinte comando para aplicar as migrações do Entity Framework e criar as tabelas no banco de dados:
   ```bash
   dotnet ef database update
   ```
4. **Inicie a aplicação**:
   ```bash
   dotnet run --project Template.Api
   ```

## Contribuindo

Para contribuir com o projeto, siga os passos abaixo:

1. Crie uma branch para a sua feature ou correção.
2. Envie um pull request com uma descrição detalhada das suas alterações, incluindo o objetivo e o impacto.

Este projeto ainda possui espaço para melhorias adicionais, mas várias boas práticas foram aplicadas para garantir uma base sólida e escalável. O objetivo é servir como um modelo para arquiteturas limpas e de fácil manutenção. Contribuições são bem-vindas para aprimorar ainda mais a funcionalidade e estrutura.