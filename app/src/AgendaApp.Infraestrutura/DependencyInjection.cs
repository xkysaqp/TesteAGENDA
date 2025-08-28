using AgendaApp.Infraestrutura.Data;
using AgendaApp.Infraestrutura.Repositories;
using AgendaApp.Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AgendaApp.Infraestrutura;

public static class DependencyInjection
{
    public static IServiceCollection AddInfraestrutura(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddDbContext<AgendaAppDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(AgendaAppDbContext).Assembly.FullName);
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
            });
            
            options.EnableSensitiveDataLogging(true);
            options.EnableDetailedErrors(true);
        });

        services.AddScoped<IPrestadorRepository, PrestadorRepository>();
        services.AddScoped<IServicoRepository, ServicoRepository>();
        services.AddScoped<IHorarioDisponivelRepository, HorarioDisponivelRepository>();
        services.AddScoped<ILojaRepository, LojaRepository>();
        
        services.AddScoped<IRepository<AgendaApp.Dominio.Entities.PrestadorServico>, PrestadorServicoRepository>();
        services.AddScoped<IRepository<AgendaApp.Dominio.Entities.Servico>>(provider => provider.GetService<IServicoRepository>()!);
        services.AddScoped<IRepository<AgendaApp.Dominio.Entities.Prestador>>(provider => provider.GetService<IPrestadorRepository>()!);

        return services;
    }
} 