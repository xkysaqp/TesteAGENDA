using AgendaApp.Aplicacao.DTOs;

namespace AgendaApp.Aplicacao.Interfaces;

public interface ILojaService
{
    Task<IEnumerable<LojaDto>> ObterTodasAsync();
    Task<LojaDto?> ObterPorIdAsync(Guid id);
    Task<LojaDto?> ObterPorSlugAsync(string slug);
    Task<LojaDto> CriarAsync(CriarLojaDto dto);
    Task<LojaDto> AtualizarAsync(AtualizarLojaDto dto);
    Task<bool> ExcluirAsync(Guid id);
    Task<bool> SlugJaExisteAsync(string slug, Guid? lojaIdExcluir = null);
    Task<bool> CnpjJaExisteAsync(string cnpj, Guid? lojaIdExcluir = null);
    Task<IEnumerable<LojaDto>> ObterLojasVencidasAsync();
    Task<IEnumerable<LojaDto>> ObterLojasProximasVencimentoAsync(int dias = 30);
} 