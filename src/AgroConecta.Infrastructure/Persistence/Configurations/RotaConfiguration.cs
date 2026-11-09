using AgroConecta.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgroConecta.Infrastructure.Persistence.Configurations;

public class RotaConfiguration : IEntityTypeConfiguration<Rota>
{
    public void Configure(EntityTypeBuilder<Rota> builder)
    {
        builder.ToTable("Rota");
        builder.HasKey(rota => rota.Id);

        builder.Property(rota => rota.Data).HasColumnType("date");
        builder.Property(rota => rota.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(rota => rota.CriadoEm).HasColumnType("datetime2");
        builder.Property(rota => rota.AtualizadoEm).HasColumnType("datetime2");

        builder.HasIndex(rota => new { rota.MotoristaId, rota.Data })
            .HasDatabaseName("IX_Rota_MotoristaId_Data");
        builder.HasIndex(rota => rota.VeiculoId).HasDatabaseName("IX_Rota_VeiculoId");

        builder.HasOne(rota => rota.Motorista)
            .WithMany(motorista => motorista.Rotas)
            .HasForeignKey(rota => rota.MotoristaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(rota => rota.Veiculo)
            .WithMany(veiculo => veiculo.Rotas)
            .HasForeignKey(rota => rota.VeiculoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
