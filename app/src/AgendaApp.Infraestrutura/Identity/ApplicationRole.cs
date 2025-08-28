using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AgendaApp.Infraestrutura.Identity;

public class ApplicationRole : IdentityRole<Guid>
{
    [Display(Name = "Descrição")]
    public string? Descricao { get; set; }

    [Display(Name = "Data de Criação")]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public ApplicationRole() : base()
    {
    }

    public ApplicationRole(string roleName) : base(roleName)
    {
        DataCriacao = DateTime.UtcNow;
    }
} 