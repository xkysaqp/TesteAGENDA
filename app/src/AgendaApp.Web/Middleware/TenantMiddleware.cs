using AgendaApp.Dominio.Interfaces;
using AgendaApp.Infraestrutura.Data;
using Microsoft.EntityFrameworkCore;

namespace AgendaApp.Web.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantMiddleware> _logger;

    public TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, AgendaAppDbContext dbContext, ILojaRepository lojaRepository)
    {
        try
        {
            var lojaId = await DetectarLojaAtual(context, lojaRepository);
            
            if (lojaId.HasValue)
            {
                dbContext.LojaAtualId = lojaId;
                
                context.Items["LojaId"] = lojaId;
                context.Items["IsMultitenant"] = true;
                
                _logger.LogDebug("Loja detectada: {LojaId}", lojaId);
            }
            else
            {
                dbContext.LojaAtualId = null;
                context.Items["IsMultitenant"] = false;
                _logger.LogDebug("Modo admin detectado - sem filtro de loja");
            }

            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no middleware de detecção de tenant");
            await _next(context);
        }
    }

    private async Task<Guid?> DetectarLojaAtual(HttpContext context, ILojaRepository lojaRepository)
    {
        var slug = ExtrairSlugDaUrl(context.Request.Path);
        if (!string.IsNullOrEmpty(slug))
        {
            var loja = await lojaRepository.ObterPorSlugAsync(slug);
            return loja?.Id;
        }

        var subdomain = ExtrairSubdominio(context.Request.Host.Host);
        if (!string.IsNullOrEmpty(subdomain))
        {
            var loja = await lojaRepository.ObterPorSlugAsync(subdomain);
            return loja?.Id;
        }

        if (context.User.Identity?.IsAuthenticated == true)
        {
            var lojaIdClaim = context.User.FindFirst("LojaId")?.Value;
            if (!string.IsNullOrEmpty(lojaIdClaim) && Guid.TryParse(lojaIdClaim, out var lojaId))
            {
                return lojaId;
            }
        }

        if (context.Request.Query.ContainsKey("lojaId"))
        {
            var queryLojaId = context.Request.Query["lojaId"].ToString();
            if (Guid.TryParse(queryLojaId, out var lojaId))
            {
                return lojaId;
            }
        }

        return null;
    }

    private static string? ExtrairSlugDaUrl(string path)
    {
        var segments = path.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
        
        if (segments.Length >= 2)
        {
            if (segments[0].Equals("loja", StringComparison.OrdinalIgnoreCase) ||
                segments[0].Equals("public", StringComparison.OrdinalIgnoreCase))
            {
                return segments[1];
            }
            
            if (segments.Length >= 2 && 
                segments[1].Equals("agendamento", StringComparison.OrdinalIgnoreCase))
            {
                return segments[0];
            }
        }

        return null;
    }

    private static string? ExtrairSubdominio(string host)
    {
        if (string.IsNullOrEmpty(host)) return null;

        host = host.Split(':')[0];

        var parts = host.Split('.');
        
        if (parts.Length >= 3 && !host.Contains("localhost"))
        {
            var subdomain = parts[0];
            
            if (!IsSubdomainComum(subdomain))
            {
                return subdomain;
            }
        }

        return null;
    }

    private static bool IsSubdomainComum(string subdomain)
    {
        var subdomainsPadrão = new[] { "www", "api", "admin", "app", "mail", "ftp" };
        return subdomainsPadrão.Contains(subdomain.ToLowerInvariant());
    }
}

public static class TenantMiddlewareExtensions
{
    public static IApplicationBuilder UseTenantDetection(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TenantMiddleware>();
    }

    public static Guid? GetLojaId(this HttpContext context)
    {
        return context.Items["LojaId"] as Guid?;
    }

    public static bool IsMultitenant(this HttpContext context)
    {
        return context.Items["IsMultitenant"] as bool? ?? false;
    }

    public static bool IsAdminMode(this HttpContext context)
    {
        return !context.IsMultitenant();
    }
} 