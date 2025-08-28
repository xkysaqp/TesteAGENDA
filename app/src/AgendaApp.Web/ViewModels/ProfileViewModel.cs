using System.ComponentModel.DataAnnotations;

namespace AgendaApp.Web.ViewModels;

public class ProfileViewModel
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
    [Display(Name = "Nome Completo")]
    public string Nome { get; set; } = string.Empty;

    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O telefone é obrigatório")]
    [Phone(ErrorMessage = "Telefone inválido")]
    [Display(Name = "Telefone")]
    public string Telefone { get; set; } = string.Empty;

    [StringLength(18, ErrorMessage = "CNPJ inválido")]
    [Display(Name = "CNPJ")]
    public string? CNPJ { get; set; }

    [StringLength(100, ErrorMessage = "Área de atuação deve ter no máximo 100 caracteres")]
    [Display(Name = "Área de Atuação")]
    public string? AreaAtuacao { get; set; }

    [Display(Name = "Último Acesso")]
    public DateTime? UltimoAcesso { get; set; }

    public bool EhAdmin { get; set; }
} 