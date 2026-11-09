using AgroConecta.Domain.Entities;
using AgroConecta.Domain.Enums;
using AgroConecta.Domain.Exceptions;

namespace AgroConecta.Tests.Dominio;

public class PedidoTests
{
    private static Pedido NovoPedido() => new()
    {
        Comprador = new Usuario { Nome = "Compradora", Email = "compradora@teste.local" },
        EnderecoEntrega = "Rua de Teste, 10",
    };

    private static Produto NovoProduto(decimal precoBase = 10m) => new()
    {
        Id = 1,
        Nome = "Tomate",
        Categoria = "Hortifrúti",
        Unidade = "kg",
        PrecoBase = precoBase,
    };

    [Fact]
    public void AdicionarItem_calcula_subtotal_e_total()
    {
        var pedido = NovoPedido();

        pedido.AdicionarItem(NovoProduto(), null, 3m, 8.50m);
        pedido.AdicionarItem(NovoProduto(), null, 2m, 5.00m);

        Assert.Equal(2, pedido.Itens.Count);
        Assert.Equal(35.50m, pedido.ValorTotal);
        Assert.All(pedido.Itens, item => Assert.Equal(item.Quantidade * item.PrecoUnitario, item.Subtotal));
    }

    [Fact]
    public void AdicionarItem_com_quantidade_zero_viola_regra()
    {
        var pedido = NovoPedido();

        var excecao = Assert.Throws<RegraNegocioException>(
            () => pedido.AdicionarItem(NovoProduto(), null, 0m, 8.50m));

        Assert.Contains("maior que zero", excecao.Message);
    }

    [Fact]
    public void AdicionarItem_com_preco_negativo_viola_regra()
    {
        var pedido = NovoPedido();

        Assert.Throws<RegraNegocioException>(
            () => pedido.AdicionarItem(NovoProduto(), null, 1m, -1m));
    }

    [Fact]
    public void RemoverItem_recalcula_o_total()
    {
        var pedido = NovoPedido();
        var primeiro = pedido.AdicionarItem(NovoProduto(), null, 2m, 10m);
        pedido.AdicionarItem(NovoProduto(), null, 1m, 5m);

        var removido = pedido.RemoverItem(primeiro);

        Assert.True(removido);
        Assert.Equal(5m, pedido.ValorTotal);
        Assert.Single(pedido.Itens);
    }

    [Fact]
    public void Pedido_percorre_o_fluxo_valido_ate_entregue()
    {
        var pedido = NovoPedido();

        pedido.AlterarStatus(StatusPedido.AguardandoAprovacao);
        pedido.AlterarStatus(StatusPedido.Aprovado);
        pedido.AlterarStatus(StatusPedido.EmSeparacao);
        pedido.AlterarStatus(StatusPedido.EmRota);
        pedido.AlterarStatus(StatusPedido.Entregue);

        Assert.Equal(StatusPedido.Entregue, pedido.Status);
    }

    [Fact]
    public void Pedido_rejeita_transicao_invalida()
    {
        var pedido = NovoPedido();

        var excecao = Assert.Throws<RegraNegocioException>(
            () => pedido.AlterarStatus(StatusPedido.Entregue));

        Assert.Contains("não permitida", excecao.Message);
        Assert.Equal(StatusPedido.Rascunho, pedido.Status);
    }

    [Fact]
    public void Pedido_cancelado_nao_volta_a_avancar()
    {
        var pedido = NovoPedido();
        pedido.AlterarStatus(StatusPedido.Cancelado);

        Assert.Throws<RegraNegocioException>(
            () => pedido.AlterarStatus(StatusPedido.Aprovado));
    }

    [Fact]
    public void Alterar_para_o_mesmo_status_e_idempotente()
    {
        var pedido = NovoPedido();

        pedido.AlterarStatus(StatusPedido.Rascunho);

        Assert.Equal(StatusPedido.Rascunho, pedido.Status);
    }

    [Fact]
    public void Pedido_em_rota_nao_aceita_novos_itens()
    {
        var pedido = NovoPedido();
        pedido.AlterarStatus(StatusPedido.AguardandoAprovacao);
        pedido.AlterarStatus(StatusPedido.Aprovado);
        pedido.AlterarStatus(StatusPedido.EmSeparacao);
        pedido.AlterarStatus(StatusPedido.EmRota);

        Assert.Throws<RegraNegocioException>(
            () => pedido.AdicionarItem(NovoProduto(), null, 1m, 10m));
    }
}
