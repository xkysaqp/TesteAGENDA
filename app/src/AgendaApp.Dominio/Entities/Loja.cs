using AgendaApp.Dominio.ValueObjects;
using AgendaApp.Dominio.Enums;

namespace AgendaApp.Dominio.Entities;

public class Loja : BaseEntity
{
    public string Nome { get; private set; }

    public string Slug { get; private set; }

    public Cnpj CNPJ { get; private set; }

    public Email EmailContato { get; private set; }

    public string TelefoneContato { get; private set; }

    public string? Endereco { get; private set; }

    public string? Descricao { get; private set; }

    public string? LogoUrl { get; private set; }

    public bool Ativa { get; private set; }

    public DateTime DataVencimento { get; private set; }

    public TipoPlano Plano { get; private set; }

    public virtual ICollection<Servico> Servicos { get; private set; } = new List<Servico>();
    public virtual ICollection<HorarioDisponivel> HorariosDisponiveis { get; private set; } = new List<HorarioDisponivel>();

    protected Loja() : base()
    {
        Nome = string.Empty;
        Slug = string.Empty;
        CNPJ = null!;
        EmailContato = null!;
        TelefoneContato = string.Empty;
        Plano = TipoPlano.Basico;
    }

    public Loja(
        string nome,
        string slug,
        string cnpj,
        string emailContato,
        string telefoneContato,
        TipoPlano plano = TipoPlano.Basico)
    {
        ValidarDados(nome, slug, emailContato, telefoneContato, plano);

        Nome = nome.Trim();
        Slug = slug.ToLowerInvariant().Trim();
        CNPJ = Cnpj.Criar(cnpj);
        EmailContato = Email.Criar(emailContato);
        TelefoneContato = telefoneContato.Trim();
        Plano = plano;
        Ativa = true;
        DataVencimento = DateTime.UtcNow.AddMonths(1); // 1 mês grátis
    }

    public void AtualizarInformacoes(string nome, string emailContato, string telefoneContato)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome da loja é obrigatório");

        if (string.IsNullOrWhiteSpace(emailContato))
            throw new ArgumentException("Email de contato é obrigatório");

        if (string.IsNullOrWhiteSpace(telefoneContato))
            throw new ArgumentException("Telefone de contato é obrigatório");

        Nome = nome.Trim();
        EmailContato = Email.Criar(emailContato);
        TelefoneContato = telefoneContato.Trim();
        MarcarComoAtualizada();
    }

    public void AtualizarCNPJ(string cnpj)
    {
        CNPJ = Cnpj.Criar(cnpj);
        MarcarComoAtualizada();
    }

    public void DefinirInformacoesComplementares(string? endereco, string? descricao, string? logoUrl)
    {
        Endereco = endereco?.Trim();
        Descricao = descricao?.Trim();
        LogoUrl = logoUrl?.Trim();
        MarcarComoAtualizada();
    }

    public new void Ativar()
    {
        Ativa = true;
        MarcarComoAtualizada();
    }

    public new void Desativar()
    {
        Ativa = false;
        MarcarComoAtualizada();
    }

    public void AtualizarPlano(TipoPlano novoPlano)
    {
        if (!Enum.IsDefined(typeof(TipoPlano), novoPlano))
            throw new ArgumentException("Plano inválido");

        Plano = novoPlano;
        MarcarComoAtualizada();
    }

    public void RenovarAssinatura(int meses = 1)
    {
        DataVencimento = DataVencimento > DateTime.UtcNow
            ? DataVencimento.AddMonths(meses)
            : DateTime.UtcNow.AddMonths(meses);
        MarcarComoAtualizada();
    }

    public bool AssinaturaVencida => DataVencimento < DateTime.UtcNow;

    public bool PodeOperar => Ativa && !AssinaturaVencida;

    private static void ValidarDados(string nome, string slug, string emailContato, string telefoneContato, TipoPlano plano)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome da loja é obrigatório");

        if (nome.Length < 2 || nome.Length > 100)
            throw new ArgumentException("Nome da loja deve ter entre 2 e 100 caracteres");

        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("Slug é obrigatório");

        if (!ValidarSlug(slug))
            throw new ArgumentException("Slug deve conter apenas letras, números e hífens, sem espaços ou caracteres especiais");

        if (string.IsNullOrWhiteSpace(emailContato))
            throw new ArgumentException("Email de contato é obrigatório");

        if (string.IsNullOrWhiteSpace(telefoneContato))
            throw new ArgumentException("Telefone de contato é obrigatório");

        if (!Enum.IsDefined(typeof(TipoPlano), plano))
            throw new ArgumentException("Plano é obrigatório");
    }

    private static bool ValidarSlug(string slug)
    {
        if (slug.Length < 3 || slug.Length > 50)
            return false;

        // Slug deve começar com letra, pode conter letras, números e hífens
        return System.Text.RegularExpressions.Regex.IsMatch(slug, @"^[a-z][a-z0-9\-]*[a-z0-9]$");
    }
} 