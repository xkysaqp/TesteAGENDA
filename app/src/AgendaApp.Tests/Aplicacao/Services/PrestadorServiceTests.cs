using AgendaApp.Aplicacao.DTOs;
using AgendaApp.Aplicacao.Services;
using AgendaApp.Dominio.Entities;
using AgendaApp.Dominio.Interfaces;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AgendaApp.Tests.Aplicacao.Services;

public class PrestadorServiceTests
{
    private readonly Mock<IPrestadorRepository> _mockRepository;
    private readonly Mock<ILogger<PrestadorService>> _mockLogger;
    private readonly PrestadorService _service;
    private readonly Fixture _fixture;

    public PrestadorServiceTests()
    {
        _mockRepository = new Mock<IPrestadorRepository>();
        _mockLogger = new Mock<ILogger<PrestadorService>>();
        _service = new PrestadorService(_mockRepository.Object, null!, _mockLogger.Object);
        _fixture = new Fixture();
        _fixture.Customize(new AutoMoqCustomization());
    }

    [Fact]
    public async Task CriarAsync_ComDadosValidos_DeveCriarPrestadorComSucesso()
    {
        // Arrange
        var dto = new CriarPrestadorDto
        {
            Nome = "João Silva",
            AreaAtuacao = "Corte de Cabelo",
            Email = "joao@exemplo.com",
            Telefone = "(11) 99999-9999",
            LojaId = Guid.NewGuid(),
            CNPJ = "12.345.678/0001-90"
        };

        var prestador = new Prestador(dto.Nome, dto.AreaAtuacao, dto.Email, dto.Telefone, dto.LojaId, dto.CNPJ);
        
        _mockRepository.Setup(r => r.EmailJaExisteAsync(dto.Email, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockRepository.Setup(r => r.CnpjJaExisteAsync(dto.CNPJ, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockRepository.Setup(r => r.AdicionarAsync(It.IsAny<Prestador>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(prestador);
        _mockRepository.Setup(r => r.SalvarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var resultado = await _service.CriarAsync(dto, "usuario@teste.com");

        // Assert
        resultado.Should().NotBeNull();
        resultado.Nome.Should().Be(dto.Nome);
        resultado.Email.Should().Be(dto.Email);
        resultado.CNPJ.Should().Be("12345678000190");
        
        _mockRepository.Verify(r => r.AdicionarAsync(It.IsAny<Prestador>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.SalvarAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_ComEmailDuplicado_DeveLancarExcecao()
    {
        // Arrange
        var dto = new CriarPrestadorDto
        {
            Nome = "João Silva",
            AreaAtuacao = "Corte de Cabelo",
            Email = "joao@exemplo.com",
            Telefone = "(11) 99999-9999",
            LojaId = Guid.NewGuid()
        };

        _mockRepository.Setup(r => r.EmailJaExisteAsync(dto.Email, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        var action = () => _service.CriarAsync(dto, "usuario@teste.com");
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Já existe um prestador com o e-mail*");
    }

    [Fact]
    public async Task CriarAsync_ComCNPJDuplicado_DeveLancarExcecao()
    {
        // Arrange
        var dto = new CriarPrestadorDto
        {
            Nome = "João Silva",
            AreaAtuacao = "Corte de Cabelo",
            Email = "joao@exemplo.com",
            Telefone = "(11) 99999-9999",
            LojaId = Guid.NewGuid(),
            CNPJ = "12.345.678/0001-90"
        };

        _mockRepository.Setup(r => r.EmailJaExisteAsync(dto.Email, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockRepository.Setup(r => r.CnpjJaExisteAsync(dto.CNPJ, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        var action = () => _service.CriarAsync(dto, "usuario@teste.com");
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Já existe um prestador com o CNPJ*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task CriarAsync_ComNomeInvalido_DeveLancarExcecao(string nome)
    {
        // Arrange
        var dto = new CriarPrestadorDto
        {
            Nome = nome,
            AreaAtuacao = "Corte de Cabelo",
            Email = "joao@exemplo.com",
            Telefone = "(11) 99999-9999",
            LojaId = Guid.NewGuid()
        };

        // Act & Assert
        var action = () => _service.CriarAsync(dto, "usuario@teste.com");
        await action.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Nome é obrigatório*");
    }

    [Fact]
    public async Task AtualizarAsync_ComDadosValidos_DeveAtualizarComSucesso()
    {
        // Arrange
        var prestadorId = Guid.NewGuid();
        var dto = new AtualizarPrestadorDto
        {
            Id = prestadorId,
            Nome = "João Silva Santos",
            AreaAtuacao = "Corte e Barba",
            Email = "joao.silva@exemplo.com",
            Telefone = "(11) 88888-8888",
            Ativo = true
        };

        var prestadorExistente = new Prestador("João Silva", "Corte", "joao@exemplo.com", "(11) 99999-9999", Guid.NewGuid());
        var prestadorAtualizado = new Prestador(dto.Nome, dto.AreaAtuacao, dto.Email, dto.Telefone, Guid.NewGuid());

        _mockRepository.Setup(r => r.ObterPorIdAsync(prestadorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(prestadorExistente);
        _mockRepository.Setup(r => r.EmailJaExisteAsync(dto.Email, prestadorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockRepository.Setup(r => r.AtualizarAsync(It.IsAny<Prestador>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(prestadorAtualizado);
        _mockRepository.Setup(r => r.SalvarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var resultado = await _service.AtualizarAsync(dto, "usuario@teste.com");

        // Assert
        resultado.Should().NotBeNull();
        resultado.Nome.Should().Be(dto.Nome);
        resultado.Email.Should().Be(dto.Email);
        
        _mockRepository.Verify(r => r.AtualizarAsync(It.IsAny<Prestador>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.SalvarAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_ComPrestadorInexistente_DeveLancarExcecao()
    {
        // Arrange
        var prestadorId = Guid.NewGuid();
        var dto = new AtualizarPrestadorDto
        {
            Id = prestadorId,
            Nome = "João Silva",
            AreaAtuacao = "Corte",
            Email = "joao@exemplo.com",
            Telefone = "(11) 99999-9999",
            Ativo = true
        };

        _mockRepository.Setup(r => r.ObterPorIdAsync(prestadorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Prestador?)null);

        // Act & Assert
        var action = () => _service.AtualizarAsync(dto, "usuario@teste.com");
        await action.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Prestador com ID*");
    }

    [Fact]
    public async Task ObterPorIdAsync_ComIdValido_DeveRetornarPrestador()
    {
        // Arrange
        var prestadorId = Guid.NewGuid();
        var prestador = new Prestador("João Silva", "Corte", "joao@exemplo.com", "(11) 99999-9999", Guid.NewGuid());

        _mockRepository.Setup(r => r.ObterCompletoAsync(prestadorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(prestador);

        // Act
        var resultado = await _service.ObterPorIdAsync(prestadorId);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Nome.Should().Be("João Silva");
        resultado.Email.Should().Be("joao@exemplo.com");
    }

    [Fact]
    public async Task ObterPorIdAsync_ComIdInexistente_DeveRetornarNull()
    {
        // Arrange
        var prestadorId = Guid.NewGuid();

        _mockRepository.Setup(r => r.ObterCompletoAsync(prestadorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Prestador?)null);

        // Act
        var resultado = await _service.ObterPorIdAsync(prestadorId);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task DesativarAsync_ComPrestadorExistente_DeveDesativarComSucesso()
    {
        // Arrange
        var prestadorId = Guid.NewGuid();
        var prestador = new Prestador("João Silva", "Corte", "joao@exemplo.com", "(11) 99999-9999", Guid.NewGuid());

        _mockRepository.Setup(r => r.ObterPorIdAsync(prestadorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(prestador);
        _mockRepository.Setup(r => r.AtualizarAsync(It.IsAny<Prestador>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(prestador);
        _mockRepository.Setup(r => r.SalvarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var resultado = await _service.DesativarAsync(prestadorId, "usuario@teste.com");

        // Assert
        resultado.Should().BeTrue();
        prestador.Ativo.Should().BeFalse();
        
        _mockRepository.Verify(r => r.AtualizarAsync(It.IsAny<Prestador>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.SalvarAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DesativarAsync_ComPrestadorInexistente_DeveRetornarFalse()
    {
        // Arrange
        var prestadorId = Guid.NewGuid();

        _mockRepository.Setup(r => r.ObterPorIdAsync(prestadorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Prestador?)null);

        // Act
        var resultado = await _service.DesativarAsync(prestadorId, "usuario@teste.com");

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public async Task AtivarAsync_ComPrestadorExistente_DeveAtivarComSucesso()
    {
        // Arrange
        var prestadorId = Guid.NewGuid();
        var prestador = new Prestador("João Silva", "Corte", "joao@exemplo.com", "(11) 99999-9999", Guid.NewGuid());
        prestador.Desativar();

        _mockRepository.Setup(r => r.ObterPorIdAsync(prestadorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(prestador);
        _mockRepository.Setup(r => r.AtualizarAsync(It.IsAny<Prestador>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(prestador);
        _mockRepository.Setup(r => r.SalvarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var resultado = await _service.AtivarAsync(prestadorId, "usuario@teste.com");

        // Assert
        resultado.Should().BeTrue();
        prestador.Ativo.Should().BeTrue();
        
        _mockRepository.Verify(r => r.AtualizarAsync(It.IsAny<Prestador>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.SalvarAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EmailJaExisteAsync_DeveChamarRepositorio()
    {
        // Arrange
        var email = "joao@exemplo.com";
        var excludeId = Guid.NewGuid();

        _mockRepository.Setup(r => r.EmailJaExisteAsync(email, excludeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var resultado = await _service.EmailJaExisteAsync(email, excludeId);

        // Assert
        resultado.Should().BeTrue();
        _mockRepository.Verify(r => r.EmailJaExisteAsync(email, excludeId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CnpjJaExisteAsync_DeveChamarRepositorio()
    {
        // Arrange
        var cnpj = "12.345.678/0001-90";
        var excludeId = Guid.NewGuid();

        _mockRepository.Setup(r => r.CnpjJaExisteAsync(cnpj, excludeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var resultado = await _service.CnpjJaExisteAsync(cnpj, excludeId);

        // Assert
        resultado.Should().BeTrue();
        _mockRepository.Verify(r => r.CnpjJaExisteAsync(cnpj, excludeId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void PodeAcessarLoja_ComUsuarioLoja_DeveValidarLojaId()
    {
        // Arrange
        var userLojaId = Guid.NewGuid();
        var lojaId = Guid.NewGuid();
        var ehUsuarioLoja = true;

        // Act
        var resultado = _service.PodeAcessarLoja(userLojaId, lojaId, ehUsuarioLoja);

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public void PodeAcessarLoja_ComUsuarioLojaDiferente_DeveRetornarFalse()
    {
        // Arrange
        var userLojaId = Guid.NewGuid();
        var lojaId = Guid.NewGuid();
        var ehUsuarioLoja = true;

        // Act
        var resultado = _service.PodeAcessarLoja(userLojaId, lojaId, ehUsuarioLoja);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public void PodeAcessarLoja_ComUsuarioAdmin_DeveRetornarTrue()
    {
        // Arrange
        var userLojaId = Guid.NewGuid();
        var lojaId = Guid.NewGuid();
        var ehUsuarioLoja = false;

        // Act
        var resultado = _service.PodeAcessarLoja(userLojaId, lojaId, ehUsuarioLoja);

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public void ValidarPermissoesCriacao_ComUsuarioLoja_DeveValidarLojaId()
    {
        // Arrange
        var userLojaId = Guid.NewGuid();
        var prestadorLojaId = userLojaId;
        var ehUsuarioLoja = true;

        // Act
        var resultado = _service.ValidarPermissoesCriacao(userLojaId, prestadorLojaId, ehUsuarioLoja);

        // Assert
        resultado.IsValid.Should().BeTrue();
        resultado.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void ValidarPermissoesCriacao_ComUsuarioLojaDiferente_DeveRetornarErro()
    {
        // Arrange
        var userLojaId = Guid.NewGuid();
        var prestadorLojaId = Guid.NewGuid();
        var ehUsuarioLoja = true;

        // Act
        var resultado = _service.ValidarPermissoesCriacao(userLojaId, prestadorLojaId, ehUsuarioLoja);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.ErrorMessage.Should().Contain("Você só pode criar prestadores para sua loja");
    }

    [Fact]
    public void ValidarPermissoesEdicao_ComUsuarioLoja_DeveValidarLojaId()
    {
        // Arrange
        var userLojaId = Guid.NewGuid();
        var prestadorLojaId = userLojaId;
        var ehUsuarioLoja = true;

        // Act
        var resultado = _service.ValidarPermissoesEdicao(userLojaId, prestadorLojaId, ehUsuarioLoja);

        // Assert
        resultado.IsValid.Should().BeTrue();
        resultado.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void ValidarPermissoesExclusao_ComUsuarioLoja_DeveValidarLojaId()
    {
        // Arrange
        var userLojaId = Guid.NewGuid();
        var prestadorLojaId = userLojaId;
        var ehUsuarioLoja = true;

        // Act
        var resultado = _service.ValidarPermissoesExclusao(userLojaId, prestadorLojaId, ehUsuarioLoja);

        // Assert
        resultado.IsValid.Should().BeTrue();
        resultado.ErrorMessage.Should().BeNull();
    }
}
