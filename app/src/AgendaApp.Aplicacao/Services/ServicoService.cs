using AutoMapper;
using Microsoft.Extensions.Logging;
using AgendaApp.Aplicacao.DTOs;
using AgendaApp.Aplicacao.Interfaces;
using AgendaApp.Dominio.Entities;
using AgendaApp.Dominio.Interfaces;

namespace AgendaApp.Aplicacao.Services;

public class ServicoService : IServicoService
{
    private readonly IServicoRepository _servicoRepository;
    private readonly IPrestadorRepository _prestadorRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ServicoService> _logger;

    public ServicoService(
        IServicoRepository servicoRepository,
        IPrestadorRepository prestadorRepository,
        IMapper mapper,
        ILogger<ServicoService> logger)
    {
        _servicoRepository = servicoRepository;
        _prestadorRepository = prestadorRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ServicoDto> CriarAsync(CriarServicoDto dto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Criando serviço: {Nome} para prestador {PrestadorId}", dto.Nome, dto.PrestadorId);

        try
        {
            await ValidarDtoParaCriacaoAsync(dto, cancellationToken);

            Prestador? prestador = null;
            
            if (dto.PrestadorId.HasValue)
            {
                prestador = await _prestadorRepository.ObterPorIdAsync(dto.PrestadorId.Value, cancellationToken);
                if (prestador == null)
                {
                    throw new ArgumentException($"Prestador com ID '{dto.PrestadorId}' não encontrado");
                }

                if (!prestador.Ativo)
                {
                    throw new InvalidOperationException($"Não é possível criar serviços para o prestador '{prestador.Nome}' pois está inativo");
                }

                if (await NomeJaExisteParaPrestadorAsync(dto.Nome, dto.PrestadorId.Value, null, cancellationToken))
                {
                    throw new InvalidOperationException($"Já existe um serviço com o nome '{dto.Nome}' para o prestador '{prestador.Nome}'");
                }
            }
            else
            {
                if (await NomeJaExisteNaLojaAsync(dto.Nome, dto.LojaId, null, cancellationToken))
                {
                    throw new InvalidOperationException($"Já existe um serviço global com o nome '{dto.Nome}' na loja");
                }
            }

            var servico = new Servico(
                dto.Nome,
                dto.Descricao,
                dto.Valor,
                dto.DuracaoEmMinutos,
                dto.LojaId ?? Guid.Empty
            );

            var servicoSalvo = await _servicoRepository.AdicionarAsync(servico, cancellationToken);
            await _servicoRepository.SalvarAsync(cancellationToken);

            _logger.LogInformation("Serviço criado com sucesso: {Id} - {Nome}", servicoSalvo.Id, servicoSalvo.Nome);

            return ConverterParaDto(servicoSalvo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar serviço: {Nome}", dto.Nome);
            throw;
        }
    }

    public async Task<ServicoDto> AtualizarAsync(AtualizarServicoDto dto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Atualizando serviço: {Id}", dto.Id);

        try
        {
            var servico = await _servicoRepository.ObterPorIdAsync(dto.Id, cancellationToken);
            if (servico == null)
            {
                throw new ArgumentException($"Serviço com ID '{dto.Id}' não encontrado");
            }

            await ValidarDtoParaAtualizacaoAsync(dto, cancellationToken);

            Prestador? prestador = null;
            
            if (dto.PrestadorId.HasValue)
            {
                prestador = await _prestadorRepository.ObterPorIdAsync(dto.PrestadorId.Value, cancellationToken);
                if (prestador == null)
                {
                    throw new ArgumentException($"Prestador com ID '{dto.PrestadorId}' não encontrado");
                }

                if (await NomeJaExisteParaPrestadorAsync(dto.Nome, dto.PrestadorId.Value, dto.Id, cancellationToken))
                {
                    throw new InvalidOperationException($"Já existe outro serviço com o nome '{dto.Nome}' para o prestador '{prestador.Nome}'");
                }
            }
            else
            {
                if (await NomeJaExisteNaLojaAsync(dto.Nome, dto.LojaId, dto.Id, cancellationToken))
                {
                    throw new InvalidOperationException($"Já existe outro serviço global com o nome '{dto.Nome}' na loja");
                }
            }

            servico.AtualizarInformacoes(dto.Nome, dto.Descricao, dto.Valor, dto.DuracaoEmMinutos);

            if (dto.Ativo && !servico.Ativo)
            {
                servico.Ativar();
            }
            else if (!dto.Ativo && servico.Ativo)
            {
                servico.Desativar();
            }

            var servicoAtualizado = await _servicoRepository.AtualizarAsync(servico, cancellationToken);
            await _servicoRepository.SalvarAsync(cancellationToken);

            _logger.LogInformation("Serviço atualizado com sucesso: {Id} - {Nome}", servicoAtualizado.Id, servicoAtualizado.Nome);

            return ConverterParaDto(servicoAtualizado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar serviço: {Id}", dto.Id);
            throw;
        }
    }

    public async Task<ServicoDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Buscando serviço por ID: {Id}", id);

        var servico = await _servicoRepository.ObterPorIdAsync(id, cancellationToken);
        return servico != null ? ConverterParaDto(servico) : null;
    }

    public async Task<IEnumerable<ServicoResumoDto>> ObterComFiltroAsync(ServicoFiltroDto filtro, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Buscando serviços com filtros");

        var servicos = await _servicoRepository.ObterTodosAsync(cancellationToken);

        var resultado = servicos.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(filtro.Nome))
        {
            resultado = resultado.Where(s => s.Nome.Contains(filtro.Nome, StringComparison.OrdinalIgnoreCase));
        }

        if (filtro.PrestadorId.HasValue)
        {
        }

        if (filtro.ValorMinimo.HasValue)
        {
            resultado = resultado.Where(s => s.Valor >= filtro.ValorMinimo.Value);
        }

        if (filtro.ValorMaximo.HasValue)
        {
            resultado = resultado.Where(s => s.Valor <= filtro.ValorMaximo.Value);
        }

        if (filtro.DuracaoMaxima.HasValue)
        {
            resultado = resultado.Where(s => s.DuracaoEmMinutos <= filtro.DuracaoMaxima.Value);
        }

        if (filtro.ApenasAtivos)
        {
            resultado = resultado.Where(s => s.Ativo);
        }

        return resultado.Select(ConverterParaResumoDto);
    }

    public async Task<IEnumerable<ServicoResumoDto>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Buscando serviços por prestador: {PrestadorId}", prestadorId);

        var servicos = await _servicoRepository.ObterPorPrestadorAsync(prestadorId, cancellationToken);
        return servicos.Select(ConverterParaResumoDto);
    }

