namespace AgendaApp.Dominio.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public DateTime DataCriacao { get; protected set; } = DateTime.UtcNow;

    public DateTime? DataAtualizacao { get; protected set; }

    public bool Ativo { get; protected set; } = true;

    public void MarcarComoAtualizada()
    {
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Desativar()
    {
        Ativo = false;
        MarcarComoAtualizada();
    }

    public void Ativar()
    {
        Ativo = true;
        MarcarComoAtualizada();
    }
} 