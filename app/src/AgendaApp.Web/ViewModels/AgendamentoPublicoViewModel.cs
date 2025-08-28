using System.ComponentModel.DataAnnotations;

namespace AgendaApp.Web.ViewModels;

public class AgendamentoPublicoViewModel
{
    public LojaPublicaViewModel Loja { get; set; } = new();
    public List<PrestadorPublicoViewModel> Prestadores { get; set; } = new();
    public List<ServicoPublicoViewModel> Servicos { get; set; } = new();
}

public class LojaPublicaViewModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
}

public class PrestadorPublicoViewModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string AreaAtuacao { get; set; } = string.Empty;
}

public class ServicoPublicoViewModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int DuracaoEmMinutos { get; set; }
    public Guid PrestadorId { get; set; }
}

public class CriarAgendamentoPublicoViewModel
{
    [Required(ErrorMessage = "Prestador é obrigatório")]
    public Guid PrestadorId { get; set; }

    [Required(ErrorMessage = "Serviço é obrigatório")]
    public Guid ServicoId { get; set; }

    [Required(ErrorMessage = "Data é obrigatória")]
    public DateOnly Data { get; set; }

    [Required(ErrorMessage = "Hora de início é obrigatória")]
    public string HoraInicio { get; set; } = string.Empty;

    [Required(ErrorMessage = "Hora de fim é obrigatória")]
    public string HoraFim { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Nome deve ter entre 2 e 200 caracteres")]
    public string NomeCliente { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail deve ter um formato válido")]
    public string EmailCliente { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefone é obrigatório")]
    [Phone(ErrorMessage = "Telefone deve ter um formato válido")]
    public string TelefoneCliente { get; set; } = string.Empty;

    public string? Observacoes { get; set; }
}

