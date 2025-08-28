using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AgendaApp.Dominio.Entities;
using AgendaApp.Dominio.ValueObjects;

namespace AgendaApp.Infraestrutura.Data.Configurations;

public class ConfigurationEntityLoja : IEntityTypeConfiguration<Loja>
{
    public void Configure(EntityTypeBuilder<Loja> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.HasIndex(e => e.Slug).IsUnique();
        
        builder.Property(e => e.Nome)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(e => e.Slug)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(e => e.Plano)
            .IsRequired()
            .HasMaxLength(20);
            
        builder.Property(e => e.TelefoneContato)
            .IsRequired()
            .HasMaxLength(20);
            
        builder.Property(e => e.Endereco)
            .HasMaxLength(500);
            
        builder.Property(e => e.Descricao)
            .HasMaxLength(1000);
            
        builder.Property(e => e.LogoUrl)
            .HasMaxLength(500);
        
        builder.Property(e => e.CNPJ)
            .HasConversion(
                cnpj => cnpj.Numero,
                value => Cnpj.Criar(value))
            .HasColumnName("CNPJ")
            .IsRequired();
            
        builder.Property(e => e.EmailContato)
            .HasConversion(
                email => email.Endereco,
                value => Email.Criar(value))
            .HasColumnName("EmailContato")
            .IsRequired();
        
        builder.ToTable("lojas");
    }
} 