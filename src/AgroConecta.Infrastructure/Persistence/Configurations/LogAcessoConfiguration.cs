using AgroConecta.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgroConecta.Infrastructure.Persistence.Configurations;

public class LogAcessoConfiguration : IEntityTypeConfiguration<LogAcesso>
{
    public void Configure(EntityTypeBuilder<LogAcesso> builder)
    {
        builder.ToTable("LogAcesso");
        builder.HasKey(log => log.Id);

        builder.Property(log => log.Id).ValueGeneratedOnAdd();
        builder.Property(log => log.Acao).HasMaxLength(60).IsRequired();
        builder.Property(log => log.Entidade).HasMaxLength(60).IsRequired();
        builder.Property(log => log.EntidadeId).HasMaxLength(60);
        builder.Property(log => log.IP).HasMaxLength(45);
        builder.Property(log => log.UserAgent).HasMaxLength(300);
        builder.Property(log => log.CriadoEm).HasColumnType("datetime2");

        builder.HasIndex(log => new { log.UsuarioId, log.CriadoEm })
            .HasDatabaseName("IX_LogAcesso_UsuarioId_CriadoEm");
        builder.HasIndex(log => new { log.Entidade, log.EntidadeId })
            .HasDatabaseName("IX_LogAcesso_Entidade_EntidadeId");
    }
}
