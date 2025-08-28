using AgendaApp.Dominio.Entities;

namespace AgendaApp.Dominio.Interfaces;

public interface IAgendamentoRepository : IRepository<Agendamento>
{
    Task<IEnumerable<Agendamento>> ObterPorLojaAsync(Guid lojaId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Agendamento>> ObterPorDataAsync(Guid lojaId, DateOnly data, CancellationToken cancellationToken = default);

    Task<IEnumerable<Agendamento>> ObterPorPeriodoAsync(Guid lojaId, DateOnly dataInicio, DateOnly dataFim, CancellationToken cancellationToken = default);

    Task<IEnumerable<Agendamento>> ObterPorStatusAsync(Guid lojaId, StatusAgendamento status, CancellationToken cancellationToken = default);

    Task<IEnumerable<Agendamento>> ObterPorServicoAsync(Guid servicoId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Agendamento>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default);

    Task<bool> TemConflitosAsync(Guid lojaId, DateOnly data, TimeOnly hora, int duracaoMinutos, 
                                Guid? prestadorId = null, Guid? excludeId = null, CancellationToken cancellationToken = default);

    Task<IEnumerable<Agendamento>> ObterAgendamentosHojeAsync(Guid lojaId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Agendamento>> ObterAgendamentosProximosAsync(Guid lojaId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Agendamento>> BuscarPorClienteAsync(Guid lojaId, string termo, CancellationToken cancellationToken = default);

    Task<Agendamento?> ObterCompletoAsync(Guid id, CancellationToken cancellationToken = default);

    Task<AgendamentoEstatisticas> ObterEstatisticasAsync(Guid lojaId, DateOnly dataInicio, DateOnly dataFim, CancellationToken cancellationToken = default);
}