using AgroConecta.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgroConecta.Infrastructure.Persistence.Configurations;

public class ItemPedidoConfiguration : IEntityTypeConfiguration<ItemPedido>
{
    public void Configure(EntityTypeBuilder<ItemPedido> builder)
    {
        builder.ToTable("ItemPedido");
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Quantidade).HasPrecision(18, 3);
        builder.Property(item => item.PrecoUnitario).HasPrecision(18, 2);
        builder.Property(item => item.Subtotal).HasPrecision(18, 2);
        builder.Property(item => item.CriadoEm).HasColumnType("datetime2");
        builder.Property(item => item.AtualizadoEm).HasColumnType("datetime2");

        builder.HasIndex(item => item.PedidoId).HasDatabaseName("IX_ItemPedido_PedidoId");
        builder.HasIndex(item => item.ProdutoId).HasDatabaseName("IX_ItemPedido_ProdutoId");
        builder.HasIndex(item => item.LoteId).HasDatabaseName("IX_ItemPedido_LoteId");

        builder.HasOne(item => item.Pedido)
            .WithMany(pedido => pedido.Itens)
            .HasForeignKey(item => item.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(item => item.Produto)
            .WithMany(produto => produto.ItensPedido)
            .HasForeignKey(item => item.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(item => item.Lote)
            .WithMany(lote => lote.ItensPedido)
            .HasForeignKey(item => item.LoteId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.ToTable(tabela =>
        {
            tabela.HasCheckConstraint("CK_ItemPedido_Quantidade", "[Quantidade] > 0");
            tabela.HasCheckConstraint("CK_ItemPedido_PrecoUnitario", "[PrecoUnitario] >= 0");
        });
    }
}
