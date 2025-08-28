using Microsoft.EntityFrameworkCore;
using AgendaApp.Dominio.Entities;
using AgendaApp.Dominio.Interfaces;
using AgendaApp.Infraestrutura.Data;

namespace AgendaApp.Infraestrutura.Repositories;

public class AgendamentoRepository : IAgendamentoRepository
{
    private readonly AgendaAppDbContext _context;

    public AgendamentoRepository(AgendaAppDbContext context)
    {
        _context = context;
    }

    public async Task<Agendamento> AdicionarAsync(Agendamento entidade, CancellationToken cancellationToken = default)
    {
        var entrada = await _context.Agendamentos.AddAsync(entidade, cancellationToken);
        return entrada.Entity;
    }

    public async Task<Agendamento> AtualizarAsync(Agendamento entidade, CancellationToken cancellationToken = default)
    {
        _context.Agendamentos.Update(entidade);
        return await Task.FromResult(entidade);
    }

    public async Task<bool> ExcluirAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var agendamento = await ObterPorIdAsync(id, cancellationToken);
        if (agendamento == null) return false;

        _context.Agendamentos.Remove(agendamento);
        return true;
    }

    public async Task<bool> RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await ExcluirAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Agendamento>> ObterPorCondicaoAsync(System.Linq.Expressions.Expression<Func<Agendamento, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .Include(a => a.Servico)
            .Include(a => a.Prestador)
            .Where(predicate)
            .OrderBy(a => a.Data)
            .ThenBy(a => a.Hora)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Agendamento>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .Include(a => a.Servico)
            .Include(a => a.Prestador)
            .OrderBy(a => a.Data)
            .ThenBy(a => a.Hora)
            .ToListAsync(cancellationToken);
    }

    public async Task<Agendamento?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .Include(a => a.Servico)
            .Include(a => a.Prestador)
            .Include(a => a.Loja)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<int> SalvarAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Agendamento>> ObterPorLojaAsync(Guid lojaId, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .Include(a => a.Servico)
            .Include(a => a.Prestador)
            .Where(a => a.LojaId == lojaId)
            .OrderBy(a => a.Data)
            .ThenBy(a => a.Hora)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Agendamento>> ObterPorDataAsync(Guid lojaId, DateOnly data, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .Include(a => a.Servico)
            .Include(a => a.Prestador)
            .Where(a => a.LojaId == lojaId && a.Data == data)
            .OrderBy(a => a.Hora)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Agendamento>> ObterPorPeriodoAsync(Guid lojaId, DateOnly dataInicio, DateOnly dataFim, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .Include(a => a.Servico)
            .Include(a => a.Prestador)
            .Where(a => a.LojaId == lojaId && a.Data >= dataInicio && a.Data <= dataFim)
            .OrderBy(a => a.Data)
            .ThenBy(a => a.Hora)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Agendamento>> ObterPorStatusAsync(Guid lojaId, StatusAgendamento status, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .Include(a => a.Servico)
            .Include(a => a.Prestador)
            .Where(a => a.LojaId == lojaId && a.Status == status)
            .OrderBy(a => a.Data)
            .ThenBy(a => a.Hora)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Agendamento>> ObterPorServicoAsync(Guid servicoId, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .Include(a => a.Servico)
            .Include(a => a.Prestador)
            .Include(a => a.Loja)
            .Where(a => a.ServicoId == servicoId)
            .OrderBy(a => a.Data)
            .ThenBy(a => a.Hora)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Agendamento>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .Include(a => a.Servico)
            .Include(a => a.Prestador)
            .Include(a => a.Loja)
            .Where(a => a.PrestadorId == prestadorId)
            .OrderBy(a => a.Data)
            .ThenBy(a => a.Hora)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> TemConflitosAsync(Guid lojaId, DateOnly data, TimeOnly hora, int duracaoMinutos, 
                                             Guid? prestadorId = null, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var horaFim = hora.AddMinutes(duracaoMinutos);
        
        var query = _context.Agendamentos
            .Where(a => a.LojaId == lojaId && 
                        a.Data == data && 
                        a.Status != StatusAgendamento.Cancelado);

        if (prestadorId.HasValue)
        {
            query = query.Where(a => a.PrestadorId == prestadorId);
        }

        if (excludeId.HasValue)
        {
            query = query.Where(a => a.Id != excludeId);
        }

        var agendamentosConflitantes = await query
            .Where(a => 
                (hora >= a.Hora && hora < a.Hora.AddMinutes(60)) ||
                (horaFim > a.Hora && horaFim <= a.Hora.AddMinutes(60)) ||
                (hora <= a.Hora && horaFim >= a.Hora.AddMinutes(60)))
            .AnyAsync(cancellationToken);

        return agendamentosConflitantes;
    }

    public async Task<IEnumerable<Agendamento>> ObterAgendamentosHojeAsync(Guid lojaId, CancellationToken cancellationToken = default)
    {
        var hoje = DateOnly.FromDateTime(DateTime.Today);
        return await ObterPorDataAsync(lojaId, hoje, cancellationToken);
    }

    public async Task<IEnumerable<Agendamento>> ObterAgendamentosProximosAsync(Guid lojaId, CancellationToken cancellationToken = default)
    {
        var agora = DateTime.Now;
        var dataHoje = DateOnly.FromDateTime(agora);
        var horaAtual = TimeOnly.FromDateTime(agora);
        var limiteHora = horaAtual.AddHours(2);

        return await _context.Agendamentos
            .Include(a => a.Servico)
            .Include(a => a.Prestador)
            .Where(a => a.LojaId == lojaId && 
                        a.Data == dataHoje && 
                        a.Hora >= horaAtual && 
                        a.Hora <= limiteHora &&
                        a.Status != StatusAgendamento.Cancelado)
            .OrderBy(a => a.Hora)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Agendamento>> BuscarPorClienteAsync(Guid lojaId, string termo, CancellationToken cancellationToken = default)
    {
        var termoLower = termo.ToLowerInvariant();
        
        return await _context.Agendamentos
            .Include(a => a.Servico)
            .Include(a => a.Prestador)
            .Where(a => a.LojaId == lojaId && 
                        (a.ClienteNome.ToLower().Contains(termoLower) ||
                         a.ClienteTelefone.Contains(termo)))
            .OrderByDescending(a => a.Data)
            .ThenBy(a => a.Hora)
            .ToListAsync(cancellationToken);
    }

    public async Task<Agendamento?> ObterCompletoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .Include(a => a.Loja)
            .Include(a => a.Servico)
            .Include(a => a.Prestador)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<AgendamentoEstatisticas> ObterEstatisticasAsync(Guid lojaId, DateOnly dataInicio, DateOnly dataFim, CancellationToken cancellationToken = default)
    {
        var agendamentos = await _context.Agendamentos
            .Where(a => a.LojaId == lojaId && a.Data >= dataInicio && a.Data <= dataFim)
            .ToListAsync(cancellationToken);

        var estatisticas = new AgendamentoEstatisticas
        {
            TotalAgendamentos = agendamentos.Count,
            AgendamentosPendentes = agendamentos.Count(a => a.Status == StatusAgendamento.Pendente),
            AgendamentosConfirmados = agendamentos.Count(a => a.Status == StatusAgendamento.Confirmado),
            AgendamentosCancelados = agendamentos.Count(a => a.Status == StatusAgendamento.Cancelado),
            AgendamentosConcluidos = agendamentos.Count(a => a.Status == StatusAgendamento.Concluido)
        };

        var agendamentosComValor = agendamentos.Where(a => a.Valor.HasValue).ToList();
        if (agendamentosComValor.Any())
        {
            estatisticas.ReceitaTotal = agendamentosComValor.Sum(a => a.Valor!.Value);
            estatisticas.ReceitaMedia = estatisticas.ReceitaTotal / agendamentosComValor.Count;
        }

        return estatisticas;
    }
} 