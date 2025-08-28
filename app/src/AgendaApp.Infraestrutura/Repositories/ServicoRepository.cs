using AgendaApp.Dominio.Entities;
using AgendaApp.Dominio.Interfaces;
using AgendaApp.Infraestrutura.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AgendaApp.Infraestrutura.Repositories;

public class ServicoRepository : IServicoRepository
{
    private readonly AgendaAppDbContext _context;

    public ServicoRepository(AgendaAppDbContext context)
    {
        _context = context;
    }

    public async Task<Servico?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Servicos
            .Include(s => s.Loja)
            .Include(s => s.PrestadorServicos)
                .ThenInclude(ps => ps.Prestador)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Servico>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Servicos
            .Include(s => s.Loja)
            .Include(s => s.PrestadorServicos)
                .ThenInclude(ps => ps.Prestador)
            .Where(s => s.Ativo)
            .OrderBy(s => s.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Servico>> ObterPorCondicaoAsync(
        Expression<Func<Servico, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Servicos
            .Include(s => s.Loja)
            .Include(s => s.PrestadorServicos)
                .ThenInclude(ps => ps.Prestador)
            .Where(predicate)
            .Where(s => s.Ativo)
            .OrderBy(s => s.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<Servico> AdicionarAsync(Servico entity, CancellationToken cancellationToken = default)
    {
        await _context.Servicos.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task<Servico> AtualizarAsync(Servico entity, CancellationToken cancellationToken = default)
    {
        _context.Servicos.Update(entity);
        return await Task.FromResult(entity);
    }

    public async Task<bool> RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var servico = await _context.Servicos.FindAsync(new object[] { id }, cancellationToken);
        if (servico == null) return false;

        servico.Desativar();
        return true;
    }

    public async Task<int> SalvarAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Servico>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default)
    {
        return await _context.Servicos
            .Include(s => s.Loja)
            .Include(s => s.PrestadorServicos)
                .ThenInclude(ps => ps.Prestador)
            .Where(s => s.PrestadorServicos.Any(ps => ps.PrestadorId == prestadorId) && s.Ativo)
            .OrderBy(s => s.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Servico>> ObterPorFaixaPrecoAsync(decimal precoMinimo, decimal precoMaximo, CancellationToken cancellationToken = default)
    {
        return await _context.Servicos
            .Include(s => s.Loja)
            .Include(s => s.PrestadorServicos)
                .ThenInclude(ps => ps.Prestador)
            .Where(s => s.Valor >= precoMinimo && s.Valor <= precoMaximo && s.Ativo)
            .OrderBy(s => s.Valor)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Servico>> ObterPorDuracaoAsync(int duracaoMinima, int duracaoMaxima, CancellationToken cancellationToken = default)
    {
        return await _context.Servicos
            .Include(s => s.Loja)
            .Include(s => s.PrestadorServicos)
                .ThenInclude(ps => ps.Prestador)
            .Where(s => s.DuracaoEmMinutos >= duracaoMinima && s.DuracaoEmMinutos <= duracaoMaxima && s.Ativo)
            .OrderBy(s => s.DuracaoEmMinutos)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Servico>> BuscarPorNomeAsync(string nome, CancellationToken cancellationToken = default)
    {
        return await _context.Servicos
            .Include(s => s.Loja)
            .Include(s => s.PrestadorServicos)
                .ThenInclude(ps => ps.Prestador)
            .Where(s => s.Nome.Contains(nome) && s.Ativo)
            .OrderBy(s => s.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Servico>> ObterAtivosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Servicos
            .Include(s => s.Loja)
            .Include(s => s.PrestadorServicos)
                .ThenInclude(ps => ps.Prestador)
            .Where(s => s.Ativo)
            .OrderBy(s => s.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Servico>> ObterPorPrestadorEAtivosAsync(Guid prestadorId, CancellationToken cancellationToken = default)
    {
        return await _context.Servicos
            .Include(s => s.Loja)
            .Include(s => s.PrestadorServicos)
                .ThenInclude(ps => ps.Prestador)
            .Where(s => s.PrestadorServicos.Any(ps => ps.PrestadorId == prestadorId) && s.Ativo)
            .OrderBy(s => s.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> ObterValorMedioAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Servicos
            .Where(s => s.Ativo)
            .AverageAsync(s => s.Valor, cancellationToken);
    }

    public async Task<int> ContarServicosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Servicos
            .CountAsync(s => s.Ativo, cancellationToken);
    }

    public async Task<int> ContarPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default)
    {
        return await _context.Servicos
            .CountAsync(s => s.PrestadorServicos.Any(ps => ps.PrestadorId == prestadorId) && s.Ativo, cancellationToken);
    }

    public async Task<bool> ExisteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Servicos
            .AnyAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Servico>> ObterMaisCarosAsync(int quantidade, CancellationToken cancellationToken = default)
    {
        return await _context.Servicos
            .Include(s => s.Loja)
            .Include(s => s.PrestadorServicos)
                .ThenInclude(ps => ps.Prestador)
            .Where(s => s.Ativo)
            .OrderByDescending(s => s.Valor)
            .Take(quantidade)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Servico>> ObterMaisRapidosAsync(int quantidade, CancellationToken cancellationToken = default)
    {
        return await _context.Servicos
            .Include(s => s.Loja)
            .Include(s => s.PrestadorServicos)
                .ThenInclude(ps => ps.Prestador)
            .Where(s => s.Ativo)
            .OrderBy(s => s.DuracaoEmMinutos)
            .Take(quantidade)
            .ToListAsync(cancellationToken);
    }

    // Métodos faltantes da interface IServicoRepository
    public async Task<IEnumerable<Servico>> ObterPorFaixaValorAsync(decimal valorMinimo, decimal valorMaximo, CancellationToken cancellationToken = default)
    {
        return await _context.Servicos
            .Include(s => s.Loja)
            .Include(s => s.PrestadorServicos)
                .ThenInclude(ps => ps.Prestador)
            .Where(s => s.Valor >= valorMinimo && s.Valor <= valorMaximo && s.Ativo)
            .OrderBy(s => s.Valor)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Servico>> ObterPorDuracaoMaximaAsync(int duracaoMaximaMinutos, CancellationToken cancellationToken = default)
    {
        return await _context.Servicos
            .Include(s => s.Loja)
            .Include(s => s.PrestadorServicos)
                .ThenInclude(ps => ps.Prestador)
            .Where(s => s.DuracaoEmMinutos <= duracaoMaximaMinutos && s.Ativo)
            .OrderBy(s => s.DuracaoEmMinutos)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Servico>> ObterComPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default)
    {
        return await _context.Servicos
            .Include(s => s.Loja)
            .Include(s => s.PrestadorServicos)
                .ThenInclude(ps => ps.Prestador)
            .Where(s => s.PrestadorServicos.Any(ps => ps.PrestadorId == prestadorId) && s.Ativo)
            .OrderBy(s => s.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> NomeJaExisteParaPrestadorAsync(string nome, Guid prestadorId, Guid? servicoId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Servicos
            .Where(s => s.Nome == nome && s.PrestadorServicos.Any(ps => ps.PrestadorId == prestadorId));
        
        if (servicoId.HasValue)
        {
            query = query.Where(s => s.Id != servicoId.Value);
        }
        
        return await query.AnyAsync(cancellationToken);
    }

    public async Task<ServicosEstatisticas> ObterEstatisticasAsync(Guid? prestadorId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Servicos.Where(s => s.Ativo);
        
        if (prestadorId.HasValue)
        {
            query = query.Where(s => s.PrestadorServicos.Any(ps => ps.PrestadorId == prestadorId.Value));
        }
        
        var servicos = await query.ToListAsync(cancellationToken);
        
        if (!servicos.Any())
        {
            return new ServicosEstatisticas
            {
                TotalServicos = 0,
                ValorMedio = 0,
                ValorMinimo = 0,
                ValorMaximo = 0,
                DuracaoMediaMinutos = 0
            };
        }
        
        return new ServicosEstatisticas
        {
            TotalServicos = servicos.Count,
            ValorMedio = servicos.Average(s => s.Valor),
            ValorMinimo = servicos.Min(s => s.Valor),
            ValorMaximo = servicos.Max(s => s.Valor),
            DuracaoMediaMinutos = (int)servicos.Average(s => s.DuracaoEmMinutos)
        };
    }
} 