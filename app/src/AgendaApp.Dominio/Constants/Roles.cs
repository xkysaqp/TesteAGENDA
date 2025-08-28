namespace AgendaApp.Dominio.Constants;

public static class Roles
{
    public const string Admin = "Admin";
    public const string Loja = "Loja";

    public static readonly string[] TodasRoles = { Admin, Loja };

    public static string ObterDescricao(string role)
    {
        return role switch
        {
            Admin => "Administrador do sistema com acesso total",
            Loja => "Usuário de loja com acesso aos dados da própria loja",
            _ => "Role desconhecida"
        };
    }
} 