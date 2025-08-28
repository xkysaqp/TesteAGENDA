namespace AgendaApp.Aplicacao.DTOs;

public class CriarPrestadorDto
{
    public string Nome { get; set; } = string.Empty;
    public string? CNPJ { get; set; }
    public string AreaAtuacao { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public Guid LojaId { get; set; }
}

public class AtualizarPrestadorDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? CNPJ { get; set; }
    public string AreaAtuacao { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public Guid LojaId { get; set; }
}

public class PrestadorDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? CNPJ { get; set; }
    public string? CNPJFormatado { get; set; }
    public string AreaAtuacao { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public bool Ativo { get; set; }
    public Guid LojaId { get; set; }
}

public class PrestadorResumoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? CNPJFormatado { get; set; }
    public string AreaAtuacao { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
    public bool Ativo { get; set; }
    public int TotalServicos { get; set; }
    public int TotalHorarios { get; set; }
}

public class PrestadorFiltroDto
{
    public string? Nome { get; set; }
    public string? AreaAtuacao { get; set; }
    public string? Email { get; set; }
    public bool ApenasAtivos { get; set; } = true;
    public Guid? LojaId { get; set; }
} 