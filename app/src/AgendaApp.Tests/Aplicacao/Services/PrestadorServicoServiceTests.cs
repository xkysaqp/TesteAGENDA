using AgendaApp.Aplicacao.DTOs;
using AgendaApp.Aplicacao.Services;
using AgendaApp.Dominio.Entities;
using AgendaApp.Dominio.Interfaces;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AgendaApp.Tests.Aplicacao.Services;

public class PrestadorServicoServiceTests
{
    private readonly Mock<IRepository<PrestadorServico>> _mockPrestadorServicoRepository;
    private readonly Mock<IRepository<Servico>> _mockServicoRepository;
    private readonly Mock<IRepository<Prestador>> _mockPrestadorRepository;
    private readonly Mock<ILogger<PrestadorServicoService>> _mockLogger;
    private readonly PrestadorServicoService _service;
    private readonly Fixture _fixture;

    public PrestadorServicoServiceTests()
    {
        _mockPrestadorServicoRepository = new Mock<IRepository<PrestadorServico>>();
        _mockServicoRepository = new Mock<IRepository<Servico>>();
        _mockPrestadorRepository = new Mock<IRepository<Prestador>>();
        _mockLogger = new Mock<ILogger<PrestadorServicoService>>();
        _service = new PrestadorServicoService(
            _mockPrestadorServicoRepository.Object,
            _mockServicoRepository.Object,
            _mockPrestadorRepository.Object,
            null!,
            _mockLogger.Object);
        _fixture = new Fixture();
        _fixture.Customize(new AutoMoqCustomization());
    }

    [Fact]
    public async Task ObterServicosGlobaisDisponiveisAsync_ComPrestadorValido_DeveRetornarServicos()
    {
        // Arrange
        var prestadorId = Guid.NewGuid();
        var lojaId = Guid.NewGuid();
        var prestador = new Prestador("João Silva", "Corte", "joao@exemplo.com", "(11) 99999-9999", lojaId);
        
        var servico1 = new Servico("Corte Masculino", "Corte tradicional", 50.00m, 30, lojaId);
        var servico2 = new Servico("Barba", "Barba tradicional", 30.00m, 20, lojaId);
        var servicos = new List<Servico> { servico1, servico2 };
        
        var prestadorServico = new PrestadorServico(prestadorId, servico1.Id, 55.00m, 35);
        var prestadorServicos = new List<PrestadorServico> { prestadorServico };

        _mockPrestadorRepository.Setup(r => r.ObterPorIdAsync(prestadorId))
            .ReturnsAsync(prestador);
        _mockServicoRepository.Setup(r => r.ObterTodosAsync())
            .ReturnsAsync(servicos);
        _mockPrestadorServicoRepository.Setup(r => r.ObterTodosAsync())
            .ReturnsAsync(prestadorServicos);

        // Act
        var resultado = await _service.ObterServicosGlobaisDisponiveisAsync(prestadorId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);
        resultado.First(s => s.Id == servico1.Id).JaVinculado.Should().BeTrue();
        resultado.First(s => s.Id == servico2.Id).JaVinculado.Should().BeFalse();
    }

    [Fact]
    public async Task ObterServicosGlobaisDisponiveisAsync_ComPrestadorInexistente_DeveLancarExcecao()
    {
        // Arrange
        var prestadorId = Guid.NewGuid();

        _mockPrestadorRepository.Setup(r => r.ObterPorIdAsync(prestadorId))
            .ReturnsAsync((Prestador?)null);

        // Act & Assert
        var action = () => _service.ObterServicosGlobaisDisponiveisAsync(prestadorId);
        await action.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Prestador não encontrado*");
    }

