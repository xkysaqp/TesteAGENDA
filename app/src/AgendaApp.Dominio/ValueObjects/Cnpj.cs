using System.Text.RegularExpressions;

namespace AgendaApp.Dominio.ValueObjects;

public sealed class Cnpj
{
    private static readonly Regex CnpjRegex = new(@"^\d{14}$", RegexOptions.Compiled);

    public string Numero { get; private set; }

    private Cnpj(string numero)
    {
        Numero = numero;
    }

    public static Cnpj Criar(string numero)
    {
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("CNPJ não pode ser vazio", nameof(numero));

        numero = Regex.Replace(numero, @"\D", "");

        if (!CnpjRegex.IsMatch(numero))
            throw new ArgumentException("CNPJ deve conter exatamente 14 dígitos", nameof(numero));

        if (!ValidarDigitoVerificador(numero))
            throw new ArgumentException("CNPJ possui dígito verificador inválido", nameof(numero));

        return new Cnpj(numero);
    }

    private static bool ValidarDigitoVerificador(string cnpj)
    {
        int[] multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        string tempCnpj = cnpj.Substring(0, 12);
        int soma = 0;

        for (int i = 0; i < 12; i++)
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

        int resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;

        string digito = resto.ToString();
        tempCnpj += digito;
        soma = 0;

        for (int i = 0; i < 13; i++)
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;
        digito += resto.ToString();

        return cnpj.EndsWith(digito);
    }

    public string Formatado =>
        $"{Numero.Substring(0, 2)}.{Numero.Substring(2, 3)}.{Numero.Substring(5, 3)}/{Numero.Substring(8, 4)}-{Numero.Substring(12, 2)}";

    public override string ToString() => Formatado;

    public override bool Equals(object? obj)
    {
        return obj is Cnpj other && Numero == other.Numero;
    }

    public override int GetHashCode() => Numero.GetHashCode();

    public static implicit operator string(Cnpj cnpj) => cnpj.Numero;
} 