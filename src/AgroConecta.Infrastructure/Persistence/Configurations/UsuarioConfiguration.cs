using AgroConecta.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgroConecta.Infrastructure.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuario");
        builder.HasKey(usuario => usuario.Id);

        builder.Property(usuario => usuario.Nome).HasMaxLength(150).IsRequired();
        builder.Property(usuario => usuario.Email).HasMaxLength(200).IsRequired();
        builder.Property(usuario => usuario.SenhaHash).HasMaxLength(255).IsRequired();
        builder.Property(usuario => usuario.Papel).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(usuario => usuario.CriadoEm).HasColumnType("datetime2");
        builder.Property(usuario => usuario.AtualizadoEm).HasColumnType("datetime2");

        builder.HasIndex(usuario => usuario.Email).IsUnique().HasDatabaseName("UQ_Usuario_Email");

        builder.HasOne(usuario => usuario.Produtor)
            .WithOne(produtor => produtor.Usuario)
            .HasForeignKey<Produtor>(produtor => produtor.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(usuario => usuario.Motorista)
            .WithOne(motorista => motorista.Usuario)
            .HasForeignKey<Motorista>(motorista => motorista.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(usuario => usuario.PedidosComoComprador)
            .WithOne(pedido => pedido.Comprador)
            .HasForeignKey(pedido => pedido.CompradorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(usuario => usuario.LogsAcesso)
            .WithOne(log => log.Usuario)
            .HasForeignKey(log => log.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