    [Fact]
    public async Task ObterServicosVinculadosAsync_ComPrestadorValido_DeveRetornarServicosVinculados()
    {
        // Arrange
        var prestadorId = Guid.NewGuid();
        var servicoId = Guid.NewGuid();
        var lojaId = Guid.NewGuid();
        
        var prestadorServico = new PrestadorServico(prestadorId, servicoId, 55.00m, 35);
        var prestadorServicos = new List<PrestadorServico> { prestadorServico };
        
        var servico = new Servico("Corte Masculino", "Corte tradicional", 50.00m, 30, lojaId);

        _mockPrestadorServicoRepository.Setup(r => r.ObterTodosAsync())
            .ReturnsAsync(prestadorServicos);
        _mockServicoRepository.Setup(r => r.ObterPorIdAsync(servicoId))
            .ReturnsAsync(servico);

        // Act
        var resultado = await _service.ObterServicosVinculadosAsync(prestadorId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(1);
        var servicoVinculado = resultado.First();
        servicoVinculado.ServicoId.Should().Be(servicoId);
        servicoVinculado.ServicoNome.Should().Be("Corte Masculino");
        servicoVinculado.ValorPersonalizado.Should().Be(55.00m);
        servicoVinculado.DuracaoPersonalizadaEmMinutos.Should().Be(35);
    }

    [Fact]
    public async Task VincularServicoAsync_ComDadosValidos_DeveVincularComSucesso()
    {
        // Arrange
        var prestadorId = Guid.NewGuid();
        var servicoId = Guid.NewGuid();
        var lojaId = Guid.NewGuid();
        
        var dto = new VincularServicoDto
        {
            PrestadorId = prestadorId,
            ServicoId = servicoId,
            ValorPersonalizado = 55.00m,
            DuracaoPersonalizadaEmMinutos = 35
        };

        var prestador = new Prestador("João Silva", "Corte", "joao@exemplo.com", "(11) 99999-9999", lojaId);
        var servico = new Servico("Corte Masculino", "Corte tradicional", 50.00m, 30, lojaId);
        var prestadorServico = new PrestadorServico(prestadorId, servicoId, 55.00m, 35);

        _mockPrestadorRepository.Setup(r => r.ObterPorIdAsync(prestadorId))
            .ReturnsAsync(prestador);
        _mockServicoRepository.Setup(r => r.ObterPorIdAsync(servicoId))
            .ReturnsAsync(servico);
        _mockPrestadorServicoRepository.Setup(r => r.ObterTodosAsync())
            .ReturnsAsync(new List<PrestadorServico>());
        _mockPrestadorServicoRepository.Setup(r => r.AdicionarAsync(It.IsAny<PrestadorServico>()))
            .ReturnsAsync(prestadorServico);

        // Act
        var resultado = await _service.VincularServicoAsync(dto, "usuario@teste.com");

        // Assert
        resultado.Should().NotBeNull();
        resultado.ServicoId.Should().Be(servicoId);
        resultado.ValorPersonalizado.Should().Be(55.00m);
        resultado.DuracaoPersonalizadaEmMinutos.Should().Be(35);
        
        _mockPrestadorServicoRepository.Verify(r => r.AdicionarAsync(It.IsAny<PrestadorServico>()), Times.Once);
    }

    [Fact]
    public async Task VincularServicoAsync_ComPrestadorInexistente_DeveLancarExcecao()
    {
        // Arrange
        var prestadorId = Guid.NewGuid();
        var servicoId = Guid.NewGuid();
        
        var dto = new VincularServicoDto
        {
            PrestadorId = prestadorId,
            ServicoId = servicoId,
            ValorPersonalizado = 55.00m,
            DuracaoPersonalizadaEmMinutos = 35
        };

        _mockPrestadorRepository.Setup(r => r.ObterPorIdAsync(prestadorId))
            .ReturnsAsync((Prestador?)null);

        // Act & Assert
        var action = () => _service.VincularServicoAsync(dto, "usuario@teste.com");
        await action.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Prestador não encontrado*");
    }

    [Fact]
    public async Task VincularServicoAsync_ComServicoInexistente_DeveLancarExcecao()
    {
        // Arrange
        var prestadorId = Guid.NewGuid();
        var servicoId = Guid.NewGuid();
        var lojaId = Guid.NewGuid();
        
        var dto = new VincularServicoDto
        {
            PrestadorId = prestadorId,
            ServicoId = servicoId,
            ValorPersonalizado = 55.00m,
            DuracaoPersonalizadaEmMinutos = 35
        };

        var prestador = new Prestador("João Silva", "Corte", "joao@exemplo.com", "(11) 99999-9999", lojaId);

        _mockPrestadorRepository.Setup(r => r.ObterPorIdAsync(prestadorId))
            .ReturnsAsync(prestador);
        _mockServicoRepository.Setup(r => r.ObterPorIdAsync(servicoId))
            .ReturnsAsync((Servico?)null);

        // Act & Assert
        var action = () => _service.VincularServicoAsync(dto, "usuario@teste.com");
        await action.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Serviço não encontrado*");
    }

    [Fact]
    public async Task VincularServicoAsync_ComVinculacaoExistente_DeveLancarExcecao()
    {
        // Arrange
        var prestadorId = Guid.NewGuid();
        var servicoId = Guid.NewGuid();
        var lojaId = Guid.NewGuid();
        
        var dto = new VincularServicoDto
        {
            PrestadorId = prestadorId,
            ServicoId = servicoId,
            ValorPersonalizado = 55.00m,
            DuracaoPersonalizadaEmMinutos = 35
        };

        var prestador = new Prestador("João Silva", "Corte", "joao@exemplo.com", "(11) 99999-9999", lojaId);
        var servico = new Servico("Corte Masculino", "Corte tradicional", 50.00m, 30, lojaId);
        var prestadorServico = new PrestadorServico(prestadorId, servicoId, 55.00m, 35);

        _mockPrestadorRepository.Setup(r => r.ObterPorIdAsync(prestadorId))
            .ReturnsAsync(prestador);
        _mockServicoRepository.Setup(r => r.ObterPorIdAsync(servicoId))
            .ReturnsAsync(servico);
        _mockPrestadorServicoRepository.Setup(r => r.ObterTodosAsync())
            .ReturnsAsync(new List<PrestadorServico> { prestadorServico });

        // Act & Assert
        var action = () => _service.VincularServicoAsync(dto, "usuario@teste.com");
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Este serviço já está vinculado ao prestador*");
    }

    [Fact]
    public async Task AtualizarServicoVinculadoAsync_ComDadosValidos_DeveAtualizarComSucesso()
    {
        // Arrange
        var prestadorServicoId = Guid.NewGuid();
        var prestadorId = Guid.NewGuid();
        var servicoId = Guid.NewGuid();
        var lojaId = Guid.NewGuid();
        
        var dto = new AtualizarServicoVinculadoDto
        {
            PrestadorServicoId = prestadorServicoId,
            ValorPersonalizado = 60.00m,
            DuracaoPersonalizadaEmMinutos = 40
        };

        var prestadorServico = new PrestadorServico(prestadorId, servicoId, 55.00m, 35);
        var servico = new Servico("Corte Masculino", "Corte tradicional", 50.00m, 30, lojaId);

        _mockPrestadorServicoRepository.Setup(r => r.ObterPorIdAsync(prestadorServicoId))
            .ReturnsAsync(prestadorServico);
        _mockPrestadorServicoRepository.Setup(r => r.AtualizarAsync(It.IsAny<PrestadorServico>()))
            .ReturnsAsync(prestadorServico);
        _mockServicoRepository.Setup(r => r.ObterPorIdAsync(servicoId))
            .ReturnsAsync(servico);

        // Act
        var resultado = await _service.AtualizarServicoVinculadoAsync(dto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.ValorPersonalizado.Should().Be(60.00m);
        resultado.DuracaoPersonalizadaEmMinutos.Should().Be(40);
        
        _mockPrestadorServicoRepository.Verify(r => r.AtualizarAsync(It.IsAny<PrestadorServico>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarServicoVinculadoAsync_ComVinculacaoInexistente_DeveLancarExcecao()
    {
        // Arrange
        var prestadorServicoId = Guid.NewGuid();
        
        var dto = new AtualizarServicoVinculadoDto
        {
            PrestadorServicoId = prestadorServicoId,
            ValorPersonalizado = 60.00m,
            DuracaoPersonalizadaEmMinutos = 40
        };

        _mockPrestadorServicoRepository.Setup(r => r.ObterPorIdAsync(prestadorServicoId))
            .ReturnsAsync((PrestadorServico?)null);

        // Act & Assert
        var action = () => _service.AtualizarServicoVinculadoAsync(dto);
        await action.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Vinculação não encontrada*");
    }

    [Fact]
    public async Task RemoverVinculacaoAsync_ComVinculacaoExistente_DeveRemoverComSucesso()
    {
        // Arrange
        var prestadorServicoId = Guid.NewGuid();
        var prestadorId = Guid.NewGuid();
        var servicoId = Guid.NewGuid();
        var lojaId = Guid.NewGuid();
        
        var prestadorServico = new PrestadorServico(prestadorId, servicoId, 55.00m, 35);
        var prestador = new Prestador("João Silva", "Corte", "joao@exemplo.com", "(11) 99999-9999", lojaId);
        var servico = new Servico("Corte Masculino", "Corte tradicional", 50.00m, 30, lojaId);

        _mockPrestadorServicoRepository.Setup(r => r.ObterPorIdAsync(prestadorServicoId))
            .ReturnsAsync(prestadorServico);
        _mockPrestadorServicoRepository.Setup(r => r.AtualizarAsync(It.IsAny<PrestadorServico>()))
            .ReturnsAsync(prestadorServico);
        _mockPrestadorRepository.Setup(r => r.ObterPorIdAsync(prestadorId))
            .ReturnsAsync(prestador);
        _mockServicoRepository.Setup(r => r.ObterPorIdAsync(servicoId))
            .ReturnsAsync(servico);

        // Act
        var resultado = await _service.RemoverVinculacaoAsync(prestadorServicoId, "usuario@teste.com");

        // Assert
        resultado.Should().BeTrue();
        prestadorServico.Ativo.Should().BeFalse();
        
        _mockPrestadorServicoRepository.Verify(r => r.AtualizarAsync(It.IsAny<PrestadorServico>()), Times.Once);
    }

    [Fact]
    public async Task RemoverVinculacaoAsync_ComVinculacaoInexistente_DeveRetornarFalse()
    {
        // Arrange
        var prestadorServicoId = Guid.NewGuid();

        _mockPrestadorServicoRepository.Setup(r => r.ObterPorIdAsync(prestadorServicoId))
            .ReturnsAsync((PrestadorServico?)null);

        // Act
        var resultado = await _service.RemoverVinculacaoAsync(prestadorServicoId, "usuario@teste.com");

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public async Task VerificarVinculacaoExisteAsync_ComVinculacaoExistente_DeveRetornarTrue()
    {
        // Arrange
        var prestadorId = Guid.NewGuid();
        var servicoId = Guid.NewGuid();
        var prestadorServico = new PrestadorServico(prestadorId, servicoId, 55.00m, 35);
        var prestadorServicos = new List<PrestadorServico> { prestadorServico };

        _mockPrestadorServicoRepository.Setup(r => r.ObterTodosAsync())
            .ReturnsAsync(prestadorServicos);

        // Act
        var resultado = await _service.VerificarVinculacaoExisteAsync(prestadorId, servicoId);

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public async Task VerificarVinculacaoExisteAsync_ComVinculacaoInexistente_DeveRetornarFalse()
    {
        // Arrange
        var prestadorId = Guid.NewGuid();
        var servicoId = Guid.NewGuid();
        var prestadorServicos = new List<PrestadorServico>();

        _mockPrestadorServicoRepository.Setup(r => r.ObterTodosAsync())
            .ReturnsAsync(prestadorServicos);

        // Act
        var resultado = await _service.VerificarVinculacaoExisteAsync(prestadorId, servicoId);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public async Task VerificarVinculacaoExisteAsync_ComVinculacaoInativa_DeveRetornarFalse()
    {
        // Arrange
        var prestadorId = Guid.NewGuid();
        var servicoId = Guid.NewGuid();
        var prestadorServico = new PrestadorServico(prestadorId, servicoId, 55.00m, 35);
        prestadorServico.Desativar();
        var prestadorServicos = new List<PrestadorServico> { prestadorServico };

        _mockPrestadorServicoRepository.Setup(r => r.ObterTodosAsync())
            .ReturnsAsync(prestadorServicos);

        // Act
        var resultado = await _service.VerificarVinculacaoExisteAsync(prestadorId, servicoId);

        // Assert
        resultado.Should().BeFalse();
    }
}
