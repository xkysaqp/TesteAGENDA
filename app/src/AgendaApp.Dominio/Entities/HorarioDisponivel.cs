using AgendaApp.Dominio.Enums;

namespace AgendaApp.Dominio.Entities;

public class HorarioDisponivel : BaseEntity
{
    public DiaSemana DiaSemana { get; private set; }
    public TimeOnly HoraInicio { get; private set; }
    public TimeOnly HoraFim { get; private set; }
    
    public Guid LojaId { get; private set; }

    public virtual Loja? Loja { get; private set; }
    public Guid PrestadorId { get; private set; }
    public Prestador Prestador { get; private set; } = null!;

    protected HorarioDisponivel() { }

    public HorarioDisponivel(DiaSemana diaSemana, TimeOnly horaInicio, TimeOnly horaFim, Guid prestadorId, Guid lojaId)
    {
        if (prestadorId == Guid.Empty)
            throw new ArgumentException("ID do prestador é obrigatório", nameof(prestadorId));
        
        if (lojaId == Guid.Empty)
            throw new ArgumentException("ID da loja é obrigatório", nameof(lojaId));

        ValidarEDefinirDiaSemana(diaSemana);
        ValidarEDefinirHorarios(horaInicio, horaFim);
        PrestadorId = prestadorId;
        LojaId = lojaId;
    }

    public void AtualizarHorario(DiaSemana diaSemana, TimeOnly horaInicio, TimeOnly horaFim)
    {
        ValidarEDefinirDiaSemana(diaSemana);
        ValidarEDefinirHorarios(horaInicio, horaFim);
        MarcarComoAtualizada();
    }

    public bool EstaDisponivel(DiaSemana diaSemana, TimeOnly horario)
    {
        return DiaSemana == diaSemana && 
               horario >= HoraInicio && 
               horario < HoraFim;
    }

    public bool PeriodoEstaDisponivel(DiaSemana diaSemana, TimeOnly horaInicio, TimeOnly horaFim)
    {
        return DiaSemana == diaSemana &&
               horaInicio >= HoraInicio &&
               horaFim <= HoraFim;
    }

    public int DuracaoEmMinutos()
    {
        var duracao = HoraFim - HoraInicio;
        return (int)duracao.TotalMinutes;
    }

    public override string ToString()
    {
        var diaSemanaTexto = DiaSemana switch
        {
            DiaSemana.Domingo => "Domingo",
            DiaSemana.Segunda => "Segunda-feira",
            DiaSemana.Terca => "Terça-feira",
            DiaSemana.Quarta => "Quarta-feira",
            DiaSemana.Quinta => "Quinta-feira",
            DiaSemana.Sexta => "Sexta-feira",
            DiaSemana.Sabado => "Sábado",
            _ => "Dia inválido"
        };

        return $"{diaSemanaTexto}: {HoraInicio:HH:mm} às {HoraFim:HH:mm}";
    }

    #region Métodos Privados de Validação

    private void ValidarEDefinirDiaSemana(DiaSemana diaSemana)
    {
        if (!Enum.IsDefined(typeof(DiaSemana), diaSemana))
            throw new ArgumentException("Dia da semana inválido", nameof(diaSemana));

        DiaSemana = diaSemana;
    }

    private void ValidarEDefinirHorarios(TimeOnly horaInicio, TimeOnly horaFim)
    {
        if (horaInicio >= horaFim)
            throw new ArgumentException("Hora de início deve ser anterior à hora de fim");

        if (horaInicio < new TimeOnly(6, 0) || horaFim > new TimeOnly(23, 59))
            throw new ArgumentException("Horário deve estar entre 06:00 e 23:59");

        var duracao = horaFim - horaInicio;
        if (duracao.TotalMinutes < 30)
            throw new ArgumentException("Período disponível deve ter pelo menos 30 minutos");

        HoraInicio = horaInicio;
        HoraFim = horaFim;
    }

    #endregion
} 