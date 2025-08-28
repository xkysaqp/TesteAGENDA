using System.ComponentModel.DataAnnotations;
using AgendaApp.Dominio.Enums;

namespace AgendaApp.Aplicacao.DTOs;

public class LojaDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string CNPJ { get; set; } = string.Empty;
    public string EmailContato { get; set; } = string.Empty;
    public string? TelefoneContato { get; set; }
    public bool Ativa { get; set; }
    public DateTime? DataVencimento { get; set; }
    public TipoPlano Plano { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    
    // Propriedades calculadas
    public bool VencimentoProximo => DataVencimento.HasValue && 
                                    DataVencimento.Value <= DateTime.UtcNow.AddDays(30);
    public bool Vencida => DataVencimento.HasValue && 
                          DataVencimento.Value < DateTime.UtcNow;
    public int DiasParaVencimento => DataVencimento.HasValue ? 
                                    (int)(DataVencimento.Value - DateTime.UtcNow).TotalDays : 0;
}

public class CriarLojaDto
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Slug é obrigatório")]
    [StringLength(100, ErrorMessage = "Slug deve ter no máximo 100 caracteres")]
    [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Slug deve conter apenas letras minúsculas, números e hífens")]
    public string Slug { get; set; } = string.Empty;

    [Required(ErrorMessage = "CNPJ é obrigatório")]
    [StringLength(18, ErrorMessage = "CNPJ inválido")]
    public string CNPJ { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email de contato é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(254, ErrorMessage = "Email deve ter no máximo 254 caracteres")]
    public string EmailContato { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
    public string? TelefoneContato { get; set; }

    public DateTime? DataVencimento { get; set; }

    [Required(ErrorMessage = "Plano é obrigatório")]
    public TipoPlano Plano { get; set; } = TipoPlano.Basico;
}

public class AtualizarLojaDto
{
    [Required]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Slug é obrigatório")]
    [StringLength(100, ErrorMessage = "Slug deve ter no máximo 100 caracteres")]
    [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Slug deve conter apenas letras minúsculas, números e hífens")]
    public string Slug { get; set; } = string.Empty;

    [Required(ErrorMessage = "CNPJ é obrigatório")]
    [StringLength(18, ErrorMessage = "CNPJ inválido")]
    public string CNPJ { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email de contato é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(254, ErrorMessage = "Email deve ter no máximo 254 caracteres")]
    public string EmailContato { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
    public string? TelefoneContato { get; set; }

    public bool Ativa { get; set; }

    public DateTime? DataVencimento { get; set; }

    [Required(ErrorMessage = "Plano é obrigatório")]
    public TipoPlano Plano { get; set; } = TipoPlano.Basico;
} 