namespace AgendaApp.Dominio.Entities;

public class PrestadorServico : BaseEntity
{
    public Guid PrestadorId { get; private set; }
    
    public Guid ServicoId { get; private set; }
    
    public decimal? ValorPersonalizado { get; private set; }
    
    public int? DuracaoPersonalizadaEmMinutos { get; private set; }

    public virtual Prestador Prestador { get; private set; } = null!;
    public virtual Servico Servico { get; private set; } = null!;

    protected PrestadorServico() { }

    public PrestadorServico(Guid prestadorId, Guid servicoId, decimal? valorPersonalizado = null, int? duracaoPersonalizadaEmMinutos = null)
    {
        PrestadorId = prestadorId;
        ServicoId = servicoId;
        ValorPersonalizado = valorPersonalizado;
        DuracaoPersonalizadaEmMinutos = duracaoPersonalizadaEmMinutos;
    }

    public void AtualizarValoresPersonalizados(decimal? valorPersonalizado, int? duracaoPersonalizadaEmMinutos)
    {
        ValorPersonalizado = valorPersonalizado;
        DuracaoPersonalizadaEmMinutos = duracaoPersonalizadaEmMinutos;
        MarcarComoAtualizada();
    }

    public decimal ObterValorEfetivo() => ValorPersonalizado ?? Servico?.Valor ?? 0;

    public int ObterDuracaoEfetiva() => DuracaoPersonalizadaEmMinutos ?? Servico?.DuracaoEmMinutos ?? 0;
} 