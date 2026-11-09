CREATE OR ALTER TRIGGER dbo.trg_Entrega_LogStatus
ON dbo.Entrega
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF TRIGGER_NESTLEVEL() > 1
        RETURN;

    INSERT INTO dbo.LogAcesso (UsuarioId, Acao, Entidade, EntidadeId, IP, UserAgent, CriadoEm)
    SELECT NULL,
           CONCAT('StatusEntrega:', removido.Status, '->', inserido.Status),
           'Entrega',
           CAST(inserido.Id AS NVARCHAR(60)),
           NULL,
           'trigger',
           SYSUTCDATETIME()
      FROM inserted AS inserido
      INNER JOIN deleted AS removido ON removido.Id = inserido.Id
     WHERE inserido.Status <> removido.Status;
END;
