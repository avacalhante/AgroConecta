CREATE OR ALTER TRIGGER dbo.trg_ItemPedido_MovimentoEstoque
ON dbo.ItemPedido
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Evita recursão quando sp_CalcularTotalPedido atualiza Subtotal.
    IF TRIGGER_NESTLEVEL() > 1
        RETURN;

    -- Saídas: itens novos ou com lote/quantidade alterados.
    INSERT INTO dbo.MovimentoEstoque (LoteId, Tipo, Quantidade, PedidoId, CriadoEm)
    SELECT inserido.LoteId, 'Saida', inserido.Quantidade, inserido.PedidoId, SYSUTCDATETIME()
      FROM inserted AS inserido
     WHERE inserido.LoteId IS NOT NULL
       AND NOT EXISTS (
           SELECT 1
             FROM deleted AS removido
            WHERE removido.Id = inserido.Id
              AND removido.LoteId = inserido.LoteId
              AND removido.Quantidade = inserido.Quantidade);

    -- Estornos: itens removidos ou com lote/quantidade alterados.
    INSERT INTO dbo.MovimentoEstoque (LoteId, Tipo, Quantidade, PedidoId, CriadoEm)
    SELECT removido.LoteId, 'Entrada', removido.Quantidade, removido.PedidoId, SYSUTCDATETIME()
      FROM deleted AS removido
     WHERE removido.LoteId IS NOT NULL
       AND NOT EXISTS (
           SELECT 1
             FROM inserted AS inserido
            WHERE inserido.Id = removido.Id
              AND inserido.LoteId = removido.LoteId
              AND inserido.Quantidade = removido.Quantidade);

    DECLARE @Pedidos TABLE (PedidoId INT PRIMARY KEY);
    INSERT INTO @Pedidos (PedidoId)
    SELECT DISTINCT PedidoId FROM inserted WHERE PedidoId IS NOT NULL
    UNION
    SELECT DISTINCT PedidoId FROM deleted WHERE PedidoId IS NOT NULL;

    DECLARE @PedidoId INT;
    DECLARE pedidos CURSOR LOCAL FAST_FORWARD FOR SELECT PedidoId FROM @Pedidos;
    OPEN pedidos;
    FETCH NEXT FROM pedidos INTO @PedidoId;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        EXEC dbo.sp_CalcularTotalPedido @PedidoId = @PedidoId;
        FETCH NEXT FROM pedidos INTO @PedidoId;
    END;
    CLOSE pedidos;
    DEALLOCATE pedidos;
END;
