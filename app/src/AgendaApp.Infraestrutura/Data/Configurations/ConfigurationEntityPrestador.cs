using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AgendaApp.Dominio.Entities;
using AgendaApp.Dominio.ValueObjects;

namespace AgendaApp.Infraestrutura.Data.Configurations;

public class ConfigurationEntityPrestador : IEntityTypeConfiguration<Prestador>
{
    public void Configure(EntityTypeBuilder<Prestador> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.HasIndex(e => e.Email);
        
        builder.Property(e => e.Nome)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(e => e.AreaAtuacao)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(e => e.Telefone)
            .IsRequired()
            .HasMaxLength(20);
        
        builder.Property(e => e.Email)
            .HasConversion(
                email => email.Endereco,
                value => Email.Criar(value))
            .HasColumnName("Email")
            .IsRequired();
            
        builder.Property(e => e.CNPJ)
            .HasConversion(
                cnpj => cnpj != null ? cnpj.Numero : null,
                value => value != null ? Cnpj.Criar(value) : null)
            .HasColumnName("CNPJ");

        builder.Property(e => e.LojaId)
            .IsRequired();

        builder.HasOne(e => e.Loja)
            .WithMany()
            .HasForeignKey(e => e.LojaId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.ToTable("prestadores");
    }
} 