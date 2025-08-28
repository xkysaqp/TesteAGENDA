using AgendaApp.Aplicacao.DTOs;

namespace AgendaApp.Aplicacao.Interfaces;

public interface IPrestadorServicoService
{
    Task<IEnumerable<ServicoGlobalDisponivelDto>> ObterServicosGlobaisDisponiveisAsync(Guid prestadorId, CancellationToken cancellationToken = default);

    Task<IEnumerable<ServicoVinculadoDto>> ObterServicosVinculadosAsync(Guid prestadorId, CancellationToken cancellationToken = default);

    Task<ServicoVinculadoDto> VincularServicoAsync(VincularServicoDto dto, string? usuarioExecutante = null, CancellationToken cancellationToken = default);

    Task<ServicoVinculadoDto> AtualizarServicoVinculadoAsync(AtualizarServicoVinculadoDto dto, string? usuarioExecutante = null, CancellationToken cancellationToken = default);

    Task<bool> RemoverVinculacaoAsync(Guid prestadorServicoId, string? usuarioExecutante = null, CancellationToken cancellationToken = default);

    Task<bool> VerificarVinculacaoExisteAsync(Guid prestadorId, Guid servicoId, CancellationToken cancellationToken = default);
} 