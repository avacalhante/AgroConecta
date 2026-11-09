using AgroConecta.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgroConecta.Infrastructure.Persistence.Configurations;

public class MovimentoEstoqueConfiguration : IEntityTypeConfiguration<MovimentoEstoque>
{
    public void Configure(EntityTypeBuilder<MovimentoEstoque> builder)
    {
        builder.ToTable("MovimentoEstoque");
        builder.HasKey(movimento => movimento.Id);

        builder.Property(movimento => movimento.Tipo).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(movimento => movimento.Quantidade).HasPrecision(18, 3);
        builder.Property(movimento => movimento.CriadoEm).HasColumnType("datetime2");
        builder.Property(movimento => movimento.AtualizadoEm).HasColumnType("datetime2");

        builder.HasIndex(movimento => new { movimento.LoteId, movimento.CriadoEm })
            .HasDatabaseName("IX_MovimentoEstoque_LoteId_CriadoEm");
        builder.HasIndex(movimento => movimento.PedidoId).HasDatabaseName("IX_MovimentoEstoque_PedidoId");

        builder.HasOne(movimento => movimento.Lote)
            .WithMany(lote => lote.Movimentos)
            .HasForeignKey(movimento => movimento.LoteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(movimento => movimento.Pedido)
            .WithMany(pedido => pedido.Movimentos)
            .HasForeignKey(movimento => movimento.PedidoId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
