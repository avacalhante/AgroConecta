CREATE OR ALTER TRIGGER dbo.trg_Produtor_AuditaAprovacao
ON dbo.Produtor
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF TRIGGER_NESTLEVEL() > 1
        RETURN;

    INSERT INTO dbo.LogAcesso (UsuarioId, Acao, Entidade, EntidadeId, IP, UserAgent, CriadoEm)
    SELECT inserido.AprovadoPorUsuarioId,
           CONCAT('StatusCadastro:', removido.StatusCadastro, '->', inserido.StatusCadastro),
           'Produtor',
           CAST(inserido.Id AS NVARCHAR(60)),
           NULL,
           'trigger',
           SYSUTCDATETIME()
      FROM inserted AS inserido
      INNER JOIN deleted AS removido ON removido.Id = inserido.Id
     WHERE inserido.StatusCadastro <> removido.StatusCadastro;
END;
