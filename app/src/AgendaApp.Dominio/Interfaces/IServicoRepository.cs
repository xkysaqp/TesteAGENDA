using AgendaApp.Dominio.Entities;

namespace AgendaApp.Dominio.Interfaces;

public interface IServicoRepository : IRepository<Servico>
{
    Task<IEnumerable<Servico>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Servico>> BuscarPorNomeAsync(string nome, CancellationToken cancellationToken = default);

    Task<IEnumerable<Servico>> ObterPorFaixaValorAsync(decimal valorMinimo, decimal valorMaximo, CancellationToken cancellationToken = default);

    Task<IEnumerable<Servico>> ObterPorDuracaoMaximaAsync(int duracaoMaximaMinutos, CancellationToken cancellationToken = default);

    Task<IEnumerable<Servico>> ObterComPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default);

    Task<bool> NomeJaExisteParaPrestadorAsync(string nome, Guid prestadorId, Guid? servicoId = null, CancellationToken cancellationToken = default);

    Task<ServicosEstatisticas> ObterEstatisticasAsync(Guid? prestadorId = null, CancellationToken cancellationToken = default);
}

public class ServicosEstatisticas
{
    public int TotalServicos { get; set; }
    public decimal ValorMedio { get; set; }
    public decimal ValorMinimo { get; set; }
    public decimal ValorMaximo { get; set; }
    public int DuracaoMediaMinutos { get; set; }
} 