using AutoMapper;
using Microsoft.Extensions.Logging;
using AgendaApp.Aplicacao.DTOs;
using AgendaApp.Aplicacao.Interfaces;
using AgendaApp.Dominio.Entities;
using AgendaApp.Dominio.Interfaces;
using AgendaApp.Dominio.ValueObjects;

namespace AgendaApp.Aplicacao.Services;

public class PrestadorService : IPrestadorService
{
    private readonly IPrestadorRepository _prestadorRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PrestadorService> _logger;

    public PrestadorService(
        IPrestadorRepository prestadorRepository,
        IMapper mapper,
        ILogger<PrestadorService> logger)
    {
        _prestadorRepository = prestadorRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PrestadorDto> CriarAsync(CriarPrestadorDto dto, string? usuarioExecutante = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Iniciando criação de prestador: {Nome}", dto.Nome);

        try
        {
            await ValidarDtoParaCriacaoAsync(dto, cancellationToken);

            if (!string.IsNullOrWhiteSpace(dto.CNPJ))
            {
                ValidarCNPJ(dto.CNPJ);
            }

            if (await EmailJaExisteAsync(dto.Email, null, cancellationToken))
            {
                throw new InvalidOperationException($"Já existe um prestador com o e-mail '{dto.Email}'");
            }

            if (!string.IsNullOrWhiteSpace(dto.CNPJ) && await CnpjJaExisteAsync(dto.CNPJ, null, cancellationToken))
            {
                throw new InvalidOperationException($"Já existe um prestador com o CNPJ '{dto.CNPJ}'");
            }

            var prestador = new Prestador(
                dto.Nome,
                dto.AreaAtuacao,
                dto.Email,
                dto.Telefone,
                dto.LojaId, 
                dto.CNPJ
            );

            var prestadorSalvo = await _prestadorRepository.AdicionarAsync(prestador, cancellationToken);
            await _prestadorRepository.SalvarAsync(cancellationToken);

            // LOG DE AUDITORIA - PRESTADOR CRIADO
            _logger.LogInformation("PRESTADOR_CRIADO | Usuario: {Usuario} | PrestadorId: {PrestadorId} | Nome: {Nome} | Email: {Email} | CNPJ: {CNPJ} | AreaAtuacao: {AreaAtuacao} | Telefone: {Telefone} | LojaId: {LojaId} | DataHora: {DataHora}",
                usuarioExecutante ?? "Sistema", 
                prestadorSalvo.Id, 
                prestadorSalvo.Nome, 
                prestadorSalvo.Email.Endereco, 
                prestadorSalvo.CNPJ?.Numero ?? "N/A", 
                prestadorSalvo.AreaAtuacao, 
                prestadorSalvo.Telefone, 
                prestadorSalvo.LojaId, 
                DateTime.UtcNow);

            _logger.LogInformation("Prestador criado com sucesso: {Id} - {Nome}", prestadorSalvo.Id, prestadorSalvo.Nome);

            return ConverterParaDto(prestadorSalvo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar prestador: {Nome}", dto.Nome);
            throw;
        }
    }

    public async Task<PrestadorDto> AtualizarAsync(AtualizarPrestadorDto dto, string? usuarioExecutante = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Iniciando atualização de prestador: {Id}", dto.Id);

        try
        {
            // Buscar prestador existente
            var prestador = await _prestadorRepository.ObterPorIdAsync(dto.Id, cancellationToken);
            if (prestador == null)
            {
                throw new ArgumentException($"Prestador com ID '{dto.Id}' não encontrado");
            }

            // Capturar estado anterior para auditoria
            var estadoAnterior = new
            {
                Nome = prestador.Nome,
                Email = prestador.Email.Endereco,
                CNPJ = prestador.CNPJ?.Numero,
                AreaAtuacao = prestador.AreaAtuacao,
                Telefone = prestador.Telefone,
                Ativo = prestador.Ativo
            };

            // Validar dados de entrada
            await ValidarDtoParaAtualizacaoAsync(dto, cancellationToken);

            // Validar CNPJ se informado
            if (!string.IsNullOrWhiteSpace(dto.CNPJ))
            {
                ValidarCNPJ(dto.CNPJ);
            }

            // Verificar e-mail único
            if (await EmailJaExisteAsync(dto.Email, dto.Id, cancellationToken))
            {
                throw new InvalidOperationException($"Já existe outro prestador com o e-mail '{dto.Email}'");
            }

            // Verificar CNPJ único se informado
            if (!string.IsNullOrWhiteSpace(dto.CNPJ) && await CnpjJaExisteAsync(dto.CNPJ, dto.Id, cancellationToken))
            {
                throw new InvalidOperationException($"Já existe outro prestador com o CNPJ '{dto.CNPJ}'");
            }

            // Atualizar dados
            prestador.AtualizarInformacoes(dto.Nome, dto.AreaAtuacao, dto.Telefone);
            prestador.AtualizarEmail(dto.Email);

            // Atualizar CNPJ
            if (!string.IsNullOrWhiteSpace(dto.CNPJ))
            {
                prestador.DefinirCNPJ(dto.CNPJ);
            }
            else
            {
                prestador.RemoverCNPJ();
            }

            // Atualizar status
            if (dto.Ativo && !prestador.Ativo)
            {
                prestador.Ativar();
            }
            else if (!dto.Ativo && prestador.Ativo)
            {
                prestador.Desativar();
            }

            // Persistir
            var prestadorAtualizado = await _prestadorRepository.AtualizarAsync(prestador, cancellationToken);
            await _prestadorRepository.SalvarAsync(cancellationToken);

            // LOG DE AUDITORIA - PRESTADOR ALTERADO
            var estadoNovo = new
            {
                Nome = prestadorAtualizado.Nome,
                Email = prestadorAtualizado.Email.Endereco,
                CNPJ = prestadorAtualizado.CNPJ?.Numero,
                AreaAtuacao = prestadorAtualizado.AreaAtuacao,
                Telefone = prestadorAtualizado.Telefone,
                Ativo = prestadorAtualizado.Ativo
            };

            _logger.LogInformation("PRESTADOR_ALTERADO | Usuario: {Usuario} | PrestadorId: {PrestadorId} | EstadoAnterior: {EstadoAnterior} | EstadoNovo: {EstadoNovo} | DataHora: {DataHora}",
                usuarioExecutante ?? "Sistema",
                prestadorAtualizado.Id,
                System.Text.Json.JsonSerializer.Serialize(estadoAnterior),
                System.Text.Json.JsonSerializer.Serialize(estadoNovo),
                DateTime.UtcNow);

            _logger.LogInformation("Prestador atualizado com sucesso: {Id} - {Nome}", prestadorAtualizado.Id, prestadorAtualizado.Nome);

            return ConverterParaDto(prestadorAtualizado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar prestador: {Id}", dto.Id);
            throw;
        }
    }

    public async Task<PrestadorDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Buscando prestador por ID: {Id}", id);

        var prestador = await _prestadorRepository.ObterCompletoAsync(id, cancellationToken);
        return prestador != null ? ConverterParaDto(prestador) : null;
    }

    public async Task<IEnumerable<PrestadorResumoDto>> ObterComFiltroAsync(PrestadorFiltroDto filtro, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Buscando prestadores com filtros");

        // TODO: Implementar filtros específicos no repositório
        var prestadores = await _prestadorRepository.ObterTodosAsync(cancellationToken);

        // Aplicar filtros (simulado - deve ser feito no repositório para performance)
        var resultado = prestadores.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(filtro.Nome))
        {
            resultado = resultado.Where(p => p.Nome.Contains(filtro.Nome, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(filtro.AreaAtuacao))
        {
            resultado = resultado.Where(p => p.AreaAtuacao.Contains(filtro.AreaAtuacao, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(filtro.Email))
        {
            resultado = resultado.Where(p => p.Email.Endereco.Contains(filtro.Email, StringComparison.OrdinalIgnoreCase));
        }

        if (filtro.ApenasAtivos)
        {
            resultado = resultado.Where(p => p.Ativo);
        }

        return resultado.Select(ConverterParaResumoDto);
    }

    public async Task<bool> DesativarAsync(Guid id, string? usuarioExecutante = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Desativando prestador: {Id}", id);

        var prestador = await _prestadorRepository.ObterPorIdAsync(id, cancellationToken);
        if (prestador == null) return false;

        prestador.Desativar();
        await _prestadorRepository.AtualizarAsync(prestador, cancellationToken);
        await _prestadorRepository.SalvarAsync(cancellationToken);

        // LOG DE AUDITORIA - PRESTADOR DESATIVADO
        _logger.LogInformation("PRESTADOR_DESATIVADO | Usuario: {Usuario} | PrestadorId: {PrestadorId} | Nome: {Nome} | Email: {Email} | Motivo: Desativação manual | DataHora: {DataHora}",
            usuarioExecutante ?? "Sistema",
            prestador.Id,
            prestador.Nome,
            prestador.Email.Endereco,
            DateTime.UtcNow);

        _logger.LogInformation("Prestador desativado: {Id} - {Nome}", prestador.Id, prestador.Nome);
        return true;
    }

    public async Task<bool> AtivarAsync(Guid id, string? usuarioExecutante = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Ativando prestador: {Id}", id);

        var prestador = await _prestadorRepository.ObterPorIdAsync(id, cancellationToken);
        if (prestador == null) return false;

        prestador.Ativar();
        await _prestadorRepository.AtualizarAsync(prestador, cancellationToken);
        await _prestadorRepository.SalvarAsync(cancellationToken);

        // LOG DE AUDITORIA - PRESTADOR ATIVADO
        _logger.LogInformation("PRESTADOR_ATIVADO | Usuario: {Usuario} | PrestadorId: {PrestadorId} | Nome: {Nome} | Email: {Email} | Motivo: Ativação manual | DataHora: {DataHora}",
            usuarioExecutante ?? "Sistema",
            prestador.Id,
            prestador.Nome,
            prestador.Email.Endereco,
            DateTime.UtcNow);

        _logger.LogInformation("Prestador ativado: {Id} - {Nome}", prestador.Id, prestador.Nome);
        return true;
    }

    public async Task<bool> EmailJaExisteAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        return await _prestadorRepository.EmailJaExisteAsync(email, excludeId, cancellationToken);
    }

    public async Task<bool> CnpjJaExisteAsync(string cnpj, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        return await _prestadorRepository.CnpjJaExisteAsync(cnpj, excludeId, cancellationToken);
    }

    private static async Task ValidarDtoParaCriacaoAsync(CriarPrestadorDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new ArgumentException("Nome é obrigatório");

        if (string.IsNullOrWhiteSpace(dto.AreaAtuacao))
            throw new ArgumentException("Área de atuação é obrigatória");

        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new ArgumentException("E-mail é obrigatório");

        if (string.IsNullOrWhiteSpace(dto.Telefone))
            throw new ArgumentException("Telefone é obrigatório");

        await Task.CompletedTask;
    }

    private static async Task ValidarDtoParaAtualizacaoAsync(AtualizarPrestadorDto dto, CancellationToken cancellationToken)
    {
        if (dto.Id == Guid.Empty)
            throw new ArgumentException("ID é obrigatório para atualização");

        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new ArgumentException("Nome é obrigatório");

        if (string.IsNullOrWhiteSpace(dto.AreaAtuacao))
            throw new ArgumentException("Área de atuação é obrigatória");

        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new ArgumentException("E-mail é obrigatório");

        if (string.IsNullOrWhiteSpace(dto.Telefone))
            throw new ArgumentException("Telefone é obrigatório");

        await Task.CompletedTask;
    }

    private static void ValidarCNPJ(string cnpj)
    {
        try
        {
            Cnpj.Criar(cnpj);
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException($"CNPJ inválido: {ex.Message}");
        }
    }

    private PrestadorDto ConverterParaDto(Prestador prestador)
    {
        return new PrestadorDto
        {
            Id = prestador.Id,
            Nome = prestador.Nome,
            CNPJ = prestador.CNPJ?.Numero,
            CNPJFormatado = prestador.CNPJ?.Formatado,
            AreaAtuacao = prestador.AreaAtuacao,
            Email = prestador.Email.Endereco,
            Telefone = prestador.Telefone,
            DataCriacao = prestador.DataCriacao,
            DataAtualizacao = prestador.DataAtualizacao,
            Ativo = prestador.Ativo
        };
    }

    private PrestadorResumoDto ConverterParaResumoDto(Prestador prestador)
    {
        return new PrestadorResumoDto
        {
            Id = prestador.Id,
            Nome = prestador.Nome,
            CNPJFormatado = prestador.CNPJ?.Formatado,
            AreaAtuacao = prestador.AreaAtuacao,
            Email = prestador.Email.Endereco,
            Telefone = prestador.Telefone,
            DataCriacao = prestador.DataCriacao,
            Ativo = prestador.Ativo,
            TotalServicos = prestador.Servicos.Count,
            TotalHorarios = prestador.HorariosDisponiveis.Count
        };
    }

    public bool PodeAcessarLoja(Guid? userLojaId, Guid? lojaId, bool ehUsuarioLoja)
    {
        if (!ehUsuarioLoja)
            return true;

        return userLojaId.HasValue && lojaId.HasValue && userLojaId.Value == lojaId.Value;
    }

    public (bool IsValid, string? ErrorMessage) ValidarPermissoesCriacao(Guid? userLojaId, Guid prestadorLojaId, bool ehUsuarioLoja)
    {
        if (ehUsuarioLoja)
        {
            if (!userLojaId.HasValue || userLojaId.Value != prestadorLojaId)
            {
                return (false, "Você só pode criar prestadores para sua loja.");
            }
        }
        else if (prestadorLojaId == Guid.Empty)
        {
            return (false, "Selecione uma loja para o prestador.");
        }

        return (true, null);
    }

    public (bool IsValid, string? ErrorMessage) ValidarPermissoesEdicao(Guid? userLojaId, Guid prestadorLojaId, bool ehUsuarioLoja)
    {
        if (ehUsuarioLoja)
        {
            if (!userLojaId.HasValue || userLojaId.Value != prestadorLojaId)
            {
                return (false, "Você só pode editar prestadores da sua loja.");
            }
        }

        return (true, null);
    }

    public (bool IsValid, string? ErrorMessage) ValidarPermissoesExclusao(Guid? userLojaId, Guid prestadorLojaId, bool ehUsuarioLoja)
    {
        if (ehUsuarioLoja)
        {
            if (!userLojaId.HasValue || userLojaId.Value != prestadorLojaId)
            {
                return (false, "Você só pode excluir prestadores da sua loja.");
            }
        }

        return (true, null);
    }
} 