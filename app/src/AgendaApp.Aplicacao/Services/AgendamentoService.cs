using AgendaApp.Aplicacao.DTOs;
using AgendaApp.Aplicacao.Interfaces;
using AgendaApp.Dominio.Entities;
using AgendaApp.Dominio.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AgendaApp.Aplicacao.Services;

public class AgendamentoService : IAgendamentoService
{
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IServicoRepository _servicoRepository;
    private readonly IPrestadorRepository _prestadorRepository;
    private readonly ILojaRepository _lojaRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<AgendamentoService> _logger;

    public AgendamentoService(
        IAgendamentoRepository agendamentoRepository,
        IServicoRepository servicoRepository,
        IPrestadorRepository prestadorRepository,
        ILojaRepository lojaRepository,
        IMapper mapper,
        ILogger<AgendamentoService> logger)
    {
        _agendamentoRepository = agendamentoRepository;
        _servicoRepository = servicoRepository;
        _prestadorRepository = prestadorRepository;
        _lojaRepository = lojaRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<AgendamentoDto> CriarAsync(CriarAgendamentoDto dto)
    {
        try
        {
            _logger.LogInformation("Criando agendamento para cliente {ClienteNome} em {Data} às {Hora}", 
                dto.ClienteNome, dto.Data, dto.Hora);

            // Validar se o serviço existe
            var servico = await _servicoRepository.ObterPorIdAsync(dto.ServicoId);
            if (servico == null)
                throw new ArgumentException("Serviço não encontrado");

            // Validar se o prestador existe
            var prestador = await _prestadorRepository.ObterPorIdAsync(dto.PrestadorId);
            if (prestador == null)
                throw new ArgumentException("Prestador não encontrado");

            // Validar se a loja existe
            var loja = await _lojaRepository.ObterPorIdAsync(dto.LojaId);
            if (loja == null)
                throw new ArgumentException("Loja não encontrada");

            // Verificar conflitos de horário
            var temConflitos = await TemConflitosAsync(dto.LojaId, dto.Data, dto.Hora, servico.DuracaoEmMinutos, dto.PrestadorId);
            if (temConflitos)
                throw new InvalidOperationException("Já existe um agendamento neste horário");

            var agendamento = new Agendamento(
                dto.Data,
                dto.Hora,
                dto.ClienteNome,
                dto.ClienteTelefone,
                dto.LojaId,
                dto.ServicoId,
                dto.PrestadorId,
                dto.ClienteEmail,
                dto.Observacoes
            );

            var agendamentoCriado = await _agendamentoRepository.AdicionarAsync(agendamento);
            await _agendamentoRepository.SalvarAsync();

            _logger.LogInformation("Agendamento criado com sucesso: {AgendamentoId}", agendamentoCriado.Id);

            return await ObterPorIdAsync(agendamentoCriado.Id) ?? throw new InvalidOperationException("Erro ao recuperar agendamento criado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar agendamento");
            throw;
        }
    }

    public async Task<AgendamentoDto?> ObterPorIdAsync(Guid id)
    {
        try
        {
            var agendamento = await _agendamentoRepository.ObterCompletoAsync(id);
            if (agendamento == null) return null;

            return _mapper.Map<AgendamentoDto>(agendamento);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter agendamento por ID {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<AgendamentoDto>> ObterPorLojaAsync(Guid lojaId)
    {
        try
        {
            var agendamentos = await _agendamentoRepository.ObterPorLojaAsync(lojaId);
            return _mapper.Map<IEnumerable<AgendamentoDto>>(agendamentos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter agendamentos da loja {LojaId}", lojaId);
            throw;
        }
    }

    public async Task<IEnumerable<AgendamentoDto>> ObterPorDataAsync(Guid lojaId, DateOnly data)
    {
        try
        {
            var agendamentos = await _agendamentoRepository.ObterPorDataAsync(lojaId, data);
            return _mapper.Map<IEnumerable<AgendamentoDto>>(agendamentos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter agendamentos da loja {LojaId} na data {Data}", lojaId, data);
            throw;
        }
    }

    public async Task<IEnumerable<AgendamentoDto>> ObterPorPeriodoAsync(Guid lojaId, DateOnly dataInicio, DateOnly dataFim)
    {
        try
        {
            var agendamentos = await _agendamentoRepository.ObterPorPeriodoAsync(lojaId, dataInicio, dataFim);
            return _mapper.Map<IEnumerable<AgendamentoDto>>(agendamentos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter agendamentos da loja {LojaId} no período {DataInicio} a {DataFim}", lojaId, dataInicio, dataFim);
            throw;
        }
    }

    public async Task<IEnumerable<AgendamentoDto>> ObterPorStatusAsync(Guid lojaId, StatusAgendamento status)
    {
        try
        {
            var agendamentos = await _agendamentoRepository.ObterPorStatusAsync(lojaId, status);
            return _mapper.Map<IEnumerable<AgendamentoDto>>(agendamentos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter agendamentos da loja {LojaId} com status {Status}", lojaId, status);
            throw;
        }
    }

    public async Task<IEnumerable<AgendamentoDto>> ObterPorPrestadorAsync(Guid prestadorId)
    {
        try
        {
            var agendamentos = await _agendamentoRepository.ObterPorPrestadorAsync(prestadorId);
            return _mapper.Map<IEnumerable<AgendamentoDto>>(agendamentos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter agendamentos do prestador {PrestadorId}", prestadorId);
            throw;
        }
    }

    public async Task<AgendamentoDto> ConfirmarAsync(Guid id, decimal? valor = null)
    {
        try
        {
            var agendamento = await _agendamentoRepository.ObterPorIdAsync(id);
            if (agendamento == null)
                throw new ArgumentException("Agendamento não encontrado");

            agendamento.Confirmar(valor);
            await _agendamentoRepository.AtualizarAsync(agendamento);
            await _agendamentoRepository.SalvarAsync();

            _logger.LogInformation("Agendamento {Id} confirmado", id);

            return await ObterPorIdAsync(id) ?? throw new InvalidOperationException("Erro ao recuperar agendamento confirmado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao confirmar agendamento {Id}", id);
            throw;
        }
    }

    public async Task<AgendamentoDto> CancelarAsync(Guid id, string motivo)
    {
        try
        {
            var agendamento = await _agendamentoRepository.ObterPorIdAsync(id);
            if (agendamento == null)
                throw new ArgumentException("Agendamento não encontrado");

            agendamento.Cancelar(motivo);
            await _agendamentoRepository.AtualizarAsync(agendamento);
            await _agendamentoRepository.SalvarAsync();

            _logger.LogInformation("Agendamento {Id} cancelado: {Motivo}", id, motivo);

            return await ObterPorIdAsync(id) ?? throw new InvalidOperationException("Erro ao recuperar agendamento cancelado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao cancelar agendamento {Id}", id);
            throw;
        }
    }

    public async Task<AgendamentoDto> ConcluirAsync(Guid id, decimal? valorFinal = null)
    {
        try
        {
            var agendamento = await _agendamentoRepository.ObterPorIdAsync(id);
            if (agendamento == null)
                throw new ArgumentException("Agendamento não encontrado");

            agendamento.Concluir(valorFinal);
            await _agendamentoRepository.AtualizarAsync(agendamento);
            await _agendamentoRepository.SalvarAsync();

            _logger.LogInformation("Agendamento {Id} concluído", id);

            return await ObterPorIdAsync(id) ?? throw new InvalidOperationException("Erro ao recuperar agendamento concluído");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao concluir agendamento {Id}", id);
            throw;
        }
    }

    public async Task<AgendamentoDto> AtualizarAsync(AtualizarAgendamentoDto dto)
    {
        try
        {
            var agendamento = await _agendamentoRepository.ObterPorIdAsync(dto.Id);
            if (agendamento == null)
                throw new ArgumentException("Agendamento não encontrado");

            if (dto.ClienteNome != null || dto.ClienteTelefone != null || dto.ClienteEmail != null)
            {
                agendamento.AtualizarCliente(
                    dto.ClienteNome ?? agendamento.ClienteNome,
                    dto.ClienteTelefone ?? agendamento.ClienteTelefone,
                    dto.ClienteEmail
                );
            }

            if (dto.Data.HasValue && dto.Hora.HasValue)
            {
                agendamento.AtualizarDataHora(dto.Data.Value, dto.Hora.Value);
            }

            if (dto.Observacoes != null)
            {
                agendamento.AtualizarObservacoes(dto.Observacoes);
            }

            await _agendamentoRepository.AtualizarAsync(agendamento);
            await _agendamentoRepository.SalvarAsync();

            _logger.LogInformation("Agendamento {Id} atualizado", dto.Id);

            return await ObterPorIdAsync(dto.Id) ?? throw new InvalidOperationException("Erro ao recuperar agendamento atualizado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar agendamento {Id}", dto.Id);
            throw;
        }
    }

    public async Task<bool> DesativarAsync(Guid id)
    {
        try
        {
            var agendamento = await _agendamentoRepository.ObterPorIdAsync(id);
            if (agendamento == null) return false;

            agendamento.Desativar();
            await _agendamentoRepository.AtualizarAsync(agendamento);
            await _agendamentoRepository.SalvarAsync();

            _logger.LogInformation("Agendamento {Id} desativado", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao desativar agendamento {Id}", id);
            throw;
        }
    }

    public async Task<bool> TemConflitosAsync(Guid lojaId, DateOnly data, TimeOnly hora, int duracaoMinutos, Guid? prestadorId = null, Guid? excludeId = null)
    {
        try
        {
            return await _agendamentoRepository.TemConflitosAsync(lojaId, data, hora, duracaoMinutos, prestadorId, excludeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar conflitos de agendamento");
            throw;
        }
    }

    public async Task<IEnumerable<AgendamentoDto>> ObterAgendamentosHojeAsync(Guid lojaId)
    {
        try
        {
            var agendamentos = await _agendamentoRepository.ObterAgendamentosHojeAsync(lojaId);
            return _mapper.Map<IEnumerable<AgendamentoDto>>(agendamentos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter agendamentos de hoje da loja {LojaId}", lojaId);
            throw;
        }
    }

    public async Task<IEnumerable<AgendamentoDto>> ObterAgendamentosProximosAsync(Guid lojaId)
    {
        try
        {
            var agendamentos = await _agendamentoRepository.ObterAgendamentosProximosAsync(lojaId);
            return _mapper.Map<IEnumerable<AgendamentoDto>>(agendamentos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter agendamentos próximos da loja {LojaId}", lojaId);
            throw;
        }
    }

    public async Task<IEnumerable<AgendamentoDto>> BuscarPorClienteAsync(Guid lojaId, string termo)
    {
        try
        {
            var agendamentos = await _agendamentoRepository.BuscarPorClienteAsync(lojaId, termo);
            return _mapper.Map<IEnumerable<AgendamentoDto>>(agendamentos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar agendamentos por cliente na loja {LojaId} com termo {Termo}", lojaId, termo);
            throw;
        }
    }

    public async Task<AgendamentoEstatisticasDto> ObterEstatisticasAsync(Guid lojaId, DateOnly dataInicio, DateOnly dataFim)
    {
        try
        {
            var estatisticas = await _agendamentoRepository.ObterEstatisticasAsync(lojaId, dataInicio, dataFim);
            return _mapper.Map<AgendamentoEstatisticasDto>(estatisticas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter estatísticas de agendamento da loja {LojaId}", lojaId);
            throw;
        }
    }
}
