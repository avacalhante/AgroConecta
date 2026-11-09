using AgroConecta.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgroConecta.Infrastructure.Persistence.Configurations;

public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("Pedido");
        builder.HasKey(pedido => pedido.Id);

        builder.Property(pedido => pedido.DataPedido).HasColumnType("datetime2").IsRequired();
        builder.Property(pedido => pedido.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(pedido => pedido.ValorTotal).HasPrecision(18, 2);
        builder.Property(pedido => pedido.EnderecoEntrega).HasMaxLength(300).IsRequired();
        builder.Property(pedido => pedido.CriadoEm).HasColumnType("datetime2");
        builder.Property(pedido => pedido.AtualizadoEm).HasColumnType("datetime2");

        builder.HasIndex(pedido => new { pedido.Status, pedido.DataPedido })
            .HasDatabaseName("IX_Pedido_Status_DataPedido");
        builder.HasIndex(pedido => pedido.CompradorId).HasDatabaseName("IX_Pedido_CompradorId");

        builder.Navigation(pedido => pedido.Itens)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasField("_itens");

        builder.ToTable(tabela => tabela.HasCheckConstraint("CK_Pedido_ValorTotal", "[ValorTotal] >= 0"));
    }
}
