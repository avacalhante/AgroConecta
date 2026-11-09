CREATE OR ALTER PROCEDURE dbo.sp_CalcularTotalPedido
    @PedidoId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE item
       SET item.Subtotal = item.Quantidade * item.PrecoUnitario,
           item.AtualizadoEm = SYSUTCDATETIME()
      FROM dbo.ItemPedido AS item
     WHERE item.PedidoId = @PedidoId;

    UPDATE pedido
       SET pedido.ValorTotal = ISNULL(
               (SELECT SUM(item.Subtotal)
                  FROM dbo.ItemPedido AS item
                 WHERE item.PedidoId = @PedidoId), 0),
           pedido.AtualizadoEm = SYSUTCDATETIME()
      FROM dbo.Pedido AS pedido
     WHERE pedido.Id = @PedidoId;
END;
