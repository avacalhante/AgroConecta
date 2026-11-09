using AgroConecta.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgroConecta.Infrastructure.Persistence.Configurations;

public class MotoristaConfiguration : IEntityTypeConfiguration<Motorista>
{
    public void Configure(EntityTypeBuilder<Motorista> builder)
    {
        builder.ToTable("Motorista");
        builder.HasKey(motorista => motorista.Id);

        builder.Property(motorista => motorista.CNH).HasMaxLength(20).IsRequired();
        builder.Property(motorista => motorista.CriadoEm).HasColumnType("datetime2");
        builder.Property(motorista => motorista.AtualizadoEm).HasColumnType("datetime2");

        builder.HasIndex(motorista => motorista.UsuarioId).IsUnique().HasDatabaseName("UQ_Motorista_UsuarioId");
        builder.HasIndex(motorista => motorista.CNH).IsUnique().HasDatabaseName("UQ_Motorista_CNH");
    }
}
