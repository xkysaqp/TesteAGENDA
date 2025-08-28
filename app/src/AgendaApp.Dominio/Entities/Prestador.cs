using AgendaApp.Dominio.ValueObjects;

namespace AgendaApp.Dominio.Entities;

public class Prestador : BaseEntity
{
    public string Nome { get; private set; } = string.Empty;
    public Cnpj? CNPJ { get; private set; }
    public string AreaAtuacao { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public string Telefone { get; private set; } = string.Empty;
    
    public Guid LojaId { get; private set; }

    public virtual Loja? Loja { get; private set; }
    
    private readonly List<Servico> _servicos = new();
    public IReadOnlyCollection<Servico> Servicos => _servicos.AsReadOnly();

    private readonly List<PrestadorServico> _prestadorServicos = new();
    public IReadOnlyCollection<PrestadorServico> PrestadorServicos => _prestadorServicos.AsReadOnly();

    private readonly List<HorarioDisponivel> _horariosDisponiveis = new();
    public IReadOnlyCollection<HorarioDisponivel> HorariosDisponiveis => _horariosDisponiveis.AsReadOnly();

    protected Prestador() { }

    public Prestador(string nome, string areaAtuacao, string email, string telefone, Guid lojaId, string? cnpj = null)
    {
        if (lojaId == Guid.Empty)
            throw new ArgumentException("ID da loja é obrigatório", nameof(lojaId));

        ValidarEDefinirNome(nome);
        ValidarEDefinirAreaAtuacao(areaAtuacao);
        ValidarEDefinirEmail(email);
        ValidarEDefinirTelefone(telefone);
        
        LojaId = lojaId;
        
        if (!string.IsNullOrWhiteSpace(cnpj))
        {
            DefinirCNPJ(cnpj);
        }
    }

    public void AtualizarInformacoes(string nome, string areaAtuacao, string telefone)
    {
        ValidarEDefinirNome(nome);
        ValidarEDefinirAreaAtuacao(areaAtuacao);
        ValidarEDefinirTelefone(telefone);
        MarcarComoAtualizada();
    }

    public void AtualizarEmail(string novoEmail)
    {
        ValidarEDefinirEmail(novoEmail);
        MarcarComoAtualizada();
    }

    public void DefinirCNPJ(string cnpj)
    {
        CNPJ = Cnpj.Criar(cnpj);
        MarcarComoAtualizada();
    }

    public void RemoverCNPJ()
    {
        CNPJ = null;
        MarcarComoAtualizada();
    }

    public void AdicionarServico(Servico servico)
    {
        if (servico == null)
            throw new ArgumentNullException(nameof(servico));

        if (_servicos.Any(s => s.Nome.Equals(servico.Nome, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Já existe um serviço com este nome para este prestador");

        _servicos.Add(servico);
        MarcarComoAtualizada();
    }

    public void RemoverServico(Guid servicoId)
    {
        var servico = _servicos.FirstOrDefault(s => s.Id == servicoId);
        if (servico != null)
        {
            _servicos.Remove(servico);
            MarcarComoAtualizada();
        }
    }

    public void AdicionarHorarioDisponivel(HorarioDisponivel horario)
    {
        if (horario == null)
            throw new ArgumentNullException(nameof(horario));

        var conflito = _horariosDisponiveis.Any(h => 
            h.DiaSemana == horario.DiaSemana &&
            ((horario.HoraInicio >= h.HoraInicio && horario.HoraInicio < h.HoraFim) ||
             (horario.HoraFim > h.HoraInicio && horario.HoraFim <= h.HoraFim) ||
             (horario.HoraInicio <= h.HoraInicio && horario.HoraFim >= h.HoraFim)));

        if (conflito)
            throw new InvalidOperationException("Existe conflito com horário já cadastrado");

        _horariosDisponiveis.Add(horario);
        MarcarComoAtualizada();
    }

    public void RemoverHorarioDisponivel(Guid horarioId)
    {
        var horario = _horariosDisponiveis.FirstOrDefault(h => h.Id == horarioId);
        if (horario != null)
        {
            _horariosDisponiveis.Remove(horario);
            MarcarComoAtualizada();
        }
    }

    private void ValidarEDefinirNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório", nameof(nome));

        if (nome.Length < 2)
            throw new ArgumentException("Nome deve ter pelo menos 2 caracteres", nameof(nome));

        if (nome.Length > 200)
            throw new ArgumentException("Nome deve ter no máximo 200 caracteres", nameof(nome));

        Nome = nome.Trim();
    }

    private void ValidarEDefinirAreaAtuacao(string areaAtuacao)
    {
        if (string.IsNullOrWhiteSpace(areaAtuacao))
            throw new ArgumentException("Área de atuação é obrigatória", nameof(areaAtuacao));

        if (areaAtuacao.Length > 100)
            throw new ArgumentException("Área de atuação deve ter no máximo 100 caracteres", nameof(areaAtuacao));

        AreaAtuacao = areaAtuacao.Trim();
    }

    private void ValidarEDefinirEmail(string email)
    {
        Email = Email.Criar(email);
    }

    private void ValidarEDefinirTelefone(string telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone))
            throw new ArgumentException("Telefone é obrigatório", nameof(telefone));

        // Remove caracteres não numéricos para validação
        var telefoneNumerico = System.Text.RegularExpressions.Regex.Replace(telefone, @"\D", "");
        
        if (telefoneNumerico.Length < 10 || telefoneNumerico.Length > 11)
            throw new ArgumentException("Telefone deve ter 10 ou 11 dígitos", nameof(telefone));

        Telefone = telefone.Trim();
    }
} 