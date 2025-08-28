using AgendaApp.Dominio.Entities;

namespace AgendaApp.Dominio.Interfaces;

public interface ILojaRepository : IRepository<Loja>
{
    Task<Loja?> ObterPorSlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<bool> SlugJaExisteAsync(string slug, Guid? excludeId = null, CancellationToken cancellationToken = default);

    Task<bool> CnpjJaExisteAsync(string cnpj, Guid? excludeId = null, CancellationToken cancellationToken = default);

    Task<IEnumerable<Loja>> ObterLojasVencidasAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<Loja>> ObterLojasProximasVencimentoAsync(int diasAntecedencia = 7, CancellationToken cancellationToken = default);

    Task<Loja?> ObterComUsuariosAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Loja?> ObterCompletaAsync(Guid id, CancellationToken cancellationToken = default);
} 