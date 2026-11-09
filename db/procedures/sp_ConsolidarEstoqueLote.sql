CREATE OR ALTER PROCEDURE dbo.sp_ConsolidarEstoqueLote
    @LoteId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT lote.Id                    AS LoteId,
           lote.CodigoRastreio        AS CodigoRastreio,
           lote.Quantidade            AS QuantidadeColhida,
           lote.Status                AS Status,
           ISNULL(SUM(CASE WHEN movimento.Tipo = 'Entrada' THEN movimento.Quantidade END), 0) AS Entradas,
           ISNULL(SUM(CASE WHEN movimento.Tipo = 'Saida' THEN movimento.Quantidade END), 0)   AS Saidas,
           ISNULL(SUM(CASE WHEN movimento.Tipo = 'Ajuste' THEN movimento.Quantidade END), 0)  AS Ajustes,
           ISNULL(SUM(CASE movimento.Tipo
                          WHEN 'Entrada' THEN movimento.Quantidade
                          WHEN 'Saida' THEN -movimento.Quantidade
                          WHEN 'Ajuste' THEN movimento.Quantidade
                          ELSE 0
                      END), 0) AS Saldo
      FROM dbo.LoteColheita AS lote
      LEFT JOIN dbo.MovimentoEstoque AS movimento ON movimento.LoteId = lote.Id
     WHERE lote.Id = @LoteId
     GROUP BY lote.Id, lote.CodigoRastreio, lote.Quantidade, lote.Status;
END;
