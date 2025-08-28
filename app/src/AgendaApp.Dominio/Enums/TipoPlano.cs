namespace AgendaApp.Dominio.Enums;

public enum TipoPlano
{
    Basico = 1,
    
    Intermediario = 2,
    
    Premium = 3,
    
    Enterprise = 4
}

public static class TipoPlanoExtensions
{
    public static string ObterDescricao(this TipoPlano plano)
    {
        return plano switch
        {
            TipoPlano.Basico => "Básico",
            TipoPlano.Intermediario => "Intermediário", 
            TipoPlano.Premium => "Premium",
            TipoPlano.Enterprise => "Enterprise",
            _ => "Plano Desconhecido"
        };
    }
    
    public static decimal ObterValorMensal(this TipoPlano plano)
    {
        return plano switch
        {
            TipoPlano.Basico => 29.90m,
            TipoPlano.Intermediario => 59.90m,
            TipoPlano.Premium => 99.90m,
            TipoPlano.Enterprise => 199.90m,
            _ => 0m
        };
    }
    
    public static string ObterDescricaoCompleta(this TipoPlano plano)
    {
        return plano switch
        {
            TipoPlano.Basico => "Plano Básico - Agenda simples, até 3 prestadores",
            TipoPlano.Intermediario => "Plano Intermediário - Agenda avançada, até 10 prestadores, relatórios",
            TipoPlano.Premium => "Plano Premium - Prestadores ilimitados, relatórios avançados, integrações",
            TipoPlano.Enterprise => "Plano Enterprise - Funcionalidades completas, suporte prioritário, customizações",
            _ => "Plano não definido"
        };
    }
    
    public static int ObterLimitePrestadores(this TipoPlano plano)
    {
        return plano switch
        {
            TipoPlano.Basico => 3,
            TipoPlano.Intermediario => 10,
            TipoPlano.Premium => -1,
            TipoPlano.Enterprise => -1,
            _ => 0
        };
    }
    
    public static bool PermiteRelatoriosAvancados(this TipoPlano plano)
    {
        return plano == TipoPlano.Premium || plano == TipoPlano.Enterprise;
    }
    
    public static bool PermiteIntegracoes(this TipoPlano plano)
    {
        return plano == TipoPlano.Premium || plano == TipoPlano.Enterprise;
    }
} 