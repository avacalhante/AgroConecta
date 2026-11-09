using AgroConecta.Domain.Entities;
using AgroConecta.Domain.Enums;
using AgroConecta.Domain.Exceptions;

namespace AgroConecta.Tests.Dominio;

public class LoteColheitaTests
{
    private static LoteColheita NovoLote() => new()
    {
        Produto = new Produto { Nome = "Tomate", Categoria = "Hortifrúti", Unidade = "kg" },
        DataColheita = new DateOnly(2026, 11, 1),
        Quantidade = 100m,
    };

    [Fact]
    public void Lote_novo_comeca_disponivel()
    {
        Assert.Equal(StatusLote.Disponivel, NovoLote().Status);
    }

    [Fact]
    public void Reservar_e_liberar_alternam_o_status()
    {
        var lote = NovoLote();

        lote.Reservar();
        Assert.Equal(StatusLote.Reservado, lote.Status);

        lote.Liberar();
        Assert.Equal(StatusLote.Disponivel, lote.Status);
    }

    [Fact]
    public void Nao_reserva_lote_ja_reservado()
    {
        var lote = NovoLote();
        lote.Reservar();

        Assert.Throws<RegraNegocioException>(lote.Reservar);
    }

    [Fact]
    public void Esgotar_e_idempotente()
    {
        var lote = NovoLote();

        lote.Esgotar();
        lote.Esgotar();

        Assert.Equal(StatusLote.Esgotado, lote.Status);
    }
}
