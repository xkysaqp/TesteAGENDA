using System.ComponentModel.DataAnnotations;

namespace AgendaApp.Web.ViewModels;

public class VincularServicoViewModel
{
    [Required]
    public Guid PrestadorId { get; set; }
    
    [Required]
    public Guid ServicoId { get; set; }
    
    [Display(Name = "Valor Personalizado")]
    [Range(0.01, 99999.99, ErrorMessage = "Valor deve ser entre R$ 0,01 e R$ 99.999,99")]
    public decimal? ValorPersonalizado { get; set; }
    
    [Display(Name = "Duração Personalizada (min)")]
    [Range(1, 1440, ErrorMessage = "Duração deve ser entre 1 e 1440 minutos")]
    public int? DuracaoPersonalizadaEmMinutos { get; set; }
}

public class ServicoGlobalDisponivelViewModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int DuracaoEmMinutos { get; set; }
    public bool JaVinculado { get; set; }
    public string LojaNome { get; set; } = string.Empty;
    
    public string ValorFormatado => Valor.ToString("C");
    public string DuracaoFormatada => DuracaoEmMinutos >= 60
        ? $"{DuracaoEmMinutos / 60}h {DuracaoEmMinutos % 60:00}min"
        : $"{DuracaoEmMinutos}min";
}

public class ServicoVinculadoViewModel
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
    
    public string ValorEfetivoFormatado => ValorEfetivo.ToString("C");
    public string ValorPadraoFormatado => ValorPadrao.ToString("C");
    public string ValorPersonalizadoFormatado => ValorPersonalizado?.ToString("C") ?? "";
    
    public string DuracaoEfetivaFormatada => DuracaoEfetivaEmMinutos >= 60
        ? $"{DuracaoEfetivaEmMinutos / 60}h {DuracaoEfetivaEmMinutos % 60:00}min"
        : $"{DuracaoEfetivaEmMinutos}min";
        
    public string DuracaoPadraoFormatada => DuracaoPadraoEmMinutos >= 60
        ? $"{DuracaoPadraoEmMinutos / 60}h {DuracaoPadraoEmMinutos % 60:00}min"
        : $"{DuracaoPadraoEmMinutos}min";
        
    public string DuracaoPersonalizadaFormatada => DuracaoPersonalizadaEmMinutos.HasValue
        ? (DuracaoPersonalizadaEmMinutos >= 60
            ? $"{DuracaoPersonalizadaEmMinutos / 60}h {DuracaoPersonalizadaEmMinutos % 60:00}min"
            : $"{DuracaoPersonalizadaEmMinutos}min")
        : "";
}

public class EditarServicoVinculadoViewModel
{
    [Required]
    public Guid PrestadorServicoId { get; set; }
    
    public string ServicoNome { get; set; } = string.Empty;
    
    [Display(Name = "Valor Padrão")]
    public decimal ValorPadrao { get; set; }
    
    [Display(Name = "Duração Padrão (min)")]
    public int DuracaoPadraoEmMinutos { get; set; }
    
    [Display(Name = "Valor Personalizado")]
    [Range(0.01, 99999.99, ErrorMessage = "Valor deve ser entre R$ 0,01 e R$ 99.999,99")]
    public decimal? ValorPersonalizado { get; set; }
    
    [Display(Name = "Duração Personalizada (min)")]
    [Range(1, 1440, ErrorMessage = "Duração deve ser entre 1 e 1440 minutos")]
    public int? DuracaoPersonalizadaEmMinutos { get; set; }
} 