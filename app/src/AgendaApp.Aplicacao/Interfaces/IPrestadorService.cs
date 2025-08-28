using AgendaApp.Aplicacao.DTOs;

namespace AgendaApp.Aplicacao.Interfaces;

public interface IPrestadorService
{
    Task<PrestadorDto> CriarAsync(CriarPrestadorDto dto, string? usuarioExecutante = null, CancellationToken cancellationToken = default);

    Task<PrestadorDto> AtualizarAsync(AtualizarPrestadorDto dto, string? usuarioExecutante = null, CancellationToken cancellationToken = default);

    Task<PrestadorDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<PrestadorResumoDto>> ObterComFiltroAsync(PrestadorFiltroDto filtro, CancellationToken cancellationToken = default);

    Task<bool> DesativarAsync(Guid id, string? usuarioExecutante = null, CancellationToken cancellationToken = default);

    Task<bool> AtivarAsync(Guid id, string? usuarioExecutante = null, CancellationToken cancellationToken = default);

    Task<bool> EmailJaExisteAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default);

    Task<bool> CnpjJaExisteAsync(string cnpj, Guid? excludeId = null, CancellationToken cancellationToken = default);

    bool PodeAcessarLoja(Guid? userLojaId, Guid? lojaId, bool ehUsuarioLoja);

    (bool IsValid, string? ErrorMessage) ValidarPermissoesCriacao(Guid? userLojaId, Guid prestadorLojaId, bool ehUsuarioLoja);

    (bool IsValid, string? ErrorMessage) ValidarPermissoesEdicao(Guid? userLojaId, Guid prestadorLojaId, bool ehUsuarioLoja);

    (bool IsValid, string? ErrorMessage) ValidarPermissoesExclusao(Guid? userLojaId, Guid prestadorLojaId, bool ehUsuarioLoja);
} 