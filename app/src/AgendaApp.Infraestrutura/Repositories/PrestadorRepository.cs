using AgendaApp.Dominio.Entities;
using AgendaApp.Dominio.Interfaces;
using AgendaApp.Dominio.ValueObjects;
using AgendaApp.Infraestrutura.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AgendaApp.Infraestrutura.Repositories;

public class PrestadorRepository : IPrestadorRepository
{
    private readonly AgendaAppDbContext _context;

    public PrestadorRepository(AgendaAppDbContext context)
    {
        _context = context;
    }

    public async Task<Prestador?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Prestadores
            .Include(p => p.Servicos)
            .Include(p => p.HorariosDisponiveis)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Prestador>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Prestadores
            .Include(p => p.Servicos)
            .Include(p => p.HorariosDisponiveis)
            .OrderBy(p => p.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Prestador>> ObterPorCondicaoAsync(
        Expression<Func<Prestador, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Prestadores
            .Include(p => p.Servicos)
            .Include(p => p.HorariosDisponiveis)
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Prestador> AdicionarAsync(Prestador entity, CancellationToken cancellationToken = default)
    {
        _context.Prestadores.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<Prestador> AtualizarAsync(Prestador entity, CancellationToken cancellationToken = default)
    {
        _context.Prestadores.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await ObterPorIdAsync(id, cancellationToken);
        if (entity == null) return false;

        _context.Prestadores.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ExisteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Prestadores
            .AnyAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Prestador?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var emailObj = Email.Criar(email);
        return await _context.Prestadores
            .Include(p => p.Servicos)
            .Include(p => p.HorariosDisponiveis)
            .FirstOrDefaultAsync(p => p.Email == emailObj, cancellationToken);
    }

    public async Task<IEnumerable<Prestador>> ObterPorAreaAtuacaoAsync(string areaAtuacao, CancellationToken cancellationToken = default)
    {
        return await _context.Prestadores
            .Include(p => p.Servicos)
            .Include(p => p.HorariosDisponiveis)
            .Where(p => p.AreaAtuacao.Contains(areaAtuacao))
            .OrderBy(p => p.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Prestador>> ObterPorLojaAsync(Guid lojaId, CancellationToken cancellationToken = default)
    {
        return await _context.Prestadores
            .Include(p => p.Servicos)
            .Include(p => p.HorariosDisponiveis)
            .Where(p => p.LojaId == lojaId)
            .OrderBy(p => p.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Prestador>> ObterAtivosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Prestadores
            .Include(p => p.Servicos)
            .Include(p => p.HorariosDisponiveis)
            .Where(p => p.Ativo)
            .OrderBy(p => p.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Prestador>> ObterPorLojaEAtivosAsync(Guid lojaId, CancellationToken cancellationToken = default)
    {
        return await _context.Prestadores
            .Include(p => p.Servicos)
            .Include(p => p.HorariosDisponiveis)
            .Where(p => p.LojaId == lojaId && p.Ativo)
            .OrderBy(p => p.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<Prestador?> ObterComServicosAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Prestadores
            .Include(p => p.Servicos.Where(s => s.Ativo))
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Prestador?> ObterComHorariosAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Prestadores
            .Include(p => p.HorariosDisponiveis.Where(h => h.Ativo))
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Prestador?> ObterCompletoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Prestadores
            .Include(p => p.Servicos)
            .Include(p => p.HorariosDisponiveis)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<int> ContarPorLojaAsync(Guid lojaId, CancellationToken cancellationToken = default)
    {
        return await _context.Prestadores
            .CountAsync(p => p.LojaId == lojaId, cancellationToken);
    }

    public async Task<bool> ExistePorEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var emailObj = Email.Criar(email);
        return await _context.Prestadores
            .AnyAsync(p => p.Email == emailObj, cancellationToken);
    }

    public async Task<bool> ExistePorEmailAsync(string email, Guid excluirId, CancellationToken cancellationToken = default)
    {
        var emailObj = Email.Criar(email);
        return await _context.Prestadores
            .AnyAsync(p => p.Email == emailObj && p.Id != excluirId, cancellationToken);
    }

    public async Task<Prestador?> ObterPorCnpjAsync(string cnpj, CancellationToken cancellationToken = default)
    {
        var cnpjObj = Cnpj.Criar(cnpj);
        return await _context.Prestadores
            .Include(p => p.Servicos)
            .Include(p => p.HorariosDisponiveis)
            .FirstOrDefaultAsync(p => p.CNPJ != null && p.CNPJ == cnpjObj, cancellationToken);
    }

    public async Task<IEnumerable<Prestador>> BuscarPorNomeAsync(string nome, CancellationToken cancellationToken = default)
    {
        return await _context.Prestadores
            .Include(p => p.Servicos)
            .Include(p => p.HorariosDisponiveis)
            .Where(p => p.Nome.Contains(nome))
            .OrderBy(p => p.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> EmailJaExisteAsync(string email, Guid? prestadorId = null, CancellationToken cancellationToken = default)
    {
        var emailObj = Email.Criar(email);
        
        if (prestadorId.HasValue)
        {
            return await _context.Prestadores
                .AnyAsync(p => p.Email == emailObj && p.Id != prestadorId.Value, cancellationToken);
        }
        
        return await _context.Prestadores
            .AnyAsync(p => p.Email == emailObj, cancellationToken);
    }

    public async Task<bool> CnpjJaExisteAsync(string cnpj, Guid? prestadorId = null, CancellationToken cancellationToken = default)
    {
        var cnpjObj = Cnpj.Criar(cnpj);
        
        if (prestadorId.HasValue)
        {
            return await _context.Prestadores
                .AnyAsync(p => p.CNPJ != null && p.CNPJ == cnpjObj && p.Id != prestadorId.Value, cancellationToken);
        }
        
        return await _context.Prestadores
            .AnyAsync(p => p.CNPJ != null && p.CNPJ == cnpjObj, cancellationToken);
    }

    public async Task<int> SalvarAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
} 