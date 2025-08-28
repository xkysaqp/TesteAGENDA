using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AgendaApp.Dominio.Entities;

namespace AgendaApp.Infraestrutura.Data.Configurations;

public class ConfigurationEntityHorarioDisponivel : IEntityTypeConfiguration<HorarioDisponivel>
{
    public void Configure(EntityTypeBuilder<HorarioDisponivel> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.DiaSemana)
            .IsRequired();
            
        builder.Property(e => e.HoraInicio)
            .IsRequired();
            
        builder.Property(e => e.HoraFim)
            .IsRequired();

        builder.Property(e => e.LojaId)
            .IsRequired();

        builder.HasOne(e => e.Prestador)
            .WithMany(p => p.HorariosDisponiveis)
            .HasForeignKey(e => e.PrestadorId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(e => e.Loja)
            .WithMany(l => l.HorariosDisponiveis)
            .HasForeignKey(e => e.LojaId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.ToTable("horarios_disponiveis");
    }
} 