CREATE OR ALTER PROCEDURE dbo.sp_RelatorioVendasProdutor
    @ProdutorId INT,
    @DataInicio DATE,
    @DataFim DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT pedido.Id                AS PedidoId,
           pedido.DataPedido        AS DataPedido,
           pedido.Status            AS StatusPedido,
           produto.Nome             AS Produto,
           lote.CodigoRastreio      AS CodigoRastreio,
           item.Quantidade          AS Quantidade,
           item.PrecoUnitario       AS PrecoUnitario,
           item.Subtotal            AS Subtotal
      FROM dbo.ItemPedido AS item
      INNER JOIN dbo.Pedido AS pedido ON pedido.Id = item.PedidoId
      INNER JOIN dbo.LoteColheita AS lote ON lote.Id = item.LoteId
      INNER JOIN dbo.Produto AS produto ON produto.Id = item.ProdutoId
     WHERE lote.ProdutorId = @ProdutorId
       AND CAST(pedido.DataPedido AS DATE) BETWEEN @DataInicio AND @DataFim
       AND pedido.Status <> 'Cancelado'
     ORDER BY pedido.DataPedido, produto.Nome;

    SELECT COUNT(DISTINCT pedido.Id) AS TotalPedidos,
           ISNULL(SUM(item.Subtotal), 0) AS ValorTotal
      FROM dbo.ItemPedido AS item
      INNER JOIN dbo.Pedido AS pedido ON pedido.Id = item.PedidoId
      INNER JOIN dbo.LoteColheita AS lote ON lote.Id = item.LoteId
     WHERE lote.ProdutorId = @ProdutorId
       AND CAST(pedido.DataPedido AS DATE) BETWEEN @DataInicio AND @DataFim
       AND pedido.Status <> 'Cancelado';
END;
