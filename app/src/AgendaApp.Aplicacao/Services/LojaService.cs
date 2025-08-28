using AgendaApp.Aplicacao.DTOs;
using AgendaApp.Aplicacao.Interfaces;
using AgendaApp.Dominio.Entities;
using AgendaApp.Dominio.Interfaces;
using AgendaApp.Dominio.ValueObjects;
using AgendaApp.Dominio.Enums;

namespace AgendaApp.Aplicacao.Services;

public class LojaService : ILojaService
{
    private readonly ILojaRepository _lojaRepository;

    public LojaService(ILojaRepository lojaRepository)
    {
        _lojaRepository = lojaRepository;
    }

    public async Task<IEnumerable<LojaDto>> ObterTodasAsync()
    {
        var lojas = await _lojaRepository.ObterTodosAsync();
        return lojas.Select(MapearParaDto);
    }

    public async Task<LojaDto?> ObterPorIdAsync(Guid id)
    {
        var loja = await _lojaRepository.ObterPorIdAsync(id);
        return loja != null ? MapearParaDto(loja) : null;
    }

    public async Task<LojaDto?> ObterPorSlugAsync(string slug)
    {
        var loja = await _lojaRepository.ObterPorSlugAsync(slug);
        return loja != null ? MapearParaDto(loja) : null;
    }

    public async Task<LojaDto> CriarAsync(CriarLojaDto dto)
    {
        // Validações de negócio
        if (await _lojaRepository.SlugJaExisteAsync(dto.Slug))
            throw new InvalidOperationException("Já existe uma loja com este slug.");

        if (await _lojaRepository.CnpjJaExisteAsync(dto.CNPJ))
            throw new InvalidOperationException("Já existe uma loja com este CNPJ.");

        var loja = new Loja(
            dto.Nome,
            dto.Slug,
            dto.CNPJ,
            dto.EmailContato,
            dto.TelefoneContato ?? string.Empty,
            dto.Plano
        );

        // Definir data de vencimento se fornecida
        if (dto.DataVencimento.HasValue)
        {
            loja.RenovarAssinatura(1);  
        }

        var lojaCriada = await _lojaRepository.AdicionarAsync(loja);
        return MapearParaDto(lojaCriada);
    }

    public async Task<LojaDto> AtualizarAsync(AtualizarLojaDto dto)
    {
        var loja = await _lojaRepository.ObterPorIdAsync(dto.Id);
        if (loja == null)
            throw new ArgumentException("Loja não encontrada.");

        // Validar duplicatas (excluindo a loja atual)
        if (loja.Slug != dto.Slug && await _lojaRepository.SlugJaExisteAsync(dto.Slug))
            throw new InvalidOperationException("Já existe uma loja com este slug.");

        if (loja.CNPJ.Numero != dto.CNPJ && await _lojaRepository.CnpjJaExisteAsync(dto.CNPJ))
            throw new InvalidOperationException("Já existe uma loja com este CNPJ.");

        // Atualizar informações
        loja.AtualizarInformacoes(dto.Nome, dto.EmailContato, dto.TelefoneContato ?? string.Empty);
        loja.AtualizarCNPJ(dto.CNPJ);
        loja.AtualizarPlano(dto.Plano);

        var lojaAtualizada = await _lojaRepository.AtualizarAsync(loja);
        return MapearParaDto(lojaAtualizada);
    }

    public async Task<bool> ExcluirAsync(Guid id)
    {
        var loja = await _lojaRepository.ObterPorIdAsync(id);
        if (loja == null)
            return false;

        return await _lojaRepository.RemoverAsync(id);
    }

    public async Task<bool> SlugJaExisteAsync(string slug, Guid? lojaIdExcluir = null)
    {
        return await _lojaRepository.SlugJaExisteAsync(slug, lojaIdExcluir);
    }

    public async Task<bool> CnpjJaExisteAsync(string cnpj, Guid? lojaIdExcluir = null)
    {
        return await _lojaRepository.CnpjJaExisteAsync(cnpj, lojaIdExcluir);
    }

    public async Task<IEnumerable<LojaDto>> ObterLojasVencidasAsync()
    {
        var lojas = await _lojaRepository.ObterLojasVencidasAsync();
        return lojas.Select(MapearParaDto);
    }

    public async Task<IEnumerable<LojaDto>> ObterLojasProximasVencimentoAsync(int dias = 30)
    {
        var lojas = await _lojaRepository.ObterLojasProximasVencimentoAsync(dias);
        return lojas.Select(MapearParaDto);
    }

    private static LojaDto MapearParaDto(Loja loja)
    {
        return new LojaDto
        {
            Id = loja.Id,
            Nome = loja.Nome,
            Slug = loja.Slug,
            CNPJ = loja.CNPJ.ToString(),
            EmailContato = loja.EmailContato.ToString(),
            TelefoneContato = loja.TelefoneContato,
            Ativa = loja.Ativa,
            DataVencimento = loja.DataVencimento,
            Plano = loja.Plano,
            DataCriacao = loja.DataCriacao,
            DataAtualizacao = loja.DataAtualizacao
        };
    }
} 