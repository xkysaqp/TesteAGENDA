namespace AgendaApp.Dominio.Entities;

public class Servico : BaseEntity
{
    public string Nome { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;
    public decimal Valor { get; private set; }
    public int DuracaoEmMinutos { get; private set; }
    
    public Guid LojaId { get; private set; }

    public virtual Loja Loja { get; private set; } = null!;
    
    public virtual ICollection<PrestadorServico> PrestadorServicos { get; private set; } = new List<PrestadorServico>();

    protected Servico() { }

    public Servico(string nome, string descricao, decimal valor, int duracaoEmMinutos, Guid lojaId)
    {
        if (lojaId == Guid.Empty)
            throw new ArgumentException("ID da loja é obrigatório", nameof(lojaId));

        ValidarEDefinirNome(nome);
        ValidarEDefinirDescricao(descricao);
        ValidarEDefinirValor(valor);
        ValidarEDefinirDuracao(duracaoEmMinutos);
        LojaId = lojaId;
    }

    public bool EhServicoGlobal => !PrestadorServicos.Any();

    public bool EstaVinculadoAoPrestador(Guid prestadorId)
    {
        return PrestadorServicos.Any(ps => ps.PrestadorId == prestadorId);
    }

    public decimal ObterValorParaPrestador(Guid prestadorId)
    {
        var prestadorServico = PrestadorServicos.FirstOrDefault(ps => ps.PrestadorId == prestadorId);
        return prestadorServico?.ValorPersonalizado ?? Valor;
    }

    public int ObterDuracaoParaPrestador(Guid prestadorId)
    {
        var prestadorServico = PrestadorServicos.FirstOrDefault(ps => ps.PrestadorId == prestadorId);
        return prestadorServico?.DuracaoPersonalizadaEmMinutos ?? DuracaoEmMinutos;
    }

    public void AtualizarInformacoes(string nome, string descricao, decimal valor, int duracaoEmMinutos)
    {
        ValidarEDefinirNome(nome);
        ValidarEDefinirDescricao(descricao);
        ValidarEDefinirValor(valor);
        ValidarEDefinirDuracao(duracaoEmMinutos);
        MarcarComoAtualizada();
    }

    public void AtualizarValor(decimal novoValor)
    {
        ValidarEDefinirValor(novoValor);
        MarcarComoAtualizada();
    }

    public void AtualizarDuracao(int novaDuracaoEmMinutos)
    {
        ValidarEDefinirDuracao(novaDuracaoEmMinutos);
        MarcarComoAtualizada();
    }

    public TimeOnly CalcularHoraFim(TimeOnly horaInicio)
    {
        return horaInicio.AddMinutes(DuracaoEmMinutos);
    }

    #region Métodos Privados de Validação

    private void ValidarEDefinirNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do serviço é obrigatório", nameof(nome));

        if (nome.Length < 2)
            throw new ArgumentException("Nome do serviço deve ter pelo menos 2 caracteres", nameof(nome));

        if (nome.Length > 100)
            throw new ArgumentException("Nome do serviço deve ter no máximo 100 caracteres", nameof(nome));

        Nome = nome.Trim();
    }

    private void ValidarEDefinirDescricao(string descricao)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("Descrição do serviço é obrigatória", nameof(descricao));

        if (descricao.Length > 500)
            throw new ArgumentException("Descrição deve ter no máximo 500 caracteres", nameof(descricao));

        Descricao = descricao.Trim();
    }

    private void ValidarEDefinirValor(decimal valor)
    {
        if (valor < 0)
            throw new ArgumentException("Valor do serviço não pode ser negativo", nameof(valor));

        if (valor > 999999.99m)
            throw new ArgumentException("Valor do serviço não pode ser superior a R$ 999.999,99", nameof(valor));

        Valor = valor;
    }

    private void ValidarEDefinirDuracao(int duracaoEmMinutos)
    {
        if (duracaoEmMinutos <= 0)
            throw new ArgumentException("Duração deve ser maior que zero", nameof(duracaoEmMinutos));

        if (duracaoEmMinutos > 1440)
            throw new ArgumentException("Duração não pode ser superior a 24 horas (1440 minutos)", nameof(duracaoEmMinutos));

        DuracaoEmMinutos = duracaoEmMinutos;
    }

    #endregion
} 