using AgroConecta.Domain.Entities;
using AgroConecta.Domain.Enums;
using AgroConecta.Domain.Exceptions;

namespace AgroConecta.Tests.Dominio;

public class EntregaTests
{
    private static Entrega NovaEntrega() => new()
    {
        Pedido = new Pedido(),
        DataPrevista = new DateOnly(2026, 11, 5),
    };

    [Fact]
    public void Entrega_vai_de_pendente_para_em_rota_e_entregue()
    {
        var entrega = NovaEntrega();

        entrega.AlterarStatus(StatusEntrega.EmRota);
        entrega.AlterarStatus(StatusEntrega.Entregue);

        Assert.Equal(StatusEntrega.Entregue, entrega.Status);
        Assert.NotNull(entrega.DataRealizada);
    }

    [Fact]
    public void Entrega_em_rota_pode_falhar()
    {
        var entrega = NovaEntrega();
        entrega.AlterarStatus(StatusEntrega.EmRota);

        entrega.AlterarStatus(StatusEntrega.Falha);

        Assert.Equal(StatusEntrega.Falha, entrega.Status);
    }

    [Fact]
    public void Entrega_rejeita_transicao_invalida()
    {
        var entrega = NovaEntrega();

        Assert.Throws<RegraNegocioException>(
            () => entrega.AlterarStatus(StatusEntrega.Entregue));
    }

    [Fact]
    public void Repetir_o_status_da_entrega_e_idempotente()
    {
        var entrega = NovaEntrega();
        entrega.AlterarStatus(StatusEntrega.EmRota);

        entrega.AlterarStatus(StatusEntrega.EmRota);

        Assert.Equal(StatusEntrega.EmRota, entrega.Status);
    }

    [Fact]
    public void Registrar_evidencia_guarda_foto_e_coordenadas()
    {
        var entrega = NovaEntrega();

        entrega.RegistrarEvidencia(-22.9068, -43.1729, "https://cdn.local/evidencia.jpg", "Entregue na portaria");

        Assert.Equal(-22.9068, entrega.Latitude);
        Assert.Equal(-43.1729, entrega.Longitude);
        Assert.Equal("https://cdn.local/evidencia.jpg", entrega.FotoUrl);
        Assert.Equal("Entregue na portaria", entrega.Observacao);
    }

    [Fact]
    public void Evidencia_rejeita_latitude_invalida()
    {
        var entrega = NovaEntrega();

        Assert.Throws<RegraNegocioException>(
            () => entrega.RegistrarEvidencia(91, 0, "foto.jpg"));
    }

    [Fact]
    public void Evidencia_rejeita_foto_vazia()
    {
        var entrega = NovaEntrega();

        Assert.Throws<RegraNegocioException>(
            () => entrega.RegistrarEvidencia(0, 0, " "));
    }

    [Fact]
    public void Entrega_cancelada_nao_aceita_evidencia()
    {
        var entrega = NovaEntrega();
        entrega.AlterarStatus(StatusEntrega.Cancelada);

        Assert.Throws<RegraNegocioException>(
            () => entrega.RegistrarEvidencia(0, 0, "foto.jpg"));
    }
}
