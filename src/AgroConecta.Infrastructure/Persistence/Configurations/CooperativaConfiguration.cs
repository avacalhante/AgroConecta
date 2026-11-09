using AgroConecta.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgroConecta.Infrastructure.Persistence.Configurations;

public class CooperativaConfiguration : IEntityTypeConfiguration<Cooperativa>
{
    public void Configure(EntityTypeBuilder<Cooperativa> builder)
    {
        builder.ToTable("Cooperativa");
        builder.HasKey(cooperativa => cooperativa.Id);

        builder.Property(cooperativa => cooperativa.Nome).HasMaxLength(150).IsRequired();
        builder.Property(cooperativa => cooperativa.Cidade).HasMaxLength(100).IsRequired();
        builder.Property(cooperativa => cooperativa.UF).HasMaxLength(2).IsRequired();
        builder.Property(cooperativa => cooperativa.Tipo).HasMaxLength(60).IsRequired();
        builder.Property(cooperativa => cooperativa.CriadoEm).HasColumnType("datetime2");
        builder.Property(cooperativa => cooperativa.AtualizadoEm).HasColumnType("datetime2");

        builder.HasIndex(cooperativa => cooperativa.Nome).HasDatabaseName("IX_Cooperativa_Nome");
    }
}
