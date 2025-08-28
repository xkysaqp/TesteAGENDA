using AgendaApp.Aplicacao.DTOs;
using AgendaApp.Dominio.Enums;

namespace AgendaApp.Aplicacao.Interfaces;

public interface IHorarioDisponivelService
{
    Task<HorarioDisponivelDto> CriarAsync(CriarHorarioDisponivelDto dto, CancellationToken cancellationToken = default);

    Task<ResultadoCriacaoMultiplaDto> CriarMultiplosAsync(CriarHorarioMultiploDto dto, CancellationToken cancellationToken = default);

    Task<HorarioDisponivelDto> AtualizarAsync(AtualizarHorarioDisponivelDto dto, CancellationToken cancellationToken = default);

    Task<HorarioDisponivelDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<HorarioDisponivelResumoDto>> ObterComFiltroAsync(HorarioDisponivelFiltroDto filtro, CancellationToken cancellationToken = default);

    Task<bool> DesativarAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ExisteConflitoAsync(Guid prestadorId, AgendaApp.Dominio.Enums.DiaSemana diaSemana, TimeOnly horaInicio, TimeOnly horaFim, Guid? excludeId = null, CancellationToken cancellationToken = default);

    Task<HorarioEstatisticasDto> ObterEstatisticasAsync(Guid? prestadorId = null, CancellationToken cancellationToken = default);

    Task<IEnumerable<HorarioDisponivelDto>> ObterPorPrestadorEDiaAsync(Guid prestadorId, DiaSemana diaSemana, CancellationToken cancellationToken = default);

    (bool IsValid, string? ErrorMessage) ValidarPermissoesCriacao(Guid? userLojaId, Guid prestadorLojaId, bool ehUsuarioLoja);

    (bool IsValid, string? ErrorMessage) ValidarPermissoesEdicao(Guid? userLojaId, Guid prestadorLojaId, bool ehUsuarioLoja);

    (bool IsValid, string? ErrorMessage) ValidarPermissoesExclusao(Guid? userLojaId, Guid prestadorLojaId, bool ehUsuarioLoja);
} 