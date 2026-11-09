using AgroConecta.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgroConecta.Infrastructure.Persistence.Configurations;

public class FaturaConfiguration : IEntityTypeConfiguration<Fatura>
{
    public void Configure(EntityTypeBuilder<Fatura> builder)
    {
        builder.ToTable("Fatura");
        builder.HasKey(fatura => fatura.Id);

        builder.Property(fatura => fatura.Numero).HasMaxLength(30).IsRequired();
        builder.Property(fatura => fatura.Valor).HasPrecision(18, 2);
        builder.Property(fatura => fatura.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(fatura => fatura.EmitidaEm).HasColumnType("datetime2");
        builder.Property(fatura => fatura.PagoEm).HasColumnType("datetime2");
        builder.Property(fatura => fatura.CriadoEm).HasColumnType("datetime2");
        builder.Property(fatura => fatura.AtualizadoEm).HasColumnType("datetime2");

        builder.HasIndex(fatura => fatura.PedidoId).IsUnique().HasDatabaseName("UQ_Fatura_PedidoId");
        builder.HasIndex(fatura => fatura.Numero).IsUnique().HasDatabaseName("UQ_Fatura_Numero");

        builder.HasOne(fatura => fatura.Pedido)
            .WithOne(pedido => pedido.Fatura)
            .HasForeignKey<Fatura>(fatura => fatura.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable(tabela => tabela.HasCheckConstraint("CK_Fatura_Valor", "[Valor] >= 0"));
    }
}
