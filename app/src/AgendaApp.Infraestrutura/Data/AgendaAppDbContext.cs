using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AgendaApp.Dominio.Entities;
using AgendaApp.Dominio.ValueObjects;
using AgendaApp.Infraestrutura.Data.Configurations;
using AgendaApp.Infraestrutura.Identity;


namespace AgendaApp.Infraestrutura.Data;

public class AgendaAppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public Guid? LojaAtualId { get; set; }

    public DbSet<Loja> Lojas { get; set; }
    public DbSet<Prestador> Prestadores { get; set; }
    public DbSet<Servico> Servicos { get; set; }
    public DbSet<PrestadorServico> PrestadorServicos { get; set; }
    public DbSet<HorarioDisponivel> HorariosDisponiveis { get; set; }
    public DbSet<Agendamento> Agendamentos { get; set; }
    
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }

    public AgendaAppDbContext(DbContextOptions<AgendaAppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasDefaultSchema("public");

        ConfigurarEntidadesDominio(modelBuilder);
        
        ConfigurarIdentity(modelBuilder);
        
        ConfigurarFiltrosMultitenant(modelBuilder);
    }

    private void ConfigurarEntidadesDominio(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<Cnpj>();
        modelBuilder.Ignore<Email>();
        
        modelBuilder.ApplyConfiguration(new ConfigurationEntityLoja());
        modelBuilder.ApplyConfiguration(new ConfigurationEntityPrestador());
        modelBuilder.ApplyConfiguration(new ConfigurationEntityServico());
        modelBuilder.ApplyConfiguration(new ConfigurationEntityPrestadorServico());
        modelBuilder.ApplyConfiguration(new ConfigurationEntityHorarioDisponivel());
        modelBuilder.ApplyConfiguration(new ConfigurationEntityAgendamento());
        modelBuilder.ApplyConfiguration(new ConfigurationEntityApplicationUser());
    }

    private void ConfigurarIdentity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityRole<Guid>>().ToTable("roles");
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("usuario_roles");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("usuario_claims");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("usuario_logins");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("usuario_tokens");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("role_claims");
    }

    private void ConfigurarFiltrosMultitenant(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Prestador>()
            .HasQueryFilter(e => !LojaAtualId.HasValue || e.LojaId == LojaAtualId);
            
        modelBuilder.Entity<Servico>()
            .HasQueryFilter(e => !LojaAtualId.HasValue || e.LojaId == LojaAtualId);
            
        modelBuilder.Entity<HorarioDisponivel>()
            .HasQueryFilter(e => !LojaAtualId.HasValue || e.LojaId == LojaAtualId);
            
        modelBuilder.Entity<Agendamento>()
            .HasQueryFilter(e => !LojaAtualId.HasValue || e.LojaId == LojaAtualId);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }
} 