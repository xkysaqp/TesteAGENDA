using System.ComponentModel.DataAnnotations;
using AgendaApp.Dominio.Enums;

namespace AgendaApp.Web.ViewModels;

public class LojaViewModel
{
    public Guid Id { get; set; }
    
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;
    
    [Display(Name = "Slug")]
    public string Slug { get; set; } = string.Empty;
    
    [Display(Name = "CNPJ")]
    public string CNPJ { get; set; } = string.Empty;
    
    [Display(Name = "Email de Contato")]
    public string EmailContato { get; set; } = string.Empty;
    
    [Display(Name = "Telefone de Contato")]
    public string? TelefoneContato { get; set; }
    
    [Display(Name = "Ativa")]
    public bool Ativa { get; set; }
    
    [Display(Name = "Data de Vencimento")]
    [DataType(DataType.Date)]
    public DateTime? DataVencimento { get; set; }
    
    [Display(Name = "Plano")]
    public TipoPlano Plano { get; set; }
    
    [Display(Name = "Data de Criação")]
    public DateTime DataCriacao { get; set; }
    
    [Display(Name = "Última Atualização")]
    public DateTime? DataAtualizacao { get; set; }
    
    
    public bool VencimentoProximo => DataVencimento.HasValue && 
                                    DataVencimento.Value <= DateTime.UtcNow.AddDays(30);
    public bool Vencida => DataVencimento.HasValue && 
                          DataVencimento.Value < DateTime.UtcNow;
    public int DiasParaVencimento => DataVencimento.HasValue ? 
                                    (int)(DataVencimento.Value - DateTime.UtcNow).TotalDays : 0;
    
    public string StatusVencimento
    {
        get
        {
            if (!DataVencimento.HasValue) return "Sem vencimento";
            if (Vencida) return "Vencida";
            if (VencimentoProximo) return $"Vence em {DiasParaVencimento} dias";
            return "Em dia";
        }
    }
    
    public string ClasseStatusVencimento
    {
        get
        {
            if (!DataVencimento.HasValue) return "text-muted";
            if (Vencida) return "text-danger";
            if (VencimentoProximo) return "text-warning";
            return "text-success";
        }
    }
}

public class CriarLojaViewModel
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
    [Display(Name = "Nome da Loja")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Slug é obrigatório")]
    [StringLength(100, ErrorMessage = "Slug deve ter no máximo 100 caracteres")]
    [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Slug deve conter apenas letras minúsculas, números e hífens")]
    [Display(Name = "Slug (identificador único)")]
    public string Slug { get; set; } = string.Empty;

    [Required(ErrorMessage = "CNPJ é obrigatório")]
    [Display(Name = "CNPJ")]
    public string CNPJ { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email de contato é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(254, ErrorMessage = "Email deve ter no máximo 254 caracteres")]
    [Display(Name = "Email de Contato")]
    public string EmailContato { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
    [Display(Name = "Telefone de Contato")]
    public string? TelefoneContato { get; set; }

    [Display(Name = "Data de Vencimento")]
    [DataType(DataType.Date)]
    public DateTime? DataVencimento { get; set; }

    [Required(ErrorMessage = "Plano é obrigatório")]
    [Display(Name = "Plano")]
    public TipoPlano Plano { get; set; } = TipoPlano.Basico;
}

public class EditarLojaViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
    [Display(Name = "Nome da Loja")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Slug é obrigatório")]
    [StringLength(100, ErrorMessage = "Slug deve ter no máximo 100 caracteres")]
    [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Slug deve conter apenas letras minúsculas, números e hífens")]
    [Display(Name = "Slug (identificador único)")]
    public string Slug { get; set; } = string.Empty;

    [Required(ErrorMessage = "CNPJ é obrigatório")]
    [Display(Name = "CNPJ")]
    public string CNPJ { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email de contato é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(254, ErrorMessage = "Email deve ter no máximo 254 caracteres")]
    [Display(Name = "Email de Contato")]
    public string EmailContato { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
    [Display(Name = "Telefone de Contato")]
    public string? TelefoneContato { get; set; }

    [Display(Name = "Loja Ativa")]
    public bool Ativa { get; set; }

    [Display(Name = "Data de Vencimento")]
    [DataType(DataType.Date)]
    public DateTime? DataVencimento { get; set; }

    [Required(ErrorMessage = "Plano é obrigatório")]
    [Display(Name = "Plano")]
    public TipoPlano Plano { get; set; }
} 