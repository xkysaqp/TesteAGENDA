using AgendaApp.Aplicacao.DTOs;

namespace AgendaApp.Aplicacao.Interfaces;

public interface IServicoService
{
    Task<ServicoDto> CriarAsync(CriarServicoDto dto, CancellationToken cancellationToken = default);

    Task<ServicoDto> AtualizarAsync(AtualizarServicoDto dto, CancellationToken cancellationToken = default);

    Task<ServicoDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<ServicoResumoDto>> ObterComFiltroAsync(ServicoFiltroDto filtro, CancellationToken cancellationToken = default);

    Task<IEnumerable<ServicoResumoDto>> ObterPorPrestadorAsync(Guid prestadorId, CancellationToken cancellationToken = default);

    Task<bool> DesativarAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> NomeJaExisteParaPrestadorAsync(string nome, Guid prestadorId, Guid? excludeId = null, CancellationToken cancellationToken = default);

    Task<ServicoEstatisticasDto> ObterEstatisticasAsync(Guid? prestadorId = null, CancellationToken cancellationToken = default);

    (bool IsValid, string? ErrorMessage) ValidarPermissoesCriacao(Guid? userLojaId, Guid servicoLojaId, bool ehUsuarioLoja);

    (bool IsValid, string? ErrorMessage) ValidarPermissoesEdicao(Guid? userLojaId, Guid servicoLojaId, bool ehUsuarioLoja);

    (bool IsValid, string? ErrorMessage) ValidarPermissoesExclusao(Guid? userLojaId, Guid servicoLojaId, bool ehUsuarioLoja);

    (bool IsValid, string? ErrorMessage) PodeAcessarServico(Guid? userLojaId, Guid servicoLojaId, bool ehUsuarioLoja);
} 