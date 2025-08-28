using AgendaApp.Dominio.Entities;
using System.Linq.Expressions;

namespace AgendaApp.Dominio.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<T>> ObterTodosAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<T>> ObterPorCondicaoAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    Task<T> AdicionarAsync(T entity, CancellationToken cancellationToken = default);

    Task<T> AtualizarAsync(T entity, CancellationToken cancellationToken = default);

    Task<bool> RemoverAsync(Guid id, CancellationToken cancellationToken = default);

    Task<int> SalvarAsync(CancellationToken cancellationToken = default);
} 