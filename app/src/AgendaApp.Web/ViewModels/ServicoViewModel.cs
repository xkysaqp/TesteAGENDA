using System.ComponentModel.DataAnnotations;

namespace AgendaApp.Web.ViewModels;

public class ServicoViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Nome do serviço é obrigatório")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Nome deve ter entre 2 e 100 caracteres")]
    [Display(Name = "Nome do Serviço")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Descrição é obrigatória")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "Descrição deve ter entre 10 e 500 caracteres")]
    [Display(Name = "Descrição")]
    [DataType(DataType.MultilineText)]
    public string Descricao { get; set; } = string.Empty;

    [Required(ErrorMessage = "Valor é obrigatório")]
    [Range(0.01, 999999.99, ErrorMessage = "Valor deve ser entre R$ 0,01 e R$ 999.999,99")]
    [Display(Name = "Valor (R$)")]
    [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
    [DataType(DataType.Currency)]
    public decimal Valor { get; set; }

    [Required(ErrorMessage = "Duração é obrigatória")]
    [Range(15, 1440, ErrorMessage = "Duração deve ser entre 15 minutos e 24 horas (1440 minutos)")]
    [Display(Name = "Duração (minutos)")]
    public int DuracaoEmMinutos { get; set; }

    [Display(Name = "Prestador")]
    public Guid? PrestadorId { get; set; }

    [Display(Name = "Prestador")]
    public string? PrestadorNome { get; set; }

    [Display(Name = "Data de Criação")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
    public DateTime? DataCriacao { get; set; }

    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;

    [Display(Name = "Loja")]
    public Guid? LojaId { get; set; }

    public string ValorFormatado => Valor.ToString("C");
    
    [Display(Name = "Duração Formatada")]
    public string DuracaoFormatada => 
        DuracaoEmMinutos >= 60 
            ? $"{DuracaoEmMinutos / 60}h {DuracaoEmMinutos % 60:00}min"
            : $"{DuracaoEmMinutos} min";
}

public class ServicoListViewModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int DuracaoEmMinutos { get; set; }
    public Guid? PrestadorId { get; set; }
    public string? PrestadorNome { get; set; }
    public DateTime DataCriacao { get; set; }
    public bool Ativo { get; set; }

    public string ValorFormatado => Valor.ToString("C");
    public string DuracaoFormatada => DuracaoEmMinutos >= 60
        ? $"{DuracaoEmMinutos / 60}h {DuracaoEmMinutos % 60:00}min"
        : $"{DuracaoEmMinutos} min";
}

public class ServicoCreateViewModel
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int DuracaoEmMinutos { get; set; }
    public bool Ativo { get; set; } = true;
}

public class ServicoFiltroViewModel
{
    [Display(Name = "Nome do Serviço")]
    public string? Nome { get; set; }

    [Display(Name = "Prestador")]
    public Guid? PrestadorId { get; set; }

    [Display(Name = "Valor Mínimo")]
    [Range(0, double.MaxValue, ErrorMessage = "Valor mínimo deve ser maior que zero")]
    [DataType(DataType.Currency)]
    public decimal? ValorMinimo { get; set; }

    [Display(Name = "Valor Máximo")]
    [Range(0, double.MaxValue, ErrorMessage = "Valor máximo deve ser maior que zero")]
    [DataType(DataType.Currency)]
    public decimal? ValorMaximo { get; set; }

    [Display(Name = "Duração Máxima (minutos)")]
    [Range(1, 1440, ErrorMessage = "Duração deve ser entre 1 e 1440 minutos")]
    public int? DuracaoMaxima { get; set; }

    [Display(Name = "Apenas Ativos")]
    public bool ApenasAtivos { get; set; } = true;

    [Display(Name = "Loja")]
    public Guid? LojaId { get; set; }
}

public class AdicionarServicoModalViewModel
{
    [Required(ErrorMessage = "Nome do serviço é obrigatório")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Nome deve ter entre 2 e 100 caracteres")]
    [Display(Name = "Nome do Serviço")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
    [Display(Name = "Descrição")]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "Valor é obrigatório")]
    [Range(0.01, 99999.99, ErrorMessage = "Valor deve ser entre R$ 0,01 e R$ 99.999,99")]
    [Display(Name = "Valor (R$)")]
    public decimal Valor { get; set; }

    [Required(ErrorMessage = "Duração é obrigatória")]
    [Range(1, 1440, ErrorMessage = "Duração deve ser entre 1 e 1440 minutos")]
    [Display(Name = "Duração (minutos)")]
    public int DuracaoEmMinutos { get; set; }

    [Required]
    public Guid PrestadorId { get; set; }

    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;
} 