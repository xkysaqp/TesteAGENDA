# 📅 AgendaApp - Sistema de Agendamento Inteligente

Sistema completo de agendamento para empresas e profissionais autônomos. Organize horários, gerencie clientes e otimize sua agenda com ferramentas poderosas e intuitivas.

## 🚀 Funcionalidades

### ✅ Cadastro de Prestadores
- Criar novo prestador
- Vincular atividades/serviços (específicos e globais)
- Definir horários de atendimento
- Fluxo único de cadastro com todas as funcionalidades integradas

### ✅ Agendamento pelo Cliente
- Link público com slug da loja (`/agendamento/{lojaSlug}`)
- Link público com ID da loja (`/agendamento/loja/{lojaId}`)
- Visualização de horários disponíveis
- Visualização de funcionários disponíveis
- Agendamento independente pelo cliente

### ✅ Sistema Administrativo
- Controle completo de agendamentos
- Confirmação, cancelamento e conclusão
- Filtros e estatísticas
- Gestão de lojas, prestadores e serviços

## 🛠️ Tecnologias Utilizadas

- **Backend**: ASP.NET Core 8.0
- **Frontend**: HTML5, CSS3, JavaScript, Bootstrap 5
- **Banco de Dados**: PostgreSQL
- **ORM**: Entity Framework Core
- **Autenticação**: ASP.NET Core Identity
- **Mapeamento**: AutoMapper
- **Arquitetura**: Clean Architecture (DDD)

## 📋 Pré-requisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/) (versão 12 ou superior)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [Visual Studio Code](https://code.visualstudio.com/)

## 🔧 Instalação e Configuração

### 1. Clone o repositório
```bash
git clone https://github.com/seu-usuario/agendaapp.git
cd agendaapp
```

### 2. Configure o banco de dados

#### Opção A: Usando Docker (Recomendado)
```bash
# Instale o Docker Desktop primeiro
docker run --name agendaapp-postgres -e POSTGRES_PASSWORD=sua_senha -e POSTGRES_DB=AgendaApp -p 5432:5432 -d postgres:15
```

#### Opção B: Instalação local
1. Instale o PostgreSQL
2. Crie um banco de dados chamado `AgendaApp`
3. Anote as credenciais de acesso

### 3. Configure a string de conexão

Edite o arquivo `app/src/AgendaApp.Web/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=AgendaApp;Username=postgres;Password=sua_senha"
  }
}
```

### 4. Instale as dependências
```bash
cd app/src/AgendaApp.Web
dotnet restore
```

### 5. Execute as migrações
```bash
# Instale a ferramenta do Entity Framework (se necessário)
dotnet tool install --global dotnet-ef

# Execute as migrações
dotnet ef database update
```

### 6. Execute o projeto
```bash
dotnet run
```

## 🌐 Acessando o Sistema

### Landing Page
```
http://localhost:5000
```

### Sistema Administrativo
```
http://localhost:5000/app
```

### Agendamento Público (exemplo)
```
http://localhost:5000/agendamento/minha-loja
http://localhost:5000/agendamento/loja/{ID-DA-LOJA}
```

## 📁 Estrutura do Projeto

```
app/src/
├── AgendaApp.Aplicacao/          # Camada de Aplicação
│   ├── DTOs/                     # Data Transfer Objects
│   ├── Interfaces/               # Interfaces dos serviços
│   └── Services/                 # Implementação dos serviços
├── AgendaApp.Dominio/            # Camada de Domínio
│   ├── Entities/                 # Entidades do domínio
│   ├── Interfaces/               # Interfaces dos repositórios
│   ├── Enums/                    # Enumerações
│   └── ValueObjects/             # Objetos de valor
├── AgendaApp.Infraestrutura/     # Camada de Infraestrutura
│   ├── Data/                     # Contexto do EF e configurações
│   ├── Repositories/             # Implementação dos repositórios
│   ├── Identity/                 # Configuração de autenticação
│   └── Migrations/               # Migrações do banco
└── AgendaApp.Web/                # Camada de Apresentação
    ├── Controllers/              # Controladores MVC
    ├── Views/                    # Views Razor
    ├── ViewModels/               # ViewModels
    └── wwwroot/                  # Arquivos estáticos
```

## 🔐 Configuração de Autenticação

O sistema utiliza ASP.NET Core Identity para autenticação. Para criar um usuário administrador:

1. Acesse o sistema
2. Clique em "Registrar"
3. Crie uma conta
4. Use o sistema normalmente

## 📱 Funcionalidades Principais

### Para Administradores
- **Gestão de Lojas**: Cadastre e gerencie suas lojas
- **Cadastro de Prestadores**: Crie prestadores com atividades e horários
- **Gestão de Serviços**: Configure serviços globais e específicos
- **Controle de Agendamentos**: Visualize, confirme e gerencie agendamentos
- **Relatórios**: Acompanhe estatísticas e desempenho

### Para Clientes
- **Agendamento Público**: Acesse via link personalizado
- **Seleção de Serviços**: Escolha entre serviços disponíveis
- **Escolha de Prestadores**: Veja profissionais disponíveis
- **Agendamento Independente**: Faça seu agendamento sem intermediação

## 🚀 Deploy

### Deploy Local
```bash
dotnet publish -c Release -o ./publish
```

### Deploy em Produção
1. Configure as variáveis de ambiente
2. Use um servidor web (IIS, Nginx, Apache)
3. Configure SSL/HTTPS
4. Configure backup do banco de dados

## 🧪 Testes

```bash
# Execute os testes unitários
dotnet test
```

## 📝 API Endpoints

### Agendamento Público
- `GET /agendamento/{lojaSlug}` - Página de agendamento público
- `GET /agendamento/loja/{lojaId}` - Página de agendamento por ID
- `GET /agendamento/{lojaSlug}/horarios` - Obter horários disponíveis
- `POST /agendamento/{lojaSlug}/criar` - Criar agendamento

### Administrativo
- `GET /app` - Painel administrativo
- `GET /agendamento` - Lista de agendamentos
- `POST /agendamento/confirmar/{id}` - Confirmar agendamento
- `POST /agendamento/cancelar/{id}` - Cancelar agendamento

## 🤝 Contribuindo

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## 📞 Suporte

- **Email**: contato@agendaapp.com
- **Telefone**: (11) 99999-9999
- **Documentação**: [Wiki do Projeto](https://github.com/seu-usuario/agendaapp/wiki)

## 🗺️ Roadmap

- [ ] Sistema de notificações por email/SMS
- [ ] Integração com Google Calendar
- [ ] App mobile (React Native)
- [ ] Sistema de pagamentos
- [ ] Relatórios avançados
- [ ] API pública para integrações

## 🙏 Agradecimentos

- ASP.NET Core Team
- Entity Framework Team
- Bootstrap Team
- Comunidade .NET

---

**Desenvolvido com ❤️ usando ASP.NET Core**