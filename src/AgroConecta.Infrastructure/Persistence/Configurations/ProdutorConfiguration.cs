using AgroConecta.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgroConecta.Infrastructure.Persistence.Configurations;

public class ProdutorConfiguration : IEntityTypeConfiguration<Produtor>
{
    public void Configure(EntityTypeBuilder<Produtor> builder)
    {
        builder.ToTable("Produtor");
        builder.HasKey(produtor => produtor.Id);

        builder.Property(produtor => produtor.NomePropriedade).HasMaxLength(150).IsRequired();
        builder.Property(produtor => produtor.Documento).HasMaxLength(30).IsRequired();
        builder.Property(produtor => produtor.Cidade).HasMaxLength(100).IsRequired();
        builder.Property(produtor => produtor.UF).HasMaxLength(2).IsRequired();
        builder.Property(produtor => produtor.Comunidade).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(produtor => produtor.StatusCadastro).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(produtor => produtor.AprovadoEm).HasColumnType("datetime2");
        builder.Property(produtor => produtor.CriadoEm).HasColumnType("datetime2");
        builder.Property(produtor => produtor.AtualizadoEm).HasColumnType("datetime2");

        builder.HasIndex(produtor => produtor.UsuarioId).IsUnique().HasDatabaseName("UQ_Produtor_UsuarioId");
        builder.HasIndex(produtor => produtor.Documento).IsUnique().HasDatabaseName("UQ_Produtor_Documento");
        builder.HasIndex(produtor => produtor.CooperativaId).HasDatabaseName("IX_Produtor_CooperativaId");

        builder.HasOne(produtor => produtor.Cooperativa)
            .WithMany(cooperativa => cooperativa.Produtores)
            .HasForeignKey(produtor => produtor.CooperativaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(produtor => produtor.AprovadoPorUsuario)
            .WithMany()
            .HasForeignKey(produtor => produtor.AprovadoPorUsuarioId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
