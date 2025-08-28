using AgendaApp.Dominio.Entities;
using AgendaApp.Dominio.Interfaces;
using AgendaApp.Dominio.ValueObjects;
using AgendaApp.Infraestrutura.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AgendaApp.Infraestrutura.Repositories;

public class LojaRepository : ILojaRepository
{
    private readonly AgendaAppDbContext _context;

    public LojaRepository(AgendaAppDbContext context)
    {
        _context = context;
    }

    // Implementação da interface base IRepository<T>
    public async Task<Loja?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Lojas
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Loja>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Lojas
            .OrderBy(l => l.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Loja>> ObterPorCondicaoAsync(
        Expression<Func<Loja, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Lojas
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Loja> AdicionarAsync(Loja entity, CancellationToken cancellationToken = default)
    {
        _context.Lojas.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<Loja> AtualizarAsync(Loja entity, CancellationToken cancellationToken = default)
    {
        _context.Lojas.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var loja = await ObterPorIdAsync(id, cancellationToken);
        if (loja == null)
            return false;

        _context.Lojas.Remove(loja);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<int> SalvarAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    // Implementação da interface específica ILojaRepository
    public async Task<Loja?> ObterPorSlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _context.Lojas
            .FirstOrDefaultAsync(l => l.Slug == slug, cancellationToken);
    }

    public async Task<bool> SlugJaExisteAsync(string slug, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Lojas.Where(l => l.Slug == slug);
        
        if (excludeId.HasValue)
            query = query.Where(l => l.Id != excludeId.Value);
            
        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> CnpjJaExisteAsync(string cnpj, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var cnpjObj = Cnpj.Criar(cnpj);
        var query = _context.Lojas.Where(l => l.CNPJ == cnpjObj);
        
        if (excludeId.HasValue)
            query = query.Where(l => l.Id != excludeId.Value);
            
        return await query.AnyAsync(cancellationToken);
    }

    public async Task<IEnumerable<Loja>> ObterLojasVencidasAsync(CancellationToken cancellationToken = default)
    {
        var dataAtual = DateTime.UtcNow;
        return await _context.Lojas
            .Where(l => l.DataVencimento < dataAtual)
            .OrderBy(l => l.DataVencimento)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Loja>> ObterLojasProximasVencimentoAsync(
        int diasAntecedencia = 7, 
        CancellationToken cancellationToken = default)
    {
        var dataLimite = DateTime.UtcNow.AddDays(diasAntecedencia);
        var dataAtual = DateTime.UtcNow;
        
        return await _context.Lojas
            .Where(l => l.DataVencimento >= dataAtual && l.DataVencimento <= dataLimite)
            .OrderBy(l => l.DataVencimento)
            .ToListAsync(cancellationToken);
    }

    public async Task<Loja?> ObterComUsuariosAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Método removido - usamos ApplicationUser do Identity agora
        return await ObterPorIdAsync(id, cancellationToken);
    }

    public async Task<Loja?> ObterCompletaAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Lojas
            .Include(l => l.Servicos)
            .Include(l => l.HorariosDisponiveis)
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }
} 