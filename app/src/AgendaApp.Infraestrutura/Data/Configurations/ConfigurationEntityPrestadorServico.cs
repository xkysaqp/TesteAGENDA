using AgendaApp.Dominio.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgendaApp.Infraestrutura.Data.Configurations;

public class ConfigurationEntityPrestadorServico : IEntityTypeConfiguration<PrestadorServico>
{
    public void Configure(EntityTypeBuilder<PrestadorServico> builder)
    {
        builder.ToTable("prestador_servicos");
        
        builder.HasKey(ps => ps.Id);
        
        builder.Property(ps => ps.Id)
            .IsRequired()
            .ValueGeneratedNever();
            
        builder.Property(ps => ps.PrestadorId)
            .IsRequired();
            
        builder.Property(ps => ps.ServicoId)
            .IsRequired();
            
        builder.Property(ps => ps.ValorPersonalizado)
            .HasColumnType("decimal(10,2)");
            
        builder.Property(ps => ps.DuracaoPersonalizadaEmMinutos);
        
        builder.Property(ps => ps.DataCriacao)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(ps => ps.DataAtualizacao);
        
        builder.Property(ps => ps.Ativo)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.HasOne(ps => ps.Prestador)
            .WithMany(p => p.PrestadorServicos)
            .HasForeignKey(ps => ps.PrestadorId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(ps => ps.Servico)
            .WithMany(s => s.PrestadorServicos)
            .HasForeignKey(ps => ps.ServicoId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasIndex(ps => new { ps.PrestadorId, ps.ServicoId })
            .IsUnique()
            .HasDatabaseName("IX_PrestadorServico_PrestadorId_ServicoId");
            
        builder.HasIndex(ps => ps.PrestadorId)
            .HasDatabaseName("IX_PrestadorServico_PrestadorId");
            
        builder.HasIndex(ps => ps.ServicoId)
            .HasDatabaseName("IX_PrestadorServico_ServicoId");
            
        builder.HasQueryFilter(ps => ps.Ativo);
    }
} 