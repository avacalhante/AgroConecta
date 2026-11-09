using AgroConecta.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgroConecta.Infrastructure.Persistence.Configurations;

public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produto");
        builder.HasKey(produto => produto.Id);

        builder.Property(produto => produto.Nome).HasMaxLength(120).IsRequired();
        builder.Property(produto => produto.Categoria).HasMaxLength(80).IsRequired();
        builder.Property(produto => produto.Unidade).HasMaxLength(20).IsRequired();
        builder.Property(produto => produto.PrecoBase).HasPrecision(18, 2);
        builder.Property(produto => produto.CriadoEm).HasColumnType("datetime2");
        builder.Property(produto => produto.AtualizadoEm).HasColumnType("datetime2");

        builder.HasIndex(produto => produto.Nome).HasDatabaseName("IX_Produto_Nome");
        builder.HasIndex(produto => produto.Ativo).HasDatabaseName("IX_Produto_Ativo");

        builder.ToTable(tabela => tabela.HasCheckConstraint("CK_Produto_PrecoBase", "[PrecoBase] >= 0"));
    }
}
