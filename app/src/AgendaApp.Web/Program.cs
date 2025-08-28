using AgendaApp.Aplicacao;
using AgendaApp.Infraestrutura;
using AgendaApp.Infraestrutura.Data;
using AgendaApp.Infraestrutura.Identity;
using AgendaApp.Web.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAplicacao();
builder.Services.AddInfraestrutura(builder.Configuration);

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;

    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<AgendaAppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

builder.Services.AddAutoMapper(typeof(Program), typeof(AgendaApp.Aplicacao.DTOs.LojaDto));

builder.Services.AddControllersWithViews();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

await ExecutarMigrationsAsync(app);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseRequestLocalization(options =>
{
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("pt-BR");
    options.SupportedCultures = new[] { new System.Globalization.CultureInfo("pt-BR") };
    options.SupportedUICultures = new[] { new System.Globalization.CultureInfo("pt-BR") };
});

app.UseAuthentication();

app.UseTenantDetection();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    try
    {
        await AgendaApp.Infraestrutura.Data.AgendaAppDbSeed.ExecutarSeedAsync(scope.ServiceProvider);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erro ao executar seed do banco de dados");
        
        if (app.Environment.IsDevelopment())
        {
            throw;
        }
    }
}

app.Run();

static async Task ExecutarMigrationsAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("🚀 Iniciando execução automática de migrations...");
        
        var context = scope.ServiceProvider.GetRequiredService<AgendaAppDbContext>();
        
        var migrationsPendentes = await context.Database.GetPendingMigrationsAsync();
        
        if (migrationsPendentes.Any())
        {
            logger.LogInformation("📦 {Count} migration(s) pendente(s) encontrada(s): {Migrations}", 
                migrationsPendentes.Count(), string.Join(", ", migrationsPendentes));
            
            await context.Database.MigrateAsync();
            
            logger.LogInformation("✅ Migrations executadas com sucesso!");
        }
        else
        {
            logger.LogInformation("✅ Banco de dados já está atualizado. Nenhuma migration pendente.");
        }
        
        var canConnect = await context.Database.CanConnectAsync();
        logger.LogInformation("🔗 Conexão com banco de dados: {Status}", 
            canConnect ? "SUCESSO" : "FALHA");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Erro ao executar migrations automáticas: {Message}", ex.Message);
        
        if (app.Environment.IsDevelopment())
        {
            logger.LogWarning("⚠️ Continuando inicialização em modo desenvolvimento apesar do erro...");
        }
        else
        {
            throw;
        }
    }
} 