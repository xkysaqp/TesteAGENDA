using AgendaApp.Dominio.Entities;

namespace AgendaApp.Aplicacao.DTOs;

public class AgendamentoDto
{
    public Guid Id { get; set; }
    public DateOnly Data { get; set; }
    public TimeOnly Hora { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public string ClienteTelefone { get; set; } = string.Empty;
    public string? ClienteEmail { get; set; }
    public string? Observacoes { get; set; }
    public StatusAgendamento Status { get; set; }
    public decimal? Valor { get; set; }
    public DateTime? DataConfirmacao { get; set; }
    public DateTime? DataCancelamento { get; set; }
    public string? MotivoCancelamento { get; set; }
    public Guid LojaId { get; set; }
    public Guid ServicoId { get; set; }
    public Guid PrestadorId { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    
    // Propriedades de navegação
    public string LojaNome { get; set; } = string.Empty;
    public string ServicoNome { get; set; } = string.Empty;
    public string PrestadorNome { get; set; } = string.Empty;
    public decimal ServicoValor { get; set; }
    public int ServicoDuracaoEmMinutos { get; set; }
}

public class CriarAgendamentoDto
{
    public DateOnly Data { get; set; }
    public TimeOnly Hora { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public string ClienteTelefone { get; set; } = string.Empty;
    public string? ClienteEmail { get; set; }
    public string? Observacoes { get; set; }
    public Guid LojaId { get; set; }
    public Guid ServicoId { get; set; }
    public Guid PrestadorId { get; set; }
}

public class AtualizarAgendamentoDto
{
    public Guid Id { get; set; }
    public DateOnly? Data { get; set; }
    public TimeOnly? Hora { get; set; }
    public string? ClienteNome { get; set; }
    public string? ClienteTelefone { get; set; }
    public string? ClienteEmail { get; set; }
    public string? Observacoes { get; set; }
}

public class AgendamentoFiltroDto
{
    public Guid? LojaId { get; set; }
    public Guid? PrestadorId { get; set; }
    public Guid? ServicoId { get; set; }
    public StatusAgendamento? Status { get; set; }
    public DateOnly? DataInicio { get; set; }
    public DateOnly? DataFim { get; set; }
    public string? ClienteNome { get; set; }
    public bool ApenasAtivos { get; set; } = true;
}

public class AgendamentoEstatisticasDto
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
