namespace AgendaApp.Aplicacao.DTOs;

public class VincularServicoDto
{
    public Guid PrestadorId { get; set; }
    public Guid ServicoId { get; set; }
    public decimal? ValorPersonalizado { get; set; }
    public int? DuracaoPersonalizadaEmMinutos { get; set; }
}

public class AtualizarServicoVinculadoDto
{
    public Guid PrestadorServicoId { get; set; }
    public decimal? ValorPersonalizado { get; set; }
    public int? DuracaoPersonalizadaEmMinutos { get; set; }
}

public class ServicoVinculadoDto
{
    public Guid PrestadorServicoId { get; set; }
    public Guid ServicoId { get; set; }
    public string ServicoNome { get; set; } = string.Empty;
    public string ServicoDescricao { get; set; } = string.Empty;
    public decimal ValorPadrao { get; set; }
    public int DuracaoPadraoEmMinutos { get; set; }
    public decimal? ValorPersonalizado { get; set; }
    public int? DuracaoPersonalizadaEmMinutos { get; set; }
    public DateTime DataVinculacao { get; set; }
    
    public decimal ValorEfetivo => ValorPersonalizado ?? ValorPadrao;
    public int DuracaoEfetivaEmMinutos => DuracaoPersonalizadaEmMinutos ?? DuracaoPadraoEmMinutos;
    
    public bool TemValorPersonalizado => ValorPersonalizado.HasValue;
    public bool TemDuracaoPersonalizada => DuracaoPersonalizadaEmMinutos.HasValue;
}

public class ServicoGlobalDisponivelDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int DuracaoEmMinutos { get; set; }
    public bool JaVinculado { get; set; }
    public string LojaId { get; set; } = string.Empty;
    public string LojaNome { get; set; } = string.Empty;
} 