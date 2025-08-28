using System.ComponentModel.DataAnnotations;

namespace AgendaApp.Web.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
    [Display(Name = "Nome Completo")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O telefone é obrigatório")]
    [Phone(ErrorMessage = "Telefone inválido")]
    [Display(Name = "Telefone")]
    public string Telefone { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória")]
    [StringLength(100, ErrorMessage = "A senha deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Senha")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirmar Senha")]
    [Compare("Password", ErrorMessage = "A senha e a confirmação não coincidem.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Display(Name = "Loja (deixe vazio para Admin)")]
    public Guid? LojaId { get; set; }

    [StringLength(18, ErrorMessage = "CNPJ inválido")]
    [Display(Name = "CNPJ (opcional)")]
    public string? CNPJ { get; set; }

    [StringLength(100, ErrorMessage = "Área de atuação deve ter no máximo 100 caracteres")]
    [Display(Name = "Área de Atuação (opcional)")]
    public string? AreaAtuacao { get; set; }
} 