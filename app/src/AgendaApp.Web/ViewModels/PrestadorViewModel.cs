using System.ComponentModel.DataAnnotations;

namespace AgendaApp.Web.ViewModels;

public class PrestadorViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Nome deve ter entre 2 e 200 caracteres")]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(18, ErrorMessage = "CNPJ deve ter no máximo 18 caracteres")]
    [RegularExpression(@"^\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}$|^\d{14}$", ErrorMessage = "CNPJ deve estar no formato XX.XXX.XXX/XXXX-XX ou apenas números")]
    [Display(Name = "CNPJ")]
    public string? CNPJ { get; set; }

    [Required(ErrorMessage = "Área de atuação é obrigatória")]
    [StringLength(100, ErrorMessage = "Área de atuação deve ter no máximo 100 caracteres")]
    [Display(Name = "Área de Atuação")]
    public string AreaAtuacao { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail deve ter um formato válido")]
    [StringLength(250, ErrorMessage = "E-mail deve ter no máximo 250 caracteres")]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefone é obrigatório")]
    [Phone(ErrorMessage = "Telefone deve ter um formato válido")]
    [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
    [Display(Name = "Telefone")]
    public string Telefone { get; set; } = string.Empty;

    [Display(Name = "Data de Criação")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
    public DateTime? DataCriacao { get; set; }

    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;

    [Display(Name = "Loja")]
    public Guid LojaId { get; set; }

    public List<ServicoListViewModel> Servicos { get; set; } = new List<ServicoListViewModel>();

    public List<ServicoCreateViewModel> ServicosCreate { get; set; } = new List<ServicoCreateViewModel>();

    public List<VincularServicoCreateViewModel> ServicosVinculados { get; set; } = new List<VincularServicoCreateViewModel>();

    public List<HorarioDisponivelCreateViewModel> HorariosDisponiveis { get; set; } = new List<HorarioDisponivelCreateViewModel>();
}

public class PrestadorListViewModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? CNPJ { get; set; }
    public string AreaAtuacao { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
    public bool Ativo { get; set; }
    public int TotalServicos { get; set; }
    public int TotalHorarios { get; set; }
}

public class PrestadorFiltroViewModel
{
    [Display(Name = "Nome")]
    public string? Nome { get; set; }

    [Display(Name = "Área de Atuação")]
    public string? AreaAtuacao { get; set; }

    [Display(Name = "E-mail")]
    public string? Email { get; set; }

    [Display(Name = "Apenas Ativos")]
    public bool ApenasAtivos { get; set; } = true;

    [Display(Name = "Loja")]
    public Guid? LojaId { get; set; }
}

public class VincularServicoCreateViewModel
{
    public Guid ServicoId { get; set; }

    public decimal? ValorPersonalizado { get; set; }

    public int? DuracaoPersonalizadaEmMinutos { get; set; }
}

public class HorarioDisponivelCreateViewModel
{
    public int DiaSemana { get; set; }
    
    public string HoraInicio { get; set; } = string.Empty;
    
    public string HoraFim { get; set; } = string.Empty;
    
    public bool Ativo { get; set; } = true;
} 