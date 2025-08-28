using AgendaApp.Dominio.Entities;
using AgendaApp.Dominio.ValueObjects;

namespace AgendaApp.Dominio.Interfaces;

public interface IPrestadorRepository : IRepository<Prestador>
{
    Task<Prestador?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<Prestador?> ObterPorCnpjAsync(string cnpj, CancellationToken cancellationToken = default);

    Task<IEnumerable<Prestador>> ObterPorAreaAtuacaoAsync(string areaAtuacao, CancellationToken cancellationToken = default);

    Task<IEnumerable<Prestador>> BuscarPorNomeAsync(string nome, CancellationToken cancellationToken = default);

    Task<Prestador?> ObterComServicosAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Prestador?> ObterComHorariosAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Prestador?> ObterCompletoAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> EmailJaExisteAsync(string email, Guid? prestadorId = null, CancellationToken cancellationToken = default);

    Task<bool> CnpjJaExisteAsync(string cnpj, Guid? prestadorId = null, CancellationToken cancellationToken = default);
} 