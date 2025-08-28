using System.Text.RegularExpressions;

namespace AgendaApp.Dominio.ValueObjects;

public sealed class Email
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Endereco { get; private set; }

    private Email(string endereco)
    {
        Endereco = endereco;
    }

    public static Email Criar(string endereco)
    {
        if (string.IsNullOrWhiteSpace(endereco))
            throw new ArgumentException("E-mail não pode ser vazio", nameof(endereco));

        endereco = endereco.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(endereco))
            throw new ArgumentException("Formato de e-mail inválido", nameof(endereco));

        return new Email(endereco);
    }

    public override string ToString() => Endereco;

    public override bool Equals(object? obj)
    {
        return obj is Email other && Endereco == other.Endereco;
    }

    public override int GetHashCode() => Endereco.GetHashCode();

    public static implicit operator string(Email email) => email.Endereco;
} 