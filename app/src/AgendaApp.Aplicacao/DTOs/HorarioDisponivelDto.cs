using AgendaApp.Dominio.Enums;

namespace AgendaApp.Aplicacao.DTOs;

public class CriarHorarioDisponivelDto
{
    public DiaSemana DiaSemana { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFim { get; set; }
    public Guid PrestadorId { get; set; }
    public Guid LojaId { get; set; }
}

public class AtualizarHorarioDisponivelDto
{
    public Guid Id { get; set; }
    public DiaSemana DiaSemana { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFim { get; set; }
    public Guid PrestadorId { get; set; }
    public Guid LojaId { get; set; }
    public bool Ativo { get; set; } = true;
}

public class CriarHorarioMultiploDto
{
    public List<DiaSemana> DiasSemana { get; set; } = new();
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFim { get; set; }
    public Guid PrestadorId { get; set; }
    public Guid LojaId { get; set; }
}

public class HorarioDisponivelDto
{
    public Guid Id { get; set; }
    public DiaSemana DiaSemana { get; set; }
    public string DiaSemanaTexto { get; set; } = string.Empty;
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFim { get; set; }
    public string HorarioFormatado { get; set; } = string.Empty;
    public int DuracaoEmMinutos { get; set; }
    public string DuracaoFormatada { get; set; } = string.Empty;
    public Guid PrestadorId { get; set; }
    public string? PrestadorNome { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public bool Ativo { get; set; }
}

public class HorarioDisponivelResumoDto
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

public class HorarioDisponivelFiltroDto
{
    public Guid? PrestadorId { get; set; }
    public DiaSemana? DiaSemana { get; set; }
    public TimeOnly? HoraMinima { get; set; }
    public TimeOnly? HoraMaxima { get; set; }
    public bool ApenasAtivos { get; set; } = true;
}

public class HorarioEstatisticasDto
{
    public int TotalHorarios { get; set; }
    public int HorariosPorDia { get; set; }
    public TimeOnly HorarioInicioMaisEarly { get; set; }
    public TimeOnly HorarioFimMaisTarde { get; set; }
    public int DuracaoMediaMinutos { get; set; }
    public string DuracaoMediaFormatada { get; set; } = string.Empty;
    public Dictionary<DiaSemana, int> HorariosPorDiaSemana { get; set; } = new();
}

public class ResultadoCriacaoMultiplaDto
{
    public int HorariosCriados { get; set; }
    public List<string> Conflitos { get; set; } = new();
    public List<HorarioDisponivelDto> HorariosAdicionados { get; set; } = new();
} 