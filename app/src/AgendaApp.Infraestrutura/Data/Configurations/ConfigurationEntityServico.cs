using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AgendaApp.Dominio.Entities;

namespace AgendaApp.Infraestrutura.Data.Configurations;

public class ConfigurationEntityServico : IEntityTypeConfiguration<Servico>
{
    public void Configure(EntityTypeBuilder<Servico> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Nome)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(e => e.Descricao)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(e => e.Valor)
            .HasPrecision(10, 2)
            .IsRequired();
            
        builder.Property(e => e.DuracaoEmMinutos)
            .IsRequired();

        builder.Property(e => e.LojaId)
            .IsRequired();

        builder.HasOne(e => e.Loja)
            .WithMany(l => l.Servicos)
            .HasForeignKey(e => e.LojaId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.ToTable("servicos");
    }
} 