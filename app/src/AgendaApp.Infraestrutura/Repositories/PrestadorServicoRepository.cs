using AgendaApp.Dominio.Entities;
using AgendaApp.Dominio.Interfaces;
using AgendaApp.Infraestrutura.Data;
using Microsoft.EntityFrameworkCore;

namespace AgendaApp.Infraestrutura.Repositories;

public class PrestadorServicoRepository : IRepository<PrestadorServico>
{
    private readonly AgendaAppDbContext _context;

    public PrestadorServicoRepository(AgendaAppDbContext context)
    {
        _context = context;
    }

    public async Task<PrestadorServico?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.PrestadorServicos
            .Include(ps => ps.Prestador)
            .Include(ps => ps.Servico)
            .FirstOrDefaultAsync(ps => ps.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<PrestadorServico>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PrestadorServicos
            .Include(ps => ps.Prestador)
            .Include(ps => ps.Servico)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PrestadorServico>> ObterPorCondicaoAsync(System.Linq.Expressions.Expression<Func<PrestadorServico, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.PrestadorServicos
            .Include(ps => ps.Prestador)
            .Include(ps => ps.Servico)
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public async Task<PrestadorServico> AdicionarAsync(PrestadorServico entidade, CancellationToken cancellationToken = default)
    {
        await _context.PrestadorServicos.AddAsync(entidade, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entidade;
    }

    public async Task<PrestadorServico> AtualizarAsync(PrestadorServico entidade, CancellationToken cancellationToken = default)
    {
        _context.PrestadorServicos.Update(entidade);
        await _context.SaveChangesAsync(cancellationToken);
        return entidade;
    }

    public async Task<bool> RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entidade = await ObterPorIdAsync(id, cancellationToken);
        if (entidade == null) return false;

        _context.PrestadorServicos.Remove(entidade);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ExisteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.PrestadorServicos
            .AnyAsync(ps => ps.Id == id, cancellationToken);
    }

    public async Task<int> ContarAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PrestadorServicos.CountAsync(cancellationToken);
    }

    public async Task<int> SalvarAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
} 