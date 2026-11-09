using AgroConecta.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgroConecta.Infrastructure.Persistence.Configurations;

public class LoteColheitaConfiguration : IEntityTypeConfiguration<LoteColheita>
{
    public void Configure(EntityTypeBuilder<LoteColheita> builder)
    {
        builder.ToTable("LoteColheita");
        builder.HasKey(lote => lote.Id);

        builder.Property(lote => lote.CodigoRastreio).IsRequired();
        builder.Property(lote => lote.DataColheita).HasColumnType("date");
        builder.Property(lote => lote.Quantidade).HasPrecision(18, 3);
        builder.Property(lote => lote.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(lote => lote.CriadoEm).HasColumnType("datetime2");
        builder.Property(lote => lote.AtualizadoEm).HasColumnType("datetime2");

        builder.HasIndex(lote => lote.CodigoRastreio)
            .IsUnique()
            .HasDatabaseName("UQ_LoteColheita_CodigoRastreio");
        builder.HasIndex(lote => new { lote.ProdutorId, lote.DataColheita })
            .HasDatabaseName("IX_LoteColheita_ProdutorId_DataColheita");
        builder.HasIndex(lote => lote.ProdutoId).HasDatabaseName("IX_LoteColheita_ProdutoId");

        builder.HasOne(lote => lote.Produtor)
            .WithMany(produtor => produtor.Lotes)
            .HasForeignKey(lote => lote.ProdutorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(lote => lote.Produto)
            .WithMany(produto => produto.Lotes)
            .HasForeignKey(lote => lote.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(tabela => tabela.HasCheckConstraint("CK_LoteColheita_Quantidade", "[Quantidade] > 0"));
    }
}
