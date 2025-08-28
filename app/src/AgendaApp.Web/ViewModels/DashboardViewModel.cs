using System.ComponentModel.DataAnnotations;

namespace AgendaApp.Web.ViewModels;

public class DashboardViewModel
{
    [Display(Name = "Nome do Usuário")]
    public string NomeUsuario { get; set; } = string.Empty;

    [Display(Name = "Tipo de Usuário")]
    public string TipoUsuario { get; set; } = string.Empty;

    public Guid? LojaId { get; set; }

    [Display(Name = "Total de Prestadores")]
    public int TotalPrestadores { get; set; }

    [Display(Name = "Total de Serviços")]
    public int TotalServicos { get; set; }

    [Display(Name = "Total de Agendamentos")]
    public int TotalAgendamentos { get; set; }

    [Display(Name = "Receita Total")]
    public decimal ReceitaTotal { get; set; }

    public List<object> DadosGraficos { get; set; } = new();

    public bool EhAdmin => TipoUsuario.Contains("Administrador");

    public bool EhUsuarioLoja => TipoUsuario.Contains("Loja");

    public string MensagemBoasVindas
    {
        get
        {
            var saudacao = DateTime.Now.Hour switch
            {
                >= 6 and < 12 => "Bom dia",
                >= 12 and < 18 => "Boa tarde",
                _ => "Boa noite"
            };

            return $"{saudacao}, {NomeUsuario}!";
        }
    }

    public List<AtividadeRecenteViewModel> AtividadesRecentes { get; set; } = new();
}

public class AtividadeRecenteViewModel
{
    public string Tipo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateTime Data { get; set; }
    public string IconeClass { get; set; } = "fas fa-info-circle";
    public string CorClass { get; set; } = "text-info";
} 