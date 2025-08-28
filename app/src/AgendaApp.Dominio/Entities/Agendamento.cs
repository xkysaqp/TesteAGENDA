using AgendaApp.Dominio.ValueObjects;

namespace AgendaApp.Dominio.Entities;

public class Agendamento : BaseEntity
{
    public DateOnly Data { get; private set; }

    public TimeOnly Hora { get; private set; }

    public string ClienteNome { get; private set; }

    public string ClienteTelefone { get; private set; }

    public Email? ClienteEmail { get; private set; }

    public string? Observacoes { get; private set; }

    public StatusAgendamento Status { get; private set; }

    public decimal? Valor { get; private set; }

    public DateTime? DataConfirmacao { get; private set; }

    public DateTime? DataCancelamento { get; private set; }

    public string? MotivoCancelamento { get; private set; }

    public Guid LojaId { get; private set; }

    public Guid ServicoId { get; private set; }

    public Guid PrestadorId { get; private set; }

    public virtual Loja Loja { get; private set; } = null!;
    public virtual Servico Servico { get; private set; } = null!;
    public virtual Prestador Prestador { get; private set; } = null!;

    protected Agendamento() : base()
    {
        ClienteNome = string.Empty;
        ClienteTelefone = string.Empty;
        Status = StatusAgendamento.Pendente;
    }

    public Agendamento(
        DateOnly data,
        TimeOnly hora,
        string clienteNome,
        string clienteTelefone,
        Guid lojaId,
        Guid servicoId,
        Guid prestadorId,
        string? clienteEmail = null,
        string? observacoes = null)
    {
        ValidarDados(data, hora, clienteNome, clienteTelefone, lojaId, servicoId, prestadorId);

        Data = data;
        Hora = hora;
        ClienteNome = clienteNome.Trim();
        ClienteTelefone = clienteTelefone.Trim();
        LojaId = lojaId;
        ServicoId = servicoId;
        PrestadorId = prestadorId;
        Status = StatusAgendamento.Pendente;

        if (!string.IsNullOrWhiteSpace(clienteEmail))
        {
            ClienteEmail = Email.Criar(clienteEmail);
        }

        if (!string.IsNullOrWhiteSpace(observacoes))
        {
            Observacoes = observacoes.Trim();
        }
    }

    public void Confirmar(decimal? valor = null)
    {
        if (Status != StatusAgendamento.Pendente)
            throw new InvalidOperationException("Apenas agendamentos pendentes podem ser confirmados");

        Status = StatusAgendamento.Confirmado;
        DataConfirmacao = DateTime.UtcNow;
        
        if (valor.HasValue && valor.Value > 0)
        {
            Valor = valor.Value;
        }

        MarcarComoAtualizada();
    }

    public void Cancelar(string motivo)
    {
        if (Status == StatusAgendamento.Cancelado)
            throw new InvalidOperationException("Agendamento já está cancelado");

        if (Status == StatusAgendamento.Concluido)
            throw new InvalidOperationException("Não é possível cancelar um agendamento já concluído");

        Status = StatusAgendamento.Cancelado;
        DataCancelamento = DateTime.UtcNow;
        MotivoCancelamento = motivo?.Trim();
        MarcarComoAtualizada();
    }

    public void Concluir(decimal? valorFinal = null)
    {
        if (Status != StatusAgendamento.Confirmado)
            throw new InvalidOperationException("Apenas agendamentos confirmados podem ser concluídos");

        Status = StatusAgendamento.Concluido;
        
        if (valorFinal.HasValue && valorFinal.Value > 0)
        {
            Valor = valorFinal.Value;
        }

        MarcarComoAtualizada();
    }

    public void AtualizarCliente(string nome, string telefone, string? email = null)
    {
        if (Status == StatusAgendamento.Concluido)
            throw new InvalidOperationException("Não é possível alterar dados de agendamento concluído");

        ValidarDadosCliente(nome, telefone);

        ClienteNome = nome.Trim();
        ClienteTelefone = telefone.Trim();

        if (!string.IsNullOrWhiteSpace(email))
        {
            ClienteEmail = Email.Criar(email);
        }
        else
        {
            ClienteEmail = null;
        }
        
        MarcarComoAtualizada();
    }

    public void AtualizarDataHora(DateOnly novaData, TimeOnly novaHora)
    {
        if (Status == StatusAgendamento.Concluido)
            throw new InvalidOperationException("Não é possível alterar data/hora de agendamento concluído");

        if (Status == StatusAgendamento.Cancelado)
            throw new InvalidOperationException("Não é possível alterar data/hora de agendamento cancelado");

        ValidarDataHora(novaData, novaHora);

        Data = novaData;
        Hora = novaHora;
        MarcarComoAtualizada();
    }

    public void AtualizarObservacoes(string? observacoes)
    {
        Observacoes = observacoes?.Trim();
        MarcarComoAtualizada();
    }

    public bool EhFuturo => Data > DateOnly.FromDateTime(DateTime.Today) || 
                           (Data == DateOnly.FromDateTime(DateTime.Today) && Hora > TimeOnly.FromDateTime(DateTime.Now));

    public bool PodeCancelar => Status != StatusAgendamento.Cancelado && Status != StatusAgendamento.Concluido;

    public bool PodeEditar => Status == StatusAgendamento.Pendente || Status == StatusAgendamento.Confirmado;

    public DateTime DataHoraCompleta => Data.ToDateTime(Hora);

    private static void ValidarDados(DateOnly data, TimeOnly hora, string clienteNome, string clienteTelefone, 
                                   Guid lojaId, Guid servicoId, Guid prestadorId)
    {
        ValidarDataHora(data, hora);
        ValidarDadosCliente(clienteNome, clienteTelefone);

        if (lojaId == Guid.Empty)
            throw new ArgumentException("LojaId é obrigatório");

        if (servicoId == Guid.Empty)
            throw new ArgumentException("ServicoId é obrigatório");

        if (prestadorId == Guid.Empty)
            throw new ArgumentException("PrestadorId é obrigatório");
    }

    private static void ValidarDataHora(DateOnly data, TimeOnly hora)
    {
        if (data < DateOnly.FromDateTime(DateTime.Today))
            throw new ArgumentException("Data do agendamento não pode ser no passado");

        // Verificar se não é muito no futuro (ex: máximo 1 ano)
        if (data > DateOnly.FromDateTime(DateTime.Today.AddYears(1)))
            throw new ArgumentException("Data do agendamento não pode ser mais de 1 ano no futuro");
    }

    private static void ValidarDadosCliente(string nome, string telefone)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do cliente é obrigatório");

        if (nome.Length < 2 || nome.Length > 100)
            throw new ArgumentException("Nome do cliente deve ter entre 2 e 100 caracteres");

        if (string.IsNullOrWhiteSpace(telefone))
            throw new ArgumentException("Telefone do cliente é obrigatório");

        if (telefone.Length < 10 || telefone.Length > 20)
            throw new ArgumentException("Telefone deve ter entre 10 e 20 caracteres");
    }
}

public enum StatusAgendamento
{
    Pendente = 1,

    Confirmado = 2,

    Cancelado = 3,

    Concluido = 4
} 