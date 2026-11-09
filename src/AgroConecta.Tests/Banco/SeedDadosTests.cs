using AgroConecta.Domain.Entities;
using AgroConecta.Domain.Enums;
using AgroConecta.Infrastructure.Persistence;
using AgroConecta.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AgroConecta.Tests.Banco;

public class SeedDadosTests
{
    private static AgroConectaDbContext CriarContexto()
    {
        var options = new DbContextOptionsBuilder<AgroConectaDbContext>()
            .UseInMemoryDatabase($"agroconecta-seed-{Guid.NewGuid()}")
            .Options;

        return new AgroConectaDbContext(options);
    }

    [Fact]
    public async Task Seed_cria_usuarios_produtores_pedidos_e_entrega()
    {
        await using var contexto = CriarContexto();

        var aplicado = await SeedDados.AplicarAsync(contexto);

        Assert.True(aplicado);
        Assert.Equal(5, await contexto.Usuarios.CountAsync());
        Assert.Equal(2, await contexto.Produtores.CountAsync());
        Assert.Equal(2, await contexto.LotesColheita.CountAsync());
        Assert.Equal(2, await contexto.Pedidos.CountAsync());
        Assert.Equal(2, await contexto.Entregas.CountAsync());
        Assert.Equal(1, await contexto.Faturas.CountAsync());
        Assert.Equal(1, await contexto.Motoristas.CountAsync());
    }

    [Fact]
    public async Task Seed_e_idempotente()
    {
        await using var contexto = CriarContexto();

        Assert.True(await SeedDados.AplicarAsync(contexto));
        Assert.False(await SeedDados.AplicarAsync(contexto));
        Assert.Equal(5, await contexto.Usuarios.CountAsync());
    }

    [Fact]
    public async Task Senha_do_seed_confere_com_o_hash()
    {
        await using var contexto = CriarContexto();
        await SeedDados.AplicarAsync(contexto);

        var produtor = await contexto.Usuarios.SingleAsync(usuario => usuario.Email == "produtor@agroconecta.local");

        var resultado = new PasswordHasher<Usuario>().VerifyHashedPassword(
            produtor,
            produtor.SenhaHash,
            SeedDados.SenhaProdutor);

        Assert.NotEqual(PasswordVerificationResult.Failed, resultado);
    }

    [Fact]
    public async Task Pedido_em_rota_tem_entrega_em_rota()
    {
        await using var contexto = CriarContexto();
        await SeedDados.AplicarAsync(contexto);

        var pedido = await contexto.Pedidos
            .Include(pedido => pedido.Entrega)
            .SingleAsync(pedido => pedido.Status == StatusPedido.EmRota);

        Assert.NotNull(pedido.Entrega);
        Assert.Equal(StatusEntrega.EmRota, pedido.Entrega!.Status);
    }

    [Fact]
    public async Task Total_do_pedido_confere_com_os_itens()
    {
        await using var contexto = CriarContexto();
        await SeedDados.AplicarAsync(contexto);

        var pedidos = await contexto.Pedidos
            .Include(pedido => pedido.Itens)
            .ToListAsync();

        foreach (var pedido in pedidos)
        {
            Assert.Equal(pedido.ValorTotal, pedido.Itens.Sum(item => item.Subtotal));
        }
    }
}
