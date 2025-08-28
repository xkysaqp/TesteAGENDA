using AgendaApp.Dominio.Entities;
using AgendaApp.Dominio.Enums;
using AgendaApp.Dominio.Interfaces;
using AgendaApp.Infraestrutura.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AgendaApp.Infraestrutura.Repositories;

public class HorarioDisponivelRepository : IHorarioDisponivelRepository
{
    private readonly AgendaAppDbContext _context;

    public HorarioDisponivelRepository(AgendaAppDbContext context)
    {
        _context = context;
    }

    // Implementação da interface base IRepository<T>
    public async Task<HorarioDisponivel?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.HorariosDisponiveis
            .Include(h => h.Prestador)
            .Include(h => h.Loja)
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<HorarioDisponivel>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.HorariosDisponiveis
            .Include(h => h.Prestador)
            .Include(h => h.Loja)
            .OrderBy(h => h.DiaSemana)
            .ThenBy(h => h.HoraInicio)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<HorarioDisponivel>> ObterPorCondicaoAsync(
        Expression<Func<HorarioDisponivel, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        return await _context.HorariosDisponiveis
            .Include(h => h.Prestador)
            .Include(h => h.Loja)
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public async Task<HorarioDisponivel> AdicionarAsync(HorarioDisponivel entity, CancellationToken cancellationToken = default)
    {
        _context.HorariosDisponiveis.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<HorarioDisponivel> AtualizarAsync(HorarioDisponivel entity, CancellationToken cancellationToken = default)
    {
        _context.HorariosDisponiveis.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var horario = await ObterPorIdAsync(id, cancellationToken);
        if (horario == null)
            return false;

        _context.HorariosDisponiveis.Remove(horario);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<int> SalvarAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    // Implementação da interface específica IHorarioDisponivelRepository
    public async Task<IEnumerable<HorarioDisponivel>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default)
    {
        return await _context.HorariosDisponiveis
            .Include(h => h.Prestador)
            .Where(h => h.PrestadorId == prestadorId)
            .OrderBy(h => h.DiaSemana)
            .ThenBy(h => h.HoraInicio)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<HorarioDisponivel>> ObterPorPrestadorEDiaAsync(
        Guid prestadorId, 
        DiaSemana diaSemana, 
        CancellationToken cancellationToken = default)
    {
        return await _context.HorariosDisponiveis
            .Include(h => h.Prestador)
            .Where(h => h.PrestadorId == prestadorId && h.DiaSemana == diaSemana)
            .OrderBy(h => h.HoraInicio)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<HorarioDisponivel>> ObterDisponivelParaPeriodoAsync(
        Guid prestadorId, 
        DiaSemana diaSemana, 
        TimeOnly horaInicio, 
        TimeOnly horaFim, 
        CancellationToken cancellationToken = default)
    {
        return await _context.HorariosDisponiveis
            .Include(h => h.Prestador)
            .Where(h => h.PrestadorId == prestadorId && 
                       h.DiaSemana == diaSemana &&
                       h.HoraInicio <= horaInicio && 
                       h.HoraFim >= horaFim &&
                       h.Ativo)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExisteConflitoAsync(
        Guid prestadorId, 
        DiaSemana diaSemana, 
        TimeOnly horaInicio, 
        TimeOnly horaFim, 
        Guid? horarioId = null, 
        CancellationToken cancellationToken = default)
    {
        var query = _context.HorariosDisponiveis
            .Where(h => h.PrestadorId == prestadorId && 
                       h.DiaSemana == diaSemana &&
                       h.Ativo &&
                       // Verificar sobreposição de horários
                       (horaInicio < h.HoraFim && horaFim > h.HoraInicio));

        if (horarioId.HasValue)
            query = query.Where(h => h.Id != horarioId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<IEnumerable<HorarioDisponivel>> ObterOrdenadosCronologicamenteAsync(
        Guid prestadorId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.HorariosDisponiveis
            .Include(h => h.Prestador)
            .Where(h => h.PrestadorId == prestadorId)
            .OrderBy(h => h.DiaSemana)
            .ThenBy(h => h.HoraInicio)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<HorarioDisponivel>> ObterPorFaixaHorarioAsync(
        TimeOnly horaInicioMinima, 
        TimeOnly horaFimMaxima, 
        CancellationToken cancellationToken = default)
    {
        return await _context.HorariosDisponiveis
            .Include(h => h.Prestador)
            .Where(h => h.HoraInicio >= horaInicioMinima && h.HoraFim <= horaFimMaxima)
            .OrderBy(h => h.DiaSemana)
            .ThenBy(h => h.HoraInicio)
            .ToListAsync(cancellationToken);
    }

    public async Task<HorariosEstatisticas> ObterEstatisticasAsync(Guid? prestadorId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.HorariosDisponiveis.AsQueryable();
        
        if (prestadorId.HasValue)
            query = query.Where(h => h.PrestadorId == prestadorId.Value);

        var horarios = await query.ToListAsync(cancellationToken);

        var estatisticas = new HorariosEstatisticas
        {
            TotalHorarios = horarios.Count,
            HorariosPorDia = horarios.Count > 0 ? horarios.Count / 7 : 0,
            DuracaoMediaMinutos = horarios.Count > 0 ? (int)horarios.Average(h => h.DuracaoEmMinutos()) : 0,
            HorariosPorDiaSemana = new Dictionary<DiaSemana, int>()
        };

        if (horarios.Any())
        {
            estatisticas.HorarioInicioMaisEarly = horarios.Min(h => h.HoraInicio);
            estatisticas.HorarioFimMaisTarde = horarios.Max(h => h.HoraFim);

            foreach (DiaSemana dia in Enum.GetValues<DiaSemana>())
            {
                estatisticas.HorariosPorDiaSemana[dia] = horarios.Count(h => h.DiaSemana == dia);
            }
        }

        return estatisticas;
    }

    public async Task<int> RemoverTodosPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default)
    {
        var horarios = await _context.HorariosDisponiveis
            .Where(h => h.PrestadorId == prestadorId)
            .ToListAsync(cancellationToken);

        _context.HorariosDisponiveis.RemoveRange(horarios);
        await _context.SaveChangesAsync(cancellationToken);
        
        return horarios.Count;
    }
} 