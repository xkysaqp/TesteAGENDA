namespace AgendaApp.Dominio.Entities;

public class AgendamentoEstatisticas
{
    public int TotalAgendamentos { get; set; }

    public int AgendamentosPendentes { get; set; }

    public int AgendamentosConfirmados { get; set; }

    public int AgendamentosCancelados { get; set; }

    public int AgendamentosConcluidos { get; set; }

    public decimal ReceitaTotal { get; set; }

    public decimal ReceitaMedia { get; set; }

    public decimal TaxaConversao => TotalAgendamentos > 0 ? (decimal)AgendamentosConfirmados / TotalAgendamentos * 100 : 0;

    public decimal TaxaCancelamento => TotalAgendamentos > 0 ? (decimal)AgendamentosCancelados / TotalAgendamentos * 100 : 0;
} 