namespace AgendaApp.Aplicacao.DTOs;

public class CriarServicoDto
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int DuracaoEmMinutos { get; set; }
    public Guid? PrestadorId { get; set; }
    public Guid? LojaId { get; set; }
}

public class AtualizarServicoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int DuracaoEmMinutos { get; set; }
    public Guid? PrestadorId { get; set; }
    public bool Ativo { get; set; } = true;
    public Guid? LojaId { get; set; }
}

public class ServicoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string ValorFormatado { get; set; } = string.Empty;
    public int DuracaoEmMinutos { get; set; }
    public string DuracaoFormatada { get; set; } = string.Empty;
    public Guid? PrestadorId { get; set; }
    public string? PrestadorNome { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public bool Ativo { get; set; }
    public Guid? LojaId { get; set; }
}

public class ServicoResumoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string ValorFormatado { get; set; } = string.Empty;
    public int DuracaoEmMinutos { get; set; }
    public string DuracaoFormatada { get; set; } = string.Empty;
    public Guid? PrestadorId { get; set; }
    public string? PrestadorNome { get; set; }
    public DateTime DataCriacao { get; set; }
    public bool Ativo { get; set; }
    public Guid? LojaId { get; set; }
}

public class ServicoFiltroDto
{
    public string? Nome { get; set; }
    public Guid? PrestadorId { get; set; }
    public decimal? ValorMinimo { get; set; }
    public decimal? ValorMaximo { get; set; }
    public int? DuracaoMaxima { get; set; }
    public bool ApenasAtivos { get; set; } = true;
    public Guid? LojaId { get; set; }
}

public class ServicoEstatisticasDto
{
    public int TotalServicos { get; set; }
    public decimal ValorMedio { get; set; }
    public decimal ValorMinimo { get; set; }
    public decimal ValorMaximo { get; set; }
    public int DuracaoMediaMinutos { get; set; }
    public string DuracaoMediaFormatada { get; set; } = string.Empty;
} 