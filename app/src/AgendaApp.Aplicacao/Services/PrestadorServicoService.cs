using AgendaApp.Aplicacao.DTOs;
using AgendaApp.Aplicacao.Interfaces;
using AgendaApp.Dominio.Entities;
using AgendaApp.Dominio.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AgendaApp.Aplicacao.Services;

public class PrestadorServicoService : IPrestadorServicoService
{
    private readonly IRepository<PrestadorServico> _prestadorServicoRepository;
    private readonly IRepository<Servico> _servicoRepository;
    private readonly IRepository<Prestador> _prestadorRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PrestadorServicoService> _logger;

    public PrestadorServicoService(
        IRepository<PrestadorServico> prestadorServicoRepository,
        IRepository<Servico> servicoRepository,
        IRepository<Prestador> prestadorRepository,
        IMapper mapper,
        ILogger<PrestadorServicoService> logger)
    {
        _prestadorServicoRepository = prestadorServicoRepository;
        _servicoRepository = servicoRepository;
        _prestadorRepository = prestadorRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<ServicoGlobalDisponivelDto>> ObterServicosGlobaisDisponiveisAsync(Guid prestadorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Obtendo serviços globais disponíveis para prestador {PrestadorId}", prestadorId);

            var prestador = await _prestadorRepository.ObterPorIdAsync(prestadorId);
            if (prestador == null)
            {
                throw new ArgumentException("Prestador não encontrado", nameof(prestadorId));
            }

            var servicosGlobais = await _servicoRepository.ObterTodosAsync();
            var servicosGlobaisDaLoja = servicosGlobais
                .Where(s => !s.PrestadorServicos.Any() && s.LojaId == prestador.LojaId && s.Ativo)
                .ToList();

            var servicosVinculados = await _prestadorServicoRepository.ObterTodosAsync();
            var idsServicosVinculados = servicosVinculados
                .Where(ps => ps.PrestadorId == prestadorId && ps.Ativo)
                .Select(ps => ps.ServicoId)
                .ToHashSet();

            var resultado = servicosGlobaisDaLoja.Select(servico => new ServicoGlobalDisponivelDto
            {
                Id = servico.Id,
                Nome = servico.Nome,
                Descricao = servico.Descricao,
                Valor = servico.Valor,
                DuracaoEmMinutos = servico.DuracaoEmMinutos,
                JaVinculado = idsServicosVinculados.Contains(servico.Id),
                LojaId = servico.LojaId.ToString(),
                LojaNome = servico.Loja?.Nome ?? ""
            }).ToList();

            _logger.LogInformation("Encontrados {Count} serviços globais para prestador {PrestadorId}", resultado.Count, prestadorId);
            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter serviços globais disponíveis para prestador {PrestadorId}", prestadorId);
            throw;
        }
    }

