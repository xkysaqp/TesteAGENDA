using System.ComponentModel.DataAnnotations;
using AgendaApp.Dominio.Entities;

namespace AgendaApp.Web.ViewModels;

public class AgendamentoViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Data é obrigatória")]
    [Display(Name = "Data")]
    [DataType(DataType.Date)]
    public DateOnly Data { get; set; }

    [Required(ErrorMessage = "Hora é obrigatória")]
    [Display(Name = "Hora")]
    [DataType(DataType.Time)]
    public TimeOnly Hora { get; set; }

    [Required(ErrorMessage = "Nome do cliente é obrigatório")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Nome deve ter entre 2 e 200 caracteres")]
    [Display(Name = "Nome do Cliente")]
    public string ClienteNome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefone do cliente é obrigatório")]
    [Phone(ErrorMessage = "Telefone deve ter um formato válido")]
    [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
    [Display(Name = "Telefone do Cliente")]
    public string ClienteTelefone { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "E-mail deve ter um formato válido")]
    [StringLength(250, ErrorMessage = "E-mail deve ter no máximo 250 caracteres")]
    [Display(Name = "E-mail do Cliente")]
    public string? ClienteEmail { get; set; }

    [StringLength(500, ErrorMessage = "Observações deve ter no máximo 500 caracteres")]
    [Display(Name = "Observações")]
    public string? Observacoes { get; set; }

    [Display(Name = "Status")]
    public StatusAgendamento Status { get; set; }

    [Display(Name = "Valor")]
    [DataType(DataType.Currency)]
    public decimal? Valor { get; set; }

    [Display(Name = "Data de Confirmação")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
    public DateTime? DataConfirmacao { get; set; }

    [Display(Name = "Data de Cancelamento")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
    public DateTime? DataCancelamento { get; set; }

    [Display(Name = "Motivo do Cancelamento")]
    public string? MotivoCancelamento { get; set; }

    [Required(ErrorMessage = "Loja é obrigatória")]
    [Display(Name = "Loja")]
    public Guid LojaId { get; set; }

    [Required(ErrorMessage = "Serviço é obrigatório")]
    [Display(Name = "Serviço")]
    public Guid ServicoId { get; set; }

    [Required(ErrorMessage = "Prestador é obrigatório")]
    [Display(Name = "Prestador")]
    public Guid PrestadorId { get; set; }

    [Display(Name = "Data de Criação")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
    public DateTime DataCriacao { get; set; }

    [Display(Name = "Data de Atualização")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
    public DateTime? DataAtualizacao { get; set; }

    // Propriedades de navegação
    [Display(Name = "Loja")]
    public string LojaNome { get; set; } = string.Empty;

    [Display(Name = "Serviço")]
    public string ServicoNome { get; set; } = string.Empty;

    [Display(Name = "Prestador")]
    public string PrestadorNome { get; set; } = string.Empty;

    [Display(Name = "Valor do Serviço")]
    [DataType(DataType.Currency)]
    public decimal ServicoValor { get; set; }

    [Display(Name = "Duração do Serviço (minutos)")]
    public int ServicoDuracaoEmMinutos { get; set; }
}

public class AgendamentoListViewModel
{
    public Guid Id { get; set; }
    public DateOnly Data { get; set; }
    public TimeOnly Hora { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public string ClienteTelefone { get; set; } = string.Empty;
    public string? ClienteEmail { get; set; }
    public StatusAgendamento Status { get; set; }
    public decimal? Valor { get; set; }
    public DateTime DataCriacao { get; set; }
    public string LojaNome { get; set; } = string.Empty;
    public string ServicoNome { get; set; } = string.Empty;
    public string PrestadorNome { get; set; } = string.Empty;
    public decimal ServicoValor { get; set; }
    public int ServicoDuracaoEmMinutos { get; set; }
}

public class AgendamentoFiltroViewModel
{
    [Display(Name = "Loja")]
    public Guid? LojaId { get; set; }

    [Display(Name = "Prestador")]
    public Guid? PrestadorId { get; set; }

    [Display(Name = "Serviço")]
    public Guid? ServicoId { get; set; }

    [Display(Name = "Status")]
    public StatusAgendamento? Status { get; set; }

    [Display(Name = "Data Início")]
    [DataType(DataType.Date)]
    public DateOnly? DataInicio { get; set; }

    [Display(Name = "Data Fim")]
    [DataType(DataType.Date)]
    public DateOnly? DataFim { get; set; }

    [Display(Name = "Nome do Cliente")]
    public string? ClienteNome { get; set; }

    [Display(Name = "Apenas Ativos")]
    public bool ApenasAtivos { get; set; } = true;
}

public class AgendamentoEstatisticasViewModel
{
    public int TotalAgendamentos { get; set; }
    public int AgendamentosPendentes { get; set; }
    public int AgendamentosConfirmados { get; set; }
    public int AgendamentosCancelados { get; set; }
    public int AgendamentosConcluidos { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal ValorMedio { get; set; }
    public int AgendamentosHoje { get; set; }
    public int AgendamentosSemana { get; set; }
    public int AgendamentosMes { get; set; }
}
