using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AgendaApp.Infraestrutura.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;

    [Display(Name = "Loja")]
    public Guid LojaId { get; set; }

    [Display(Name = "CNPJ")]
    public string? CNPJ { get; set; }

    [Display(Name = "Área de Atuação")]
    public string? AreaAtuacao { get; set; }

    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;

    [Display(Name = "Último Acesso")]
    public DateTime? UltimoAcesso { get; set; }

    [Display(Name = "Data de Criação")]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    [Display(Name = "Data de Atualização")]
    public DateTime? DataAtualizacao { get; set; }

    public void RegistrarAcesso()
    {
        UltimoAcesso = DateTime.UtcNow;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Ativar()
    {
        Ativo = true;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Desativar()
    {
        Ativo = false;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void VincularALoja(Guid lojaId)
    {
        if (lojaId == Guid.Empty)
            throw new ArgumentException("ID da loja é obrigatório");
            
        LojaId = lojaId;
        DataAtualizacao = DateTime.UtcNow;
    }

    public bool EhAdmin => UserName == "admin@agendaapp.com" || Email == "admin@agendaapp.com";
    
    public bool EhUsuarioLoja => !EhAdmin;
    
    public bool PodeAcessarLoja(Guid? lojaId) => EhAdmin || LojaId == lojaId;
} 