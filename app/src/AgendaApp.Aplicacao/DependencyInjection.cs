using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using AgendaApp.Aplicacao.Interfaces;
using AgendaApp.Aplicacao.Services;

namespace AgendaApp.Aplicacao;

public static class DependencyInjection
{
    public static IServiceCollection AddAplicacao(this IServiceCollection services)
    {
        services.AddScoped<IPrestadorService, PrestadorService>();
        services.AddScoped<IServicoService, ServicoService>();
        services.AddScoped<IPrestadorServicoService, PrestadorServicoService>();
        services.AddScoped<IHorarioDisponivelService, HorarioDisponivelService>();
        services.AddScoped<ILojaService, LojaService>();
        services.AddScoped<IAgendamentoService, AgendamentoService>();

        return services;
    }
} 