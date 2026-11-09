using AgroConecta.Domain.Entities;
using AgroConecta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgroConecta.Tests.Banco;

public class ModeloBancoTests
{
    private static AgroConectaDbContext CriarContexto()
    {
        var options = new DbContextOptionsBuilder<AgroConectaDbContext>()
            .UseSqlServer(
                "Server=localhost,1433;Database=AgroConectaTeste;User Id=sa;Password=Teste@123;TrustServerCertificate=True")
            .Options;

        return new AgroConectaDbContext(options);
    }

    [Fact]
    public void Modelo_mapeia_as_14_tabelas_de_negocio()
    {
        using var contexto = CriarContexto();

        var tabelas = contexto.Model.GetEntityTypes()
            .Select(entidade => entidade.GetTableName())
            .OrderBy(nome => nome)
            .ToArray();

        Assert.Equal(
            new[]
            {
                "Cooperativa",
                "Entrega",
                "Fatura",
                "ItemPedido",
                "LogAcesso",
                "LoteColheita",
                "Motorista",
                "MovimentoEstoque",
                "Pedido",
                "Produto",
                "Produtor",
                "Rota",
                "Usuario",
                "Veiculo",
            },
            tabelas);
    }

    [Fact]
    public void Toda_entidade_de_negocio_tem_chave_primaria()
    {
        using var contexto = CriarContexto();

        foreach (var entidade in contexto.Model.GetEntityTypes())
        {
            var chave = entidade.FindPrimaryKey();
            Assert.True(chave is not null, $"Entidade {entidade.DisplayName()} sem chave primária.");
            Assert.Single(chave!.Properties);
        }
    }

    [Fact]
    public void Codigo_de_rastreio_do_lote_e_unico()
    {
        using var contexto = CriarContexto();

        var indice = contexto.Model.FindEntityType(typeof(LoteColheita))!
            .GetIndexes()
            .Single(indice => indice.Properties.Any(propriedade => propriedade.Name == nameof(LoteColheita.CodigoRastreio)));

        Assert.True(indice.IsUnique);
        Assert.Equal("UQ_LoteColheita_CodigoRastreio", indice.GetDatabaseName());
    }

    [Fact]
    public void Pedido_tem_uma_entrega_e_uma_fatura()
    {
        using var contexto = CriarContexto();

        var pedido = contexto.Model.FindEntityType(typeof(Pedido))!;

        var entrega = pedido.GetNavigations()
            .Single(navegacao => navegacao.Name == nameof(Pedido.Entrega));
        var fatura = pedido.GetNavigations()
            .Single(navegacao => navegacao.Name == nameof(Pedido.Fatura));

        Assert.True(entrega.ForeignKey.IsUnique);
        Assert.True(fatura.ForeignKey.IsUnique);
        Assert.Equal(typeof(Entrega), entrega.TargetEntityType.ClrType);
        Assert.Equal(typeof(Fatura), fatura.TargetEntityType.ClrType);
    }

    [Fact]
    public void Usuario_produtor_e_motorista_sao_um_para_um()
    {
        using var contexto = CriarContexto();

        var usuario = contexto.Model.FindEntityType(typeof(Usuario))!;

        var produtor = usuario.GetNavigations()
            .Single(navegacao => navegacao.Name == nameof(Usuario.Produtor));
        var motorista = usuario.GetNavigations()
            .Single(navegacao => navegacao.Name == nameof(Usuario.Motorista));

        Assert.True(produtor.ForeignKey.IsUnique);
        Assert.True(motorista.ForeignKey.IsUnique);
    }

    [Fact]
    public void Enums_sao_persistidos_como_texto()
    {
        using var contexto = CriarContexto();

        var statusPedido = contexto.Model.FindEntityType(typeof(Pedido))!
            .FindProperty(nameof(Pedido.Status))!;

        Assert.Equal(typeof(string), statusPedido.GetProviderClrType());
    }
}
