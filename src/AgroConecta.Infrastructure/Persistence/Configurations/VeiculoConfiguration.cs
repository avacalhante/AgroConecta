using AgroConecta.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgroConecta.Infrastructure.Persistence.Configurations;

public class VeiculoConfiguration : IEntityTypeConfiguration<Veiculo>
{
    public void Configure(EntityTypeBuilder<Veiculo> builder)
    {
        builder.ToTable("Veiculo");
        builder.HasKey(veiculo => veiculo.Id);

        builder.Property(veiculo => veiculo.Placa).HasMaxLength(10).IsRequired();
        builder.Property(veiculo => veiculo.Tipo).HasMaxLength(40).IsRequired();
        builder.Property(veiculo => veiculo.CapacidadeKg).HasPrecision(18, 2);
        builder.Property(veiculo => veiculo.CriadoEm).HasColumnType("datetime2");
        builder.Property(veiculo => veiculo.AtualizadoEm).HasColumnType("datetime2");

        builder.HasIndex(veiculo => veiculo.Placa).IsUnique().HasDatabaseName("UQ_Veiculo_Placa");
    }
}
