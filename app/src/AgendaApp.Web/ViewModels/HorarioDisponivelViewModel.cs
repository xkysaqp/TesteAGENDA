using System.ComponentModel.DataAnnotations;
using AgendaApp.Dominio.Enums;

namespace AgendaApp.Web.ViewModels;

public class HorarioDisponivelViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Dia da semana é obrigatório")]
    [Display(Name = "Dia da Semana")]
    public DiaSemana DiaSemana { get; set; }

    [Required(ErrorMessage = "Hora de início é obrigatória")]
    [Display(Name = "Hora de Início")]
    [DataType(DataType.Time)]
    public TimeOnly HoraInicio { get; set; }

    [Required(ErrorMessage = "Hora de fim é obrigatória")]
    [Display(Name = "Hora de Fim")]
    [DataType(DataType.Time)]
    public TimeOnly HoraFim { get; set; }

    [Required(ErrorMessage = "Prestador é obrigatório")]
    [Display(Name = "Prestador")]
    public Guid PrestadorId { get; set; }

    [Display(Name = "Prestador")]
    public string? PrestadorNome { get; set; }

    [Display(Name = "Data de Criação")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
    public DateTime? DataCriacao { get; set; }

    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;

    [Display(Name = "Dia da Semana")]
    public string DiaSemanaTexto => DiaSemana switch
    {
        DiaSemana.Domingo => "Domingo",
        DiaSemana.Segunda => "Segunda-feira",
        DiaSemana.Terca => "Terça-feira",
        DiaSemana.Quarta => "Quarta-feira",
        DiaSemana.Quinta => "Quinta-feira",
        DiaSemana.Sexta => "Sexta-feira",
        DiaSemana.Sabado => "Sábado",
        _ => "Indefinido"
    };

    [Display(Name = "Horário")]
    public string HorarioFormatado => $"{HoraInicio:HH:mm} às {HoraFim:HH:mm}";

    [Display(Name = "Duração")]
    public string DuracaoFormatada
    {
        get
        {
            var duracao = HoraFim - HoraInicio;
            var totalMinutos = (int)duracao.TotalMinutes;
            return totalMinutos >= 60 
                ? $"{totalMinutos / 60}h {totalMinutos % 60:00}min"
                : $"{totalMinutos} min";
        }
    }

    [Display(Name = "Hora de Início")]
    public string HoraInicioString 
    { 
        get => HoraInicio.ToString("HH:mm"); 
        set => HoraInicio = TimeOnly.TryParse(value, out var hora) ? hora : new TimeOnly(8, 0); 
    }

    [Display(Name = "Hora de Fim")]
    public string HoraFimString 
    { 
        get => HoraFim.ToString("HH:mm"); 
        set => HoraFim = TimeOnly.TryParse(value, out var hora) ? hora : new TimeOnly(18, 0); 
    }
}

public class HorarioDisponivelListViewModel
{
    public Guid Id { get; set; }
    public DiaSemana DiaSemana { get; set; }
    public string DiaSemanaTexto { get; set; } = string.Empty;
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFim { get; set; }
    public string HorarioFormatado { get; set; } = string.Empty;
    public string DuracaoFormatada { get; set; } = string.Empty;
    public Guid PrestadorId { get; set; }
    public string PrestadorNome { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
    public bool Ativo { get; set; }
}

public class HorarioDisponivelFiltroViewModel
{
    [Display(Name = "Prestador")]
    public Guid? PrestadorId { get; set; }

    [Display(Name = "Dia da Semana")]
    public DiaSemana? DiaSemana { get; set; }

    [Display(Name = "Hora Mínima")]
    [DataType(DataType.Time)]
    public TimeOnly? HoraMinima { get; set; }

    [Display(Name = "Hora Máxima")]
    [DataType(DataType.Time)]
    public TimeOnly? HoraMaxima { get; set; }

    [Display(Name = "Apenas Ativos")]
    public bool ApenasAtivos { get; set; } = true;
}

public class HorarioMultiploViewModel
{
    [Required(ErrorMessage = "Prestador é obrigatório")]
    [Display(Name = "Prestador")]
    public Guid PrestadorId { get; set; }

    [Display(Name = "Prestador")]
    public string? PrestadorNome { get; set; }

    [Required(ErrorMessage = "Selecione pelo menos um dia da semana")]
    [Display(Name = "Dias da Semana")]
    public List<DiaSemana> DiasSemana { get; set; } = new();

    [Required(ErrorMessage = "Hora de início é obrigatória")]
    [Display(Name = "Hora de Início")]
    [DataType(DataType.Time)]
    public TimeOnly HoraInicio { get; set; }

    [Required(ErrorMessage = "Hora de fim é obrigatória")]
    [Display(Name = "Hora de Fim")]
    [DataType(DataType.Time)]
    public TimeOnly HoraFim { get; set; }

    public string HoraInicioString 
    { 
        get => HoraInicio.ToString("HH:mm"); 
        set => HoraInicio = TimeOnly.TryParse(value, out var hora) ? hora : new TimeOnly(8, 0); 
    }

    public string HoraFimString 
    { 
        get => HoraFim.ToString("HH:mm"); 
        set => HoraFim = TimeOnly.TryParse(value, out var hora) ? hora : new TimeOnly(18, 0); 
    }

    public bool Segunda { get; set; }
    public bool Terca { get; set; }
    public bool Quarta { get; set; }
    public bool Quinta { get; set; }
    public bool Sexta { get; set; }
    public bool Sabado { get; set; }
    public bool Domingo { get; set; }
} 