    public async Task<bool> DesativarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Desativando serviço: {Id}", id);

        var servico = await _servicoRepository.ObterPorIdAsync(id, cancellationToken);
        if (servico == null) return false;

        servico.Desativar();
        await _servicoRepository.AtualizarAsync(servico, cancellationToken);
        await _servicoRepository.SalvarAsync(cancellationToken);

        _logger.LogInformation("Serviço desativado: {Id} - {Nome}", servico.Id, servico.Nome);
        return true;
    }

    public async Task<bool> NomeJaExisteParaPrestadorAsync(string nome, Guid prestadorId, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        return await _servicoRepository.NomeJaExisteParaPrestadorAsync(nome, prestadorId, excludeId, cancellationToken);
    }

    public async Task<bool> NomeJaExisteNaLojaAsync(string nome, Guid? lojaId, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        if (!lojaId.HasValue) return false;
        
        var servicos = await _servicoRepository.ObterTodosAsync(cancellationToken);
        return servicos.Any(s => 
            s.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase) &&
            s.LojaId == lojaId &&
            !s.PrestadorServicos.Any() &&
            s.Ativo &&
            (excludeId == null || s.Id != excludeId));
    }

    public async Task<ServicoEstatisticasDto> ObterEstatisticasAsync(Guid? prestadorId = null, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Obtendo estatísticas de serviços para prestador: {PrestadorId}", prestadorId);

        var estatisticas = await _servicoRepository.ObterEstatisticasAsync(prestadorId, cancellationToken);

        return new ServicoEstatisticasDto
        {
            TotalServicos = estatisticas.TotalServicos,
            ValorMedio = estatisticas.ValorMedio,
            ValorMinimo = estatisticas.ValorMinimo,
            ValorMaximo = estatisticas.ValorMaximo,
            DuracaoMediaMinutos = estatisticas.DuracaoMediaMinutos,
            DuracaoMediaFormatada = FormatarDuracao(estatisticas.DuracaoMediaMinutos)
        };
    }

    #region Métodos Privados de Validação

    private static async Task ValidarDtoParaCriacaoAsync(CriarServicoDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new ArgumentException("Nome do serviço é obrigatório");

        if (string.IsNullOrWhiteSpace(dto.Descricao))
            throw new ArgumentException("Descrição é obrigatória");

        if (dto.Valor <= 0)
            throw new ArgumentException("Valor deve ser maior que zero");

        if (dto.DuracaoEmMinutos <= 0)
            throw new ArgumentException("Duração deve ser maior que zero");

        if (dto.DuracaoEmMinutos < 15)
            throw new ArgumentException("Duração mínima é de 15 minutos");

        if (dto.PrestadorId == Guid.Empty)
            throw new ArgumentException("Prestador é obrigatório");

        await Task.CompletedTask;
    }

    private static async Task ValidarDtoParaAtualizacaoAsync(AtualizarServicoDto dto, CancellationToken cancellationToken)
    {
        if (dto.Id == Guid.Empty)
            throw new ArgumentException("ID é obrigatório para atualização");

        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new ArgumentException("Nome do serviço é obrigatório");

        if (string.IsNullOrWhiteSpace(dto.Descricao))
            throw new ArgumentException("Descrição é obrigatória");

        if (dto.Valor <= 0)
            throw new ArgumentException("Valor deve ser maior que zero");

        if (dto.DuracaoEmMinutos <= 0)
            throw new ArgumentException("Duração deve ser maior que zero");

        if (dto.DuracaoEmMinutos < 15)
            throw new ArgumentException("Duração mínima é de 15 minutos");

        if (dto.PrestadorId == Guid.Empty)
            throw new ArgumentException("Prestador é obrigatório");

        await Task.CompletedTask;
    }

    #endregion

    #region Métodos de Conversão

    private static ServicoDto ConverterParaDto(Servico servico)
    {
        return new ServicoDto
        {
            Id = servico.Id,
            Nome = servico.Nome,
            Descricao = servico.Descricao,
            Valor = servico.Valor,
            ValorFormatado = servico.Valor.ToString("C"),
            DuracaoEmMinutos = servico.DuracaoEmMinutos,
            DuracaoFormatada = FormatarDuracao(servico.DuracaoEmMinutos),
            PrestadorId = null,
            PrestadorNome = null,
            DataCriacao = servico.DataCriacao,
            DataAtualizacao = servico.DataAtualizacao,
            Ativo = servico.Ativo,
            LojaId = servico.LojaId
        };
    }

    private static ServicoResumoDto ConverterParaResumoDto(Servico servico)
    {
        return new ServicoResumoDto
        {
            Id = servico.Id,
            Nome = servico.Nome,
            Descricao = servico.Descricao,
            Valor = servico.Valor,
            ValorFormatado = servico.Valor.ToString("C"),
            DuracaoEmMinutos = servico.DuracaoEmMinutos,
            DuracaoFormatada = FormatarDuracao(servico.DuracaoEmMinutos),
            PrestadorId = null,
            PrestadorNome = string.Empty,
            DataCriacao = servico.DataCriacao,
            Ativo = servico.Ativo,
            LojaId = servico.LojaId
        };
    }

    private static string FormatarDuracao(int minutos)
    {
        return minutos >= 60 
            ? $"{minutos / 60}h {minutos % 60:00}min"
            : $"{minutos} min";
    }

    #endregion

    #region Métodos de Validação de Permissões

    public (bool IsValid, string? ErrorMessage) ValidarPermissoesCriacao(Guid? userLojaId, Guid servicoLojaId, bool ehUsuarioLoja)
    {
        if (ehUsuarioLoja)
        {
            if (!userLojaId.HasValue || userLojaId.Value != servicoLojaId)
            {
                return (false, "Você só pode criar serviços para sua loja.");
            }
        }

        return (true, null);
    }

    public (bool IsValid, string? ErrorMessage) ValidarPermissoesEdicao(Guid? userLojaId, Guid servicoLojaId, bool ehUsuarioLoja)
    {
        if (ehUsuarioLoja)
        {
            if (!userLojaId.HasValue || userLojaId.Value != servicoLojaId)
            {
                return (false, "Você só pode editar serviços da sua loja.");
            }
        }

        return (true, null);
    }

    public (bool IsValid, string? ErrorMessage) ValidarPermissoesExclusao(Guid? userLojaId, Guid servicoLojaId, bool ehUsuarioLoja)
    {
        if (ehUsuarioLoja)
        {
            if (!userLojaId.HasValue || userLojaId.Value != servicoLojaId)
            {
                return (false, "Você só pode excluir serviços da sua loja.");
            }
        }

        return (true, null);
    }

    public (bool IsValid, string? ErrorMessage) PodeAcessarServico(Guid? userLojaId, Guid servicoLojaId, bool ehUsuarioLoja)
    {
        if (!ehUsuarioLoja)
            return (true, null);

        if (userLojaId.HasValue && userLojaId.Value == servicoLojaId)
            return (true, null);

        return (false, "Você não tem permissão para acessar este serviço.");
    }

    #endregion
} 