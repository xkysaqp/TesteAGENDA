using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AgendaApp.Dominio.Entities;
using AgendaApp.Dominio.ValueObjects;

namespace AgendaApp.Infraestrutura.Data.Configurations;

public class ConfigurationEntityAgendamento : IEntityTypeConfiguration<Agendamento>
{
    public void Configure(EntityTypeBuilder<Agendamento> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.HasIndex(e => new { e.LojaId, e.Data, e.Hora });
        builder.HasIndex(e => new { e.ServicoId, e.Data });
        builder.HasIndex(e => e.Status);
        
        builder.Property(e => e.ClienteNome)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(e => e.ClienteTelefone)
            .IsRequired()
            .HasMaxLength(20);
            
        builder.Property(e => e.Observacoes)
            .HasMaxLength(1000);
            
        builder.Property(e => e.MotivoCancelamento)
            .HasMaxLength(500);
            
        builder.Property(e => e.Valor)
            .HasPrecision(10, 2);
            
        builder.Property(e => e.Data)
            .IsRequired();
            
        builder.Property(e => e.Hora)
            .IsRequired();
            
        builder.Property(e => e.Status)
            .IsRequired();

        builder.Property(e => e.ClienteEmail)
            .HasConversion(
                email => email != null ? email.Endereco : null,
                value => value != null ? Email.Criar(value) : null)
            .HasColumnName("ClienteEmail");

        builder.HasOne(e => e.Loja)
            .WithMany()
            .HasForeignKey(e => e.LojaId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(e => e.Servico)
            .WithMany()
            .HasForeignKey(e => e.ServicoId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(e => e.Prestador)
            .WithMany()
            .HasForeignKey(e => e.PrestadorId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.ToTable("agendamentos");
    }
} 