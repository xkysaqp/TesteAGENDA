using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AgendaApp.Infraestrutura.Identity;

namespace AgendaApp.Infraestrutura.Data.Configurations;

public class ConfigurationEntityApplicationUser : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("application_users");
        
        builder.Property(e => e.Nome)
            .HasMaxLength(200);
            
        builder.Property(e => e.CNPJ)
            .HasMaxLength(14);
            
        builder.Property(e => e.AreaAtuacao)
            .HasMaxLength(100);

        builder.Property(e => e.LojaId)
            .IsRequired();
        
        builder.HasOne<AgendaApp.Dominio.Entities.Loja>()
            .WithMany()
            .HasForeignKey(e => e.LojaId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(e => e.LojaId);
        builder.HasIndex(e => e.CNPJ);
    }
} 