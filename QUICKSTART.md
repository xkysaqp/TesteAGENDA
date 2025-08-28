# 🚀 Guia de Início Rápido - AgendaApp

Este guia te ajudará a configurar e executar o AgendaApp em menos de 5 minutos!

## ⚡ Setup Rápido

### 1. Pré-requisitos
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (opcional)

### 2. Clone e Configure
```bash
git clone https://github.com/seu-usuario/agendaapp.git
cd agendaapp
```

### 3. Banco de Dados (Escolha uma opção)

#### Opção A: Docker (Recomendado)
```bash
docker-compose up -d postgres
```

#### Opção B: PostgreSQL Local
1. Instale o PostgreSQL
2. Crie um banco chamado `AgendaApp`

### 4. Configure a Conexão
Edite `app/src/AgendaApp.Web/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=AgendaApp;Username=postgres;Password=sua_senha_aqui"
  }
}
```

### 5. Execute o Projeto
```bash
cd app/src/AgendaApp.Web
dotnet restore
dotnet ef database update
dotnet run
```

### 6. Acesse o Sistema
- **Landing Page**: http://localhost:5000
- **Sistema**: http://localhost:5000/app

## 🎯 Primeiros Passos

1. **Crie uma conta** no sistema
2. **Cadastre uma loja**
3. **Adicione prestadores** com seus serviços
4. **Configure horários** de atendimento
5. **Teste o agendamento público** usando o link da loja

## 🔧 Comandos Úteis

```bash
# Executar testes
dotnet test

# Publicar para produção
dotnet publish -c Release

# Ver logs
dotnet run --environment Development

# Parar banco Docker
docker-compose down
```

## 🆘 Solução de Problemas

### Erro de Conexão com Banco
- Verifique se o PostgreSQL está rodando
- Confirme a string de conexão
- Teste: `dotnet ef database update`

### Erro de Compilação
- Execute: `dotnet clean && dotnet restore`
- Verifique se o .NET 8.0 está instalado

### Porta em Uso
- Mude a porta no `launchSettings.json`
- Ou pare outros serviços na porta 5000

## 📞 Suporte

- **Issues**: [GitHub Issues](https://github.com/seu-usuario/agendaapp/issues)
- **Email**: contato@agendaapp.com

---

**🎉 Pronto! Seu sistema de agendamento está funcionando!**
