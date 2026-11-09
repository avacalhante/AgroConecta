using AgroConecta.Domain.Enums;

namespace AgroConecta.Web.Models;

/// <summary>Classes Bootstrap para exibição consistente de status nas telas.</summary>
public static class Badges
{
    public static string Pedido(StatusPedido status) => status switch
    {
        StatusPedido.Rascunho => "bg-secondary",
        StatusPedido.AguardandoAprovacao => "bg-warning text-dark",
        StatusPedido.Aprovado => "bg-info text-dark",
        StatusPedido.EmSeparacao => "bg-primary",
        StatusPedido.EmRota => "bg-primary",
        StatusPedido.Entregue => "bg-success",
        StatusPedido.Cancelado => "bg-danger",
        _ => "bg-secondary",
    };

    public static string Entrega(StatusEntrega status) => status switch
    {
        StatusEntrega.Pendente => "bg-secondary",
        StatusEntrega.EmRota => "bg-primary",
        StatusEntrega.Entregue => "bg-success",
        StatusEntrega.Falha => "bg-danger",
        StatusEntrega.Cancelada => "bg-dark",
        _ => "bg-secondary",
    };

    public static string Lote(StatusLote status) => status switch
    {
        StatusLote.Disponivel => "bg-success",
        StatusLote.Reservado => "bg-warning text-dark",
        StatusLote.Esgotado => "bg-secondary",
        _ => "bg-secondary",
    };

    public static string Cadastro(StatusCadastro status) => status switch
    {
        StatusCadastro.Pendente => "bg-warning text-dark",
        StatusCadastro.Aprovado => "bg-success",
        StatusCadastro.Rejeitado => "bg-danger",
        _ => "bg-secondary",
    };

    public static string Fatura(StatusFatura status) => status switch
    {
        StatusFatura.Pendente => "bg-warning text-dark",
        StatusFatura.Emitida => "bg-info text-dark",
        StatusFatura.Paga => "bg-success",
        StatusFatura.Cancelada => "bg-danger",
        _ => "bg-secondary",
    };

    public static string Papel(PapelUsuario papel) => papel switch
    {
        PapelUsuario.Admin => "bg-dark",
        PapelUsuario.Produtor => "bg-success",
        PapelUsuario.Comprador => "bg-primary",
        PapelUsuario.Motorista => "bg-info text-dark",
        _ => "bg-secondary",
    };
}
