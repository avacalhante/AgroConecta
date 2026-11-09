using AgroConecta.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgroConecta.Infrastructure.Persistence.Configurations;

public class EntregaConfiguration : IEntityTypeConfiguration<Entrega>
{
    public void Configure(EntityTypeBuilder<Entrega> builder)
    {
        builder.ToTable("Entrega");
        builder.HasKey(entrega => entrega.Id);

        builder.Property(entrega => entrega.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(entrega => entrega.DataPrevista).HasColumnType("date");
        builder.Property(entrega => entrega.DataRealizada).HasColumnType("datetime2");
        builder.Property(entrega => entrega.FotoUrl).HasMaxLength(500);
        builder.Property(entrega => entrega.Observacao).HasMaxLength(500);
        builder.Property(entrega => entrega.CriadoEm).HasColumnType("datetime2");
        builder.Property(entrega => entrega.AtualizadoEm).HasColumnType("datetime2");

        builder.HasIndex(entrega => entrega.PedidoId).IsUnique().HasDatabaseName("UQ_Entrega_PedidoId");
        builder.HasIndex(entrega => new { entrega.Status, entrega.DataPrevista })
            .HasDatabaseName("IX_Entrega_Status_DataPrevista");
        builder.HasIndex(entrega => entrega.RotaId).HasDatabaseName("IX_Entrega_RotaId");

        builder.HasOne(entrega => entrega.Pedido)
            .WithOne(pedido => pedido.Entrega)
            .HasForeignKey<Entrega>(entrega => entrega.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(entrega => entrega.Rota)
            .WithMany(rota => rota.Entregas)
            .HasForeignKey(entrega => entrega.RotaId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