    public async Task<IEnumerable<ServicoVinculadoDto>> ObterServicosVinculadosAsync(Guid prestadorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Obtendo serviços vinculados ao prestador {PrestadorId}", prestadorId);

            var prestadorServicos = await _prestadorServicoRepository.ObterTodosAsync();
            var servicosVinculados = prestadorServicos
                .Where(ps => ps.PrestadorId == prestadorId && ps.Ativo)
                .ToList();

            var resultado = new List<ServicoVinculadoDto>();

            foreach (var prestadorServico in servicosVinculados)
            {
                var servico = await _servicoRepository.ObterPorIdAsync(prestadorServico.ServicoId);
                if (servico != null && servico.Ativo)
                {
                    resultado.Add(new ServicoVinculadoDto
                    {
                        PrestadorServicoId = prestadorServico.Id,
                        ServicoId = servico.Id,
                        ServicoNome = servico.Nome,
                        ServicoDescricao = servico.Descricao,
                        ValorPadrao = servico.Valor,
                        DuracaoPadraoEmMinutos = servico.DuracaoEmMinutos,
                        ValorPersonalizado = prestadorServico.ValorPersonalizado,
                        DuracaoPersonalizadaEmMinutos = prestadorServico.DuracaoPersonalizadaEmMinutos,
                        DataVinculacao = prestadorServico.DataCriacao
                    });
                }
            }

            _logger.LogInformation("Encontrados {Count} serviços vinculados ao prestador {PrestadorId}", resultado.Count, prestadorId);
            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter serviços vinculados ao prestador {PrestadorId}", prestadorId);
            throw;
        }
    }

    public async Task<ServicoVinculadoDto> VincularServicoAsync(VincularServicoDto dto, string? usuarioExecutante = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Vinculando serviço {ServicoId} ao prestador {PrestadorId}", dto.ServicoId, dto.PrestadorId);

            var prestador = await _prestadorRepository.ObterPorIdAsync(dto.PrestadorId);
            if (prestador == null)
            {
                throw new ArgumentException("Prestador não encontrado", nameof(dto.PrestadorId));
            }

            var servico = await _servicoRepository.ObterPorIdAsync(dto.ServicoId);
            if (servico == null)
            {
                throw new ArgumentException("Serviço não encontrado", nameof(dto.ServicoId));
            }

            if (servico.PrestadorServicos.Any())
            {
                throw new InvalidOperationException("Apenas serviços globais podem ser vinculados a prestadores");
            }

            var vinculacaoExistente = await VerificarVinculacaoExisteAsync(dto.PrestadorId, dto.ServicoId, cancellationToken);
            if (vinculacaoExistente)
            {
                throw new InvalidOperationException("Este serviço já está vinculado ao prestador");
            }

            var prestadorServico = new PrestadorServico(
                dto.PrestadorId,
                dto.ServicoId,
                dto.ValorPersonalizado,
                dto.DuracaoPersonalizadaEmMinutos);

            await _prestadorServicoRepository.AdicionarAsync(prestadorServico);

            // LOG DE AUDITORIA - SERVIÇO VINCULADO
            _logger.LogInformation("SERVICO_VINCULADO | Usuario: {Usuario} | PrestadorServicoId: {PrestadorServicoId} | PrestadorId: {PrestadorId} | PrestadorNome: {PrestadorNome} | ServicoId: {ServicoId} | ServicoNome: {ServicoNome} | ValorPersonalizado: {ValorPersonalizado} | DuracaoPersonalizada: {DuracaoPersonalizada} | DataHora: {DataHora}",
                usuarioExecutante ?? "Sistema",
                prestadorServico.Id,
                prestadorServico.PrestadorId,
                prestador.Nome,
                prestadorServico.ServicoId,
                servico.Nome,
                prestadorServico.ValorPersonalizado?.ToString() ?? "Padrão",
                prestadorServico.DuracaoPersonalizadaEmMinutos?.ToString() ?? "Padrão",
                DateTime.UtcNow);

            _logger.LogInformation("Serviço {ServicoId} vinculado com sucesso ao prestador {PrestadorId}", dto.ServicoId, dto.PrestadorId);

            return new ServicoVinculadoDto
            {
                PrestadorServicoId = prestadorServico.Id,
                ServicoId = servico.Id,
                ServicoNome = servico.Nome,
                ServicoDescricao = servico.Descricao,
                ValorPadrao = servico.Valor,
                DuracaoPadraoEmMinutos = servico.DuracaoEmMinutos,
                ValorPersonalizado = prestadorServico.ValorPersonalizado,
                DuracaoPersonalizadaEmMinutos = prestadorServico.DuracaoPersonalizadaEmMinutos,
                DataVinculacao = prestadorServico.DataCriacao
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao vincular serviço {ServicoId} ao prestador {PrestadorId}", dto.ServicoId, dto.PrestadorId);
            throw;
        }
    }

    public async Task<ServicoVinculadoDto> AtualizarServicoVinculadoAsync(AtualizarServicoVinculadoDto dto, string? usuarioExecutante = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Atualizando valores personalizados da vinculação {PrestadorServicoId}", dto.PrestadorServicoId);

            var prestadorServico = await _prestadorServicoRepository.ObterPorIdAsync(dto.PrestadorServicoId);
            if (prestadorServico == null)
            {
                throw new ArgumentException("Vinculação não encontrada", nameof(dto.PrestadorServicoId));
            }

            prestadorServico.AtualizarValoresPersonalizados(dto.ValorPersonalizado, dto.DuracaoPersonalizadaEmMinutos);
            await _prestadorServicoRepository.AtualizarAsync(prestadorServico);

            var servico = await _servicoRepository.ObterPorIdAsync(prestadorServico.ServicoId);
            if (servico == null)
            {
                throw new InvalidOperationException("Serviço vinculado não encontrado");
            }

            _logger.LogInformation("Valores personalizados atualizados com sucesso para vinculação {PrestadorServicoId}", dto.PrestadorServicoId);

            return new ServicoVinculadoDto
            {
                PrestadorServicoId = prestadorServico.Id,
                ServicoId = servico.Id,
                ServicoNome = servico.Nome,
                ServicoDescricao = servico.Descricao,
                ValorPadrao = servico.Valor,
                DuracaoPadraoEmMinutos = servico.DuracaoEmMinutos,
                ValorPersonalizado = prestadorServico.ValorPersonalizado,
                DuracaoPersonalizadaEmMinutos = prestadorServico.DuracaoPersonalizadaEmMinutos,
                DataVinculacao = prestadorServico.DataCriacao
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar vinculação {PrestadorServicoId}", dto.PrestadorServicoId);
            throw;
        }
    }

    public async Task<bool> RemoverVinculacaoAsync(Guid prestadorServicoId, string? usuarioExecutante = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Removendo vinculação {PrestadorServicoId}", prestadorServicoId);

            var prestadorServico = await _prestadorServicoRepository.ObterPorIdAsync(prestadorServicoId);
            if (prestadorServico == null)
            {
                _logger.LogWarning("Vinculação {PrestadorServicoId} não encontrada para remoção", prestadorServicoId);
                return false;
            }

            // Capturar informações para auditoria antes de desativar
            var prestador = await _prestadorRepository.ObterPorIdAsync(prestadorServico.PrestadorId);
            var servico = await _servicoRepository.ObterPorIdAsync(prestadorServico.ServicoId);

            prestadorServico.Desativar();
            await _prestadorServicoRepository.AtualizarAsync(prestadorServico);

            // LOG DE AUDITORIA - SERVIÇO DESVINCULADO
            _logger.LogInformation("SERVICO_DESVINCULADO | Usuario: {Usuario} | PrestadorServicoId: {PrestadorServicoId} | PrestadorId: {PrestadorId} | PrestadorNome: {PrestadorNome} | ServicoId: {ServicoId} | ServicoNome: {ServicoNome} | Motivo: Desvinculação manual | DataHora: {DataHora}",
                usuarioExecutante ?? "Sistema",
                prestadorServico.Id,
                prestadorServico.PrestadorId,
                prestador?.Nome ?? "N/A",
                prestadorServico.ServicoId,
                servico?.Nome ?? "N/A",
                DateTime.UtcNow);

            _logger.LogInformation("Vinculação {PrestadorServicoId} removida com sucesso", prestadorServicoId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover vinculação {PrestadorServicoId}", prestadorServicoId);
            return false;
        }
    }

    public async Task<bool> VerificarVinculacaoExisteAsync(Guid prestadorId, Guid servicoId, CancellationToken cancellationToken = default)
    {
        try
        {
            var prestadorServicos = await _prestadorServicoRepository.ObterTodosAsync();
            return prestadorServicos.Any(ps => 
                ps.PrestadorId == prestadorId && 
                ps.ServicoId == servicoId && 
                ps.Ativo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar vinculação entre prestador {PrestadorId} e serviço {ServicoId}", prestadorId, servicoId);
            return false;
        }
    }
} 