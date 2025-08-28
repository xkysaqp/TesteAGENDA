using AgendaApp.Dominio.Entities;
using AgendaApp.Dominio.ValueObjects;
using FluentAssertions;
using Xunit;

namespace AgendaApp.Tests.Dominio.Entities;

public class PrestadorTests
{
    [Fact]
    public void Criar_ComDadosValidos_DeveCriarPrestadorComSucesso()
    {
        // Arrange
        var nome = "João Silva";
        var areaAtuacao = "Corte de Cabelo";
        var email = "joao@exemplo.com";
        var telefone = "(11) 99999-9999";
        var lojaId = Guid.NewGuid();
        var cnpj = "12.345.678/0001-90";

        // Act
        var prestador = new Prestador(nome, areaAtuacao, email, telefone, lojaId, cnpj);

        // Assert
        prestador.Should().NotBeNull();
        prestador.Nome.Should().Be(nome);
        prestador.AreaAtuacao.Should().Be(areaAtuacao);
        prestador.Email.Endereco.Should().Be(email);
        prestador.Telefone.Should().Be(telefone);
        prestador.LojaId.Should().Be(lojaId);
        prestador.CNPJ.Should().NotBeNull();
        prestador.CNPJ!.Numero.Should().Be("12345678000190");
        prestador.Ativo.Should().BeTrue();
        prestador.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Criar_SemCNPJ_DeveCriarPrestadorComSucesso()
    {
        // Arrange
        var nome = "Maria Santos";
        var areaAtuacao = "Manicure";
        var email = "maria@exemplo.com";
        var telefone = "(11) 88888-8888";
        var lojaId = Guid.NewGuid();

        // Act
        var prestador = new Prestador(nome, areaAtuacao, email, telefone, lojaId);

        // Assert
        prestador.Should().NotBeNull();
        prestador.Nome.Should().Be(nome);
        prestador.CNPJ.Should().BeNull();
        prestador.Ativo.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Criar_ComNomeInvalido_DeveLancarExcecao(string nome)
    {
        // Arrange
        var areaAtuacao = "Corte de Cabelo";
        var email = "teste@exemplo.com";
        var telefone = "(11) 99999-9999";
        var lojaId = Guid.NewGuid();

        // Act & Assert
        var action = () => new Prestador(nome!, areaAtuacao, email, telefone, lojaId);
        action.Should().Throw<ArgumentException>().WithMessage("*Nome é obrigatório*");
    }

    [Theory]
    [InlineData("A")]
    [InlineData("AB")]
    public void Criar_ComNomeMuitoCurto_DeveLancarExcecao(string nome)
    {
        // Arrange
        var areaAtuacao = "Corte de Cabelo";
        var email = "teste@exemplo.com";
        var telefone = "(11) 99999-9999";
        var lojaId = Guid.NewGuid();

        // Act & Assert
        var action = () => new Prestador(nome, areaAtuacao, email, telefone, lojaId);
        action.Should().Throw<ArgumentException>().WithMessage("*Nome deve ter pelo menos 2 caracteres*");
    }

    [Fact]
    public void Criar_ComNomeMuitoLongo_DeveLancarExcecao()
    {
        // Arrange
        var nome = new string('A', 201); // 201 caracteres
        var areaAtuacao = "Corte de Cabelo";
        var email = "teste@exemplo.com";
        var telefone = "(11) 99999-9999";
        var lojaId = Guid.NewGuid();

        // Act & Assert
        var action = () => new Prestador(nome, areaAtuacao, email, telefone, lojaId);
        action.Should().Throw<ArgumentException>().WithMessage("*Nome deve ter no máximo 200 caracteres*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Criar_ComAreaAtuacaoInvalida_DeveLancarExcecao(string areaAtuacao)
    {
        // Arrange
        var nome = "João Silva";
        var email = "teste@exemplo.com";
        var telefone = "(11) 99999-9999";
        var lojaId = Guid.NewGuid();

        // Act & Assert
        var action = () => new Prestador(nome, areaAtuacao!, email, telefone, lojaId);
        action.Should().Throw<ArgumentException>().WithMessage("*Área de atuação é obrigatória*");
    }

    [Fact]
    public void Criar_ComAreaAtuacaoMuitoLonga_DeveLancarExcecao()
    {
        // Arrange
        var nome = "João Silva";
        var areaAtuacao = new string('A', 101); // 101 caracteres
        var email = "teste@exemplo.com";
        var telefone = "(11) 99999-9999";
        var lojaId = Guid.NewGuid();

        // Act & Assert
        var action = () => new Prestador(nome, areaAtuacao, email, telefone, lojaId);
        action.Should().Throw<ArgumentException>().WithMessage("*Área de atuação deve ter no máximo 100 caracteres*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Criar_ComTelefoneInvalido_DeveLancarExcecao(string telefone)
    {
        // Arrange
        var nome = "João Silva";
        var areaAtuacao = "Corte de Cabelo";
        var email = "teste@exemplo.com";
        var lojaId = Guid.NewGuid();

        // Act & Assert
        var action = () => new Prestador(nome, areaAtuacao, email, telefone!, lojaId);
        action.Should().Throw<ArgumentException>().WithMessage("*Telefone é obrigatório*");
    }

    [Theory]
    [InlineData("123456789")] // 9 dígitos
    [InlineData("123456789012")] // 12 dígitos
    public void Criar_ComTelefoneComDadosIncorretos_DeveLancarExcecao(string telefone)
    {
        // Arrange
        var nome = "João Silva";
        var areaAtuacao = "Corte de Cabelo";
        var email = "teste@exemplo.com";
        var lojaId = Guid.NewGuid();

        // Act & Assert
        var action = () => new Prestador(nome, areaAtuacao, email, telefone, lojaId);
        action.Should().Throw<ArgumentException>().WithMessage("*Telefone deve ter 10 ou 11 dígitos*");
    }

    [Fact]
    public void Criar_ComLojaIdVazio_DeveLancarExcecao()
    {
        // Arrange
        var nome = "João Silva";
        var areaAtuacao = "Corte de Cabelo";
        var email = "teste@exemplo.com";
        var telefone = "(11) 99999-9999";
        var lojaId = Guid.Empty;

        // Act & Assert
        var action = () => new Prestador(nome, areaAtuacao, email, telefone, lojaId);
        action.Should().Throw<ArgumentException>().WithMessage("*ID da loja é obrigatório*");
    }

    [Fact]
    public void AtualizarInformacoes_ComDadosValidos_DeveAtualizarComSucesso()
    {
        // Arrange
        var prestador = new Prestador("João Silva", "Corte", "joao@exemplo.com", "(11) 99999-9999", Guid.NewGuid());
        var novoNome = "João Silva Santos";
        var novaAreaAtuacao = "Corte e Barba";
        var novoTelefone = "(11) 88888-8888";

        // Act
        prestador.AtualizarInformacoes(novoNome, novaAreaAtuacao, novoTelefone);

        // Assert
        prestador.Nome.Should().Be(novoNome);
        prestador.AreaAtuacao.Should().Be(novaAreaAtuacao);
        prestador.Telefone.Should().Be(novoTelefone);
        prestador.DataAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void AtualizarEmail_ComEmailValido_DeveAtualizarComSucesso()
    {
        // Arrange
        var prestador = new Prestador("João Silva", "Corte", "joao@exemplo.com", "(11) 99999-9999", Guid.NewGuid());
        var novoEmail = "joao.silva@exemplo.com";

        // Act
        prestador.AtualizarEmail(novoEmail);

        // Assert
        prestador.Email.Endereco.Should().Be(novoEmail);
        prestador.DataAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void DefinirCNPJ_ComCNPJValido_DeveDefinirComSucesso()
    {
        // Arrange
        var prestador = new Prestador("João Silva", "Corte", "joao@exemplo.com", "(11) 99999-9999", Guid.NewGuid());
        var cnpj = "12.345.678/0001-90";

        // Act
        prestador.DefinirCNPJ(cnpj);

        // Assert
        prestador.CNPJ.Should().NotBeNull();
        prestador.CNPJ!.Numero.Should().Be("12345678000190");
        prestador.DataAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void RemoverCNPJ_DeveRemoverCNPJComSucesso()
    {
        // Arrange
        var prestador = new Prestador("João Silva", "Corte", "joao@exemplo.com", "(11) 99999-9999", Guid.NewGuid(), "12.345.678/0001-90");

        // Act
        prestador.RemoverCNPJ();

        // Assert
        prestador.CNPJ.Should().BeNull();
        prestador.DataAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Ativar_DeveAtivarPrestador()
    {
        // Arrange
        var prestador = new Prestador("João Silva", "Corte", "joao@exemplo.com", "(11) 99999-9999", Guid.NewGuid());
        prestador.Desativar();

        // Act
        prestador.Ativar();

        // Assert
        prestador.Ativo.Should().BeTrue();
    }

    [Fact]
    public void Desativar_DeveDesativarPrestador()
    {
        // Arrange
        var prestador = new Prestador("João Silva", "Corte", "joao@exemplo.com", "(11) 99999-9999", Guid.NewGuid());

        // Act
        prestador.Desativar();

        // Assert
        prestador.Ativo.Should().BeFalse();
    }

    [Fact]
    public void AdicionarServico_ComServicoValido_DeveAdicionarComSucesso()
    {
        // Arrange
        var prestador = new Prestador("João Silva", "Corte", "joao@exemplo.com", "(11) 99999-9999", Guid.NewGuid());
        var servico = new Servico("Corte Masculino", "Corte tradicional masculino", 50.00m, 30, Guid.NewGuid());

        // Act
        prestador.AdicionarServico(servico);

        // Assert
        prestador.Servicos.Should().HaveCount(1);
        prestador.Servicos.First().Should().Be(servico);
        prestador.DataAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void AdicionarServico_ComServicoNulo_DeveLancarExcecao()
    {
        // Arrange
        var prestador = new Prestador("João Silva", "Corte", "joao@exemplo.com", "(11) 99999-9999", Guid.NewGuid());

        // Act & Assert
        var action = () => prestador.AdicionarServico(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AdicionarServico_ComServicoDuplicado_DeveLancarExcecao()
    {
        // Arrange
        var prestador = new Prestador("João Silva", "Corte", "joao@exemplo.com", "(11) 99999-9999", Guid.NewGuid());
        var servico = new Servico("Corte Masculino", "Corte tradicional masculino", 50.00m, 30, Guid.NewGuid());
        prestador.AdicionarServico(servico);

        // Act & Assert
        var action = () => prestador.AdicionarServico(servico);
        action.Should().Throw<InvalidOperationException>().WithMessage("*Já existe um serviço com este nome*");
    }

    [Fact]
    public void RemoverServico_ComServicoExistente_DeveRemoverComSucesso()
    {
        // Arrange
        var prestador = new Prestador("João Silva", "Corte", "joao@exemplo.com", "(11) 99999-9999", Guid.NewGuid());
        var servico = new Servico("Corte Masculino", "Corte tradicional masculino", 50.00m, 30, Guid.NewGuid());
        prestador.AdicionarServico(servico);

        // Act
        prestador.RemoverServico(servico.Id);

        // Assert
        prestador.Servicos.Should().BeEmpty();
        prestador.DataAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void RemoverServico_ComServicoInexistente_NaoDeveFazerNada()
    {
        // Arrange
        var prestador = new Prestador("João Silva", "Corte", "joao@exemplo.com", "(11) 99999-9999", Guid.NewGuid());
        var servicoId = Guid.NewGuid();

        // Act
        prestador.RemoverServico(servicoId);

        // Assert
        prestador.Servicos.Should().BeEmpty();
    }
}
