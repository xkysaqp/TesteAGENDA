using AgendaApp.Dominio.Entities;
using AgendaApp.Dominio.Enums;

namespace AgendaApp.Dominio.Interfaces;

public interface IHorarioDisponivelRepository : IRepository<HorarioDisponivel>
{
    Task<IEnumerable<HorarioDisponivel>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default);

    Task<IEnumerable<HorarioDisponivel>> ObterPorPrestadorEDiaAsync(Guid prestadorId, DiaSemana diaSemana, CancellationToken cancellationToken = default);

    Task<IEnumerable<HorarioDisponivel>> ObterDisponivelParaPeriodoAsync(
        Guid prestadorId, 
        DiaSemana diaSemana, 
        TimeOnly horaInicio, 
        TimeOnly horaFim, 
        CancellationToken cancellationToken = default);

    Task<bool> ExisteConflitoAsync(
        Guid prestadorId, 
        DiaSemana diaSemana, 
        TimeOnly horaInicio, 
        TimeOnly horaFim, 
        Guid? horarioId = null, 
        CancellationToken cancellationToken = default);

    Task<IEnumerable<HorarioDisponivel>> ObterOrdenadosCronologicamenteAsync(Guid prestadorId, CancellationToken cancellationToken = default);

    Task<IEnumerable<HorarioDisponivel>> ObterPorFaixaHorarioAsync(
        TimeOnly horaInicioMinima, 
        TimeOnly horaFimMaxima, 
        CancellationToken cancellationToken = default);

    Task<HorariosEstatisticas> ObterEstatisticasAsync(Guid? prestadorId = null, CancellationToken cancellationToken = default);

    Task<int> RemoverTodosPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default);
}

public class HorariosEstatisticas
{
    public int TotalHorarios { get; set; }
    public int HorariosPorDia { get; set; }
    public TimeOnly HorarioInicioMaisEarly { get; set; }
    public TimeOnly HorarioFimMaisTarde { get; set; }
    public int DuracaoMediaMinutos { get; set; }
    public Dictionary<DiaSemana, int> HorariosPorDiaSemana { get; set; } = new();
} 