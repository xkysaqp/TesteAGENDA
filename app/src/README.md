# AgendaApp - Sistema de Gerenciamento de Agenda

## 📋 Visão Geral
Sistema de gerenciamento de agenda desenvolvido com .NET 8 e arquitetura Domain-Driven Design (DDD), seguindo as melhores práticas de desenvolvimento e segurança.

## 🏗️ Arquitetura DDD

### Estrutura de Projetos

```
src/
├── AgendaApp.Dominio/              # 🎯 Camada de Domínio
│   ├── Entities/                   # Entidades do negócio
│   │   ├── BaseEntity.cs          # Entidade base com propriedades comuns
│   │   ├── Prestador.cs           # Entidade Prestador de serviços
│   │   ├── Servico.cs             # Entidade Serviço oferecido
│   │   └── HorarioDisponivel.cs   # Entidade Horário disponível
│   ├── ValueObjects/               # Objetos de valor
│   │   ├── Email.cs               # Value Object para e-mail
│   │   └── Cnpj.cs                # Value Object para CNPJ
│   ├── Enums/                      # Enumerações
│   │   └── DiaSemana.cs           # Enum para dias da semana
│   └── Interfaces/                 # Contratos do domínio
│       ├── IRepository.cs         # Interface base para repositórios
│       ├── IPrestadorRepository.cs # Interface específica do Prestador
│       ├── IServicoRepository.cs  # Interface específica do Serviço
│       └── IHorarioDisponivelRepository.cs # Interface específica do HorarioDisponivel
│
├── AgendaApp.Aplicacao/           # 🔧 Camada de Aplicação
│   └── DependencyInjection.cs    # Configuração de DI da aplicação
│
├── AgendaApp.Infraestrutura/      # 💾 Camada de Infraestrutura
│   ├── Data/
│   │   └── AgendaAppDbContext.cs  # Contexto do Entity Framework
│   └── DependencyInjection.cs    # Configuração de DI da infraestrutura
│
└── AgendaApp.Web/                 # 🌐 Camada de Apresentação (MVC)
    ├── Controllers/
    │   └── HomeController.cs      # Controller principal
    ├── Views/                     # Views do ASP.NET Core MVC
    ├── wwwroot/                   # Arquivos estáticos
    ├── appsettings.json          # Configurações da aplicação
    └── Program.cs                # Ponto de entrada da aplicação
```

## ✅ Sprint 1 - Infraestrutura Implementada

### 🎯 Tarefas Concluídas

- [x] **Solução Visual Studio com múltiplos projetos (DDD)**
  - AgendaApp.Dominio - Entidades e regras de negócio
  - AgendaApp.Aplicacao - Serviços de aplicação
  - AgendaApp.Infraestrutura - Acesso a dados e infraestrutura
  - AgendaApp.Web - Interface MVC

- [x] **Entity Framework Core com PostgreSQL**
  - Configuração do DbContext principal
  - Configuração de conexão com PostgreSQL
  - Configurações de retry e logging

- [x] **Injeção de Dependência**
  - Microsoft.Extensions.DependencyInjection configurado
  - Classes de DI para cada camada
  - Registro automático de serviços

- [x] **Configuração de Aplicação**
  - appsettings.json com connection string
  - appsettings.Development.json para desenvolvimento
  - Configuração de logs detalhados

- [x] **DbContext Inicial**
  - AgendaAppDbContext configurado para PostgreSQL
  - Configurações de schema e migrações

### 🎯 Tarefas Sprint 2 - Domínio

- [x] **Entidades do Domínio**
  - Prestador: Nome, CNPJ (opcional), área de atuação, e-mail, telefone
  - Servico: Nome, descrição, valor, duração, relacionamento com Prestador
  - HorarioDisponivel: Dia da semana, hora início/fim, relacionamento com Prestador

- [x] **Value Objects**
  - Email: Validação de formato de e-mail
  - CNPJ: Validação completa incluindo dígito verificador

- [x] **Interfaces de Repositório**
  - IPrestadorRepository: Métodos específicos para Prestador
  - IServicoRepository: Métodos específicos para Serviço  
  - IHorarioDisponivelRepository: Métodos específicos para Horário

## 🛠️ Tecnologias Utilizadas

- **.NET 8** - Framework principal
- **ASP.NET Core MVC** - Interface web
- **Entity Framework Core 8.0** - ORM
- **PostgreSQL** (Npgsql) - Banco de dados
- **Microsoft.Extensions.DependencyInjection** - Injeção de dependência
- **Swagger/OpenAPI** - Documentação de API
- **Bootstrap** - Framework CSS

## 🔒 Práticas de Segurança Implementadas

### Backend (.NET)
- ✅ Configuração para uso de Stored Procedures (prevenção SQL Injection)
- ✅ HTTPS obrigatório configurado
- ✅ Validação de inputs preparada
- ✅ Logs estruturados para auditoria
- ✅ Connection string segura

### Preparado para Frontend
- 🔄 Estrutura pronta para prevenção XSS
- 🔄 Configuração CSRF preparada
- 🔄 Cookies seguros configurados

## 🚀 Como Executar

### Pré-requisitos
- .NET 8 SDK
- PostgreSQL
- Visual Studio 2022 ou VS Code

### Configuração do Banco
1. Certifique-se de que o PostgreSQL esteja rodando
2. Atualize a connection string em `appsettings.json` se necessário
3. Execute as migrações (quando criadas)

### Execução
```bash
cd src
dotnet build
cd AgendaApp.Web
dotnet run
```

A aplicação estará disponível em `https://localhost:5001`

## 📝 Próximos Passos (Sprint 3)

- [ ] Implementar repositórios concretos na camada de Infraestrutura
- [ ] Configurar mapeamento do Entity Framework para as entidades
- [ ] Criar migrações do banco de dados
- [ ] Implementar services da aplicação (CRUD)
- [ ] Criar controllers e views funcionais para interface web
- [ ] Implementar validações e tratamento de erros

## 🧪 Testes
Estrutura preparada para implementação de:
- Testes unitários
- Testes de integração
- Testes de API

## 📚 Documentação Adicional
- [Documentação do .NET 8](https://docs.microsoft.com/en-us/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [ASP.NET Core MVC](https://docs.microsoft.com/en-us/aspnet/core/mvc/)
- [Domain-Driven Design](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/)

---
**Desenvolvido seguindo as melhores práticas de DDD e segurança em .NET 8** 