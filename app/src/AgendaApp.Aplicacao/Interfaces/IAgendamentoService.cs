using AgendaApp.Aplicacao.DTOs;
using AgendaApp.Dominio.Entities;

namespace AgendaApp.Aplicacao.Interfaces;

public interface IAgendamentoService
{
    Task<AgendamentoDto> CriarAsync(CriarAgendamentoDto dto);
    Task<AgendamentoDto?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<AgendamentoDto>> ObterPorLojaAsync(Guid lojaId);
    Task<IEnumerable<AgendamentoDto>> ObterPorDataAsync(Guid lojaId, DateOnly data);
    Task<IEnumerable<AgendamentoDto>> ObterPorPeriodoAsync(Guid lojaId, DateOnly dataInicio, DateOnly dataFim);
    Task<IEnumerable<AgendamentoDto>> ObterPorStatusAsync(Guid lojaId, StatusAgendamento status);
    Task<IEnumerable<AgendamentoDto>> ObterPorPrestadorAsync(Guid prestadorId);
    Task<AgendamentoDto> ConfirmarAsync(Guid id, decimal? valor = null);
    Task<AgendamentoDto> CancelarAsync(Guid id, string motivo);
    Task<AgendamentoDto> ConcluirAsync(Guid id, decimal? valorFinal = null);
    Task<AgendamentoDto> AtualizarAsync(AtualizarAgendamentoDto dto);
    Task<bool> DesativarAsync(Guid id);
    Task<bool> TemConflitosAsync(Guid lojaId, DateOnly data, TimeOnly hora, int duracaoMinutos, Guid? prestadorId = null, Guid? excludeId = null);
    Task<IEnumerable<AgendamentoDto>> ObterAgendamentosHojeAsync(Guid lojaId);
    Task<IEnumerable<AgendamentoDto>> ObterAgendamentosProximosAsync(Guid lojaId);
    Task<IEnumerable<AgendamentoDto>> BuscarPorClienteAsync(Guid lojaId, string termo);
    Task<AgendamentoEstatisticasDto> ObterEstatisticasAsync(Guid lojaId, DateOnly dataInicio, DateOnly dataFim);
}
