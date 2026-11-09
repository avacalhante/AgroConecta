-- ============================================================================
-- AgroConecta — script completo do banco (SQL Server 2022)
-- Gerado por scripts/gerar-script-banco.sh; não edite manualmente.
--
-- Uso:
--   sqlcmd -S localhost,1433 -U sa -P "AgroConecta@2026" -C -d AgroConecta -i db/script-completo.sql
--
-- O banco de destino precisa existir. Crie com:
--   sqlcmd -S localhost,1433 -U sa -P "AgroConecta@2026" -C -Q "CREATE DATABASE AgroConecta"
-- ============================================================================
GO

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Cooperativa] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(150) NOT NULL,
    [Cidade] nvarchar(100) NOT NULL,
    [UF] nvarchar(2) NOT NULL,
    [Tipo] nvarchar(60) NOT NULL,
    [Ativa] bit NOT NULL,
    [CriadoEm] datetime2 NOT NULL,
    [AtualizadoEm] datetime2 NULL,
    CONSTRAINT [PK_Cooperativa] PRIMARY KEY ([Id])
);

CREATE TABLE [Produto] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(120) NOT NULL,
    [Categoria] nvarchar(80) NOT NULL,
    [Unidade] nvarchar(20) NOT NULL,
    [PrecoBase] decimal(18,2) NOT NULL,
    [Ativo] bit NOT NULL,
    [CriadoEm] datetime2 NOT NULL,
    [AtualizadoEm] datetime2 NULL,
    CONSTRAINT [PK_Produto] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_Produto_PrecoBase] CHECK ([PrecoBase] >= 0)
);

CREATE TABLE [Usuario] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(150) NOT NULL,
    [Email] nvarchar(200) NOT NULL,
    [SenhaHash] nvarchar(255) NOT NULL,
    [Papel] nvarchar(20) NOT NULL,
    [Ativo] bit NOT NULL,
    [CriadoEm] datetime2 NOT NULL,
    [AtualizadoEm] datetime2 NULL,
    CONSTRAINT [PK_Usuario] PRIMARY KEY ([Id])
);

CREATE TABLE [Veiculo] (
    [Id] int NOT NULL IDENTITY,
    [Placa] nvarchar(10) NOT NULL,
    [Tipo] nvarchar(40) NOT NULL,
    [CapacidadeKg] decimal(18,2) NOT NULL,
    [Ativo] bit NOT NULL,
    [CriadoEm] datetime2 NOT NULL,
    [AtualizadoEm] datetime2 NULL,
    CONSTRAINT [PK_Veiculo] PRIMARY KEY ([Id])
);

CREATE TABLE [LogAcesso] (
    [Id] bigint NOT NULL IDENTITY,
    [UsuarioId] int NULL,
    [Acao] nvarchar(60) NOT NULL,
    [Entidade] nvarchar(60) NOT NULL,
    [EntidadeId] nvarchar(60) NULL,
    [IP] nvarchar(45) NULL,
    [UserAgent] nvarchar(300) NULL,
    [CriadoEm] datetime2 NOT NULL,
    CONSTRAINT [PK_LogAcesso] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_LogAcesso_Usuario_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuario] ([Id]) ON DELETE SET NULL
);

CREATE TABLE [Motorista] (
    [Id] int NOT NULL IDENTITY,
    [UsuarioId] int NOT NULL,
    [CNH] nvarchar(20) NOT NULL,
    [Ativo] bit NOT NULL,
    [CriadoEm] datetime2 NOT NULL,
    [AtualizadoEm] datetime2 NULL,
    CONSTRAINT [PK_Motorista] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Motorista_Usuario_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuario] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Pedido] (
    [Id] int NOT NULL IDENTITY,
    [CompradorId] int NOT NULL,
    [DataPedido] datetime2 NOT NULL,
    [Status] nvarchar(30) NOT NULL,
    [ValorTotal] decimal(18,2) NOT NULL,
    [EnderecoEntrega] nvarchar(300) NOT NULL,
    [CriadoEm] datetime2 NOT NULL,
    [AtualizadoEm] datetime2 NULL,
    CONSTRAINT [PK_Pedido] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_Pedido_ValorTotal] CHECK ([ValorTotal] >= 0),
    CONSTRAINT [FK_Pedido_Usuario_CompradorId] FOREIGN KEY ([CompradorId]) REFERENCES [Usuario] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Produtor] (
    [Id] int NOT NULL IDENTITY,
    [UsuarioId] int NOT NULL,
    [CooperativaId] int NULL,
    [NomePropriedade] nvarchar(150) NOT NULL,
    [Documento] nvarchar(30) NOT NULL,
    [Cidade] nvarchar(100) NOT NULL,
    [UF] nvarchar(2) NOT NULL,
    [Comunidade] nvarchar(30) NOT NULL,
    [StatusCadastro] nvarchar(20) NOT NULL,
    [AprovadoEm] datetime2 NULL,
    [AprovadoPorUsuarioId] int NULL,
    [CriadoEm] datetime2 NOT NULL,
    [AtualizadoEm] datetime2 NULL,
    CONSTRAINT [PK_Produtor] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Produtor_Cooperativa_CooperativaId] FOREIGN KEY ([CooperativaId]) REFERENCES [Cooperativa] ([Id]) ON DELETE SET NULL,
    CONSTRAINT [FK_Produtor_Usuario_AprovadoPorUsuarioId] FOREIGN KEY ([AprovadoPorUsuarioId]) REFERENCES [Usuario] ([Id]) ON DELETE SET NULL,
    CONSTRAINT [FK_Produtor_Usuario_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuario] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Rota] (
    [Id] int NOT NULL IDENTITY,
    [Data] date NOT NULL,
    [MotoristaId] int NOT NULL,
    [VeiculoId] int NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [CriadoEm] datetime2 NOT NULL,
    [AtualizadoEm] datetime2 NULL,
    CONSTRAINT [PK_Rota] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Rota_Motorista_MotoristaId] FOREIGN KEY ([MotoristaId]) REFERENCES [Motorista] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Rota_Veiculo_VeiculoId] FOREIGN KEY ([VeiculoId]) REFERENCES [Veiculo] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Fatura] (
    [Id] int NOT NULL IDENTITY,
    [PedidoId] int NOT NULL,
    [Numero] nvarchar(30) NOT NULL,
    [Valor] decimal(18,2) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [EmitidaEm] datetime2 NULL,
    [PagoEm] datetime2 NULL,
    [CriadoEm] datetime2 NOT NULL,
    [AtualizadoEm] datetime2 NULL,
    CONSTRAINT [PK_Fatura] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_Fatura_Valor] CHECK ([Valor] >= 0),
    CONSTRAINT [FK_Fatura_Pedido_PedidoId] FOREIGN KEY ([PedidoId]) REFERENCES [Pedido] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [LoteColheita] (
    [Id] int NOT NULL IDENTITY,
    [ProdutorId] int NOT NULL,
    [ProdutoId] int NOT NULL,
    [CodigoRastreio] uniqueidentifier NOT NULL,
    [DataColheita] date NOT NULL,
    [Quantidade] decimal(18,3) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [CriadoEm] datetime2 NOT NULL,
    [AtualizadoEm] datetime2 NULL,
    CONSTRAINT [PK_LoteColheita] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_LoteColheita_Quantidade] CHECK ([Quantidade] > 0),
    CONSTRAINT [FK_LoteColheita_Produto_ProdutoId] FOREIGN KEY ([ProdutoId]) REFERENCES [Produto] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_LoteColheita_Produtor_ProdutorId] FOREIGN KEY ([ProdutorId]) REFERENCES [Produtor] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Entrega] (
    [Id] int NOT NULL IDENTITY,
    [PedidoId] int NOT NULL,
    [RotaId] int NULL,
    [Status] nvarchar(20) NOT NULL,
    [DataPrevista] date NOT NULL,
    [DataRealizada] datetime2 NULL,
    [Latitude] float NULL,
    [Longitude] float NULL,
    [FotoUrl] nvarchar(500) NULL,
    [Observacao] nvarchar(500) NULL,
    [CriadoEm] datetime2 NOT NULL,
    [AtualizadoEm] datetime2 NULL,
    CONSTRAINT [PK_Entrega] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Entrega_Pedido_PedidoId] FOREIGN KEY ([PedidoId]) REFERENCES [Pedido] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Entrega_Rota_RotaId] FOREIGN KEY ([RotaId]) REFERENCES [Rota] ([Id]) ON DELETE SET NULL
);

CREATE TABLE [ItemPedido] (
    [Id] int NOT NULL IDENTITY,
    [PedidoId] int NOT NULL,
    [ProdutoId] int NOT NULL,
    [LoteId] int NULL,
    [Quantidade] decimal(18,3) NOT NULL,
    [PrecoUnitario] decimal(18,2) NOT NULL,
    [Subtotal] decimal(18,2) NOT NULL,
    [CriadoEm] datetime2 NOT NULL,
    [AtualizadoEm] datetime2 NULL,
    CONSTRAINT [PK_ItemPedido] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_ItemPedido_PrecoUnitario] CHECK ([PrecoUnitario] >= 0),
    CONSTRAINT [CK_ItemPedido_Quantidade] CHECK ([Quantidade] > 0),
    CONSTRAINT [FK_ItemPedido_LoteColheita_LoteId] FOREIGN KEY ([LoteId]) REFERENCES [LoteColheita] ([Id]) ON DELETE SET NULL,
    CONSTRAINT [FK_ItemPedido_Pedido_PedidoId] FOREIGN KEY ([PedidoId]) REFERENCES [Pedido] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ItemPedido_Produto_ProdutoId] FOREIGN KEY ([ProdutoId]) REFERENCES [Produto] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [MovimentoEstoque] (
    [Id] int NOT NULL IDENTITY,
    [LoteId] int NOT NULL,
    [Tipo] nvarchar(20) NOT NULL,
    [Quantidade] decimal(18,3) NOT NULL,
    [PedidoId] int NULL,
    [CriadoEm] datetime2 NOT NULL,
    [AtualizadoEm] datetime2 NULL,
    CONSTRAINT [PK_MovimentoEstoque] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MovimentoEstoque_LoteColheita_LoteId] FOREIGN KEY ([LoteId]) REFERENCES [LoteColheita] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_MovimentoEstoque_Pedido_PedidoId] FOREIGN KEY ([PedidoId]) REFERENCES [Pedido] ([Id]) ON DELETE SET NULL
);

CREATE INDEX [IX_Cooperativa_Nome] ON [Cooperativa] ([Nome]);

CREATE INDEX [IX_Entrega_RotaId] ON [Entrega] ([RotaId]);

CREATE INDEX [IX_Entrega_Status_DataPrevista] ON [Entrega] ([Status], [DataPrevista]);

CREATE UNIQUE INDEX [UQ_Entrega_PedidoId] ON [Entrega] ([PedidoId]);

CREATE UNIQUE INDEX [UQ_Fatura_Numero] ON [Fatura] ([Numero]);

CREATE UNIQUE INDEX [UQ_Fatura_PedidoId] ON [Fatura] ([PedidoId]);

CREATE INDEX [IX_ItemPedido_LoteId] ON [ItemPedido] ([LoteId]);

CREATE INDEX [IX_ItemPedido_PedidoId] ON [ItemPedido] ([PedidoId]);

CREATE INDEX [IX_ItemPedido_ProdutoId] ON [ItemPedido] ([ProdutoId]);

CREATE INDEX [IX_LogAcesso_Entidade_EntidadeId] ON [LogAcesso] ([Entidade], [EntidadeId]);

CREATE INDEX [IX_LogAcesso_UsuarioId_CriadoEm] ON [LogAcesso] ([UsuarioId], [CriadoEm]);

CREATE INDEX [IX_LoteColheita_ProdutoId] ON [LoteColheita] ([ProdutoId]);

CREATE INDEX [IX_LoteColheita_ProdutorId_DataColheita] ON [LoteColheita] ([ProdutorId], [DataColheita]);

CREATE UNIQUE INDEX [UQ_LoteColheita_CodigoRastreio] ON [LoteColheita] ([CodigoRastreio]);

CREATE UNIQUE INDEX [UQ_Motorista_CNH] ON [Motorista] ([CNH]);

CREATE UNIQUE INDEX [UQ_Motorista_UsuarioId] ON [Motorista] ([UsuarioId]);

CREATE INDEX [IX_MovimentoEstoque_LoteId_CriadoEm] ON [MovimentoEstoque] ([LoteId], [CriadoEm]);

CREATE INDEX [IX_MovimentoEstoque_PedidoId] ON [MovimentoEstoque] ([PedidoId]);

CREATE INDEX [IX_Pedido_CompradorId] ON [Pedido] ([CompradorId]);

CREATE INDEX [IX_Pedido_Status_DataPedido] ON [Pedido] ([Status], [DataPedido]);

CREATE INDEX [IX_Produto_Ativo] ON [Produto] ([Ativo]);

CREATE INDEX [IX_Produto_Nome] ON [Produto] ([Nome]);

CREATE INDEX [IX_Produtor_AprovadoPorUsuarioId] ON [Produtor] ([AprovadoPorUsuarioId]);

CREATE INDEX [IX_Produtor_CooperativaId] ON [Produtor] ([CooperativaId]);

CREATE UNIQUE INDEX [UQ_Produtor_Documento] ON [Produtor] ([Documento]);

CREATE UNIQUE INDEX [UQ_Produtor_UsuarioId] ON [Produtor] ([UsuarioId]);

CREATE INDEX [IX_Rota_MotoristaId_Data] ON [Rota] ([MotoristaId], [Data]);

CREATE INDEX [IX_Rota_VeiculoId] ON [Rota] ([VeiculoId]);

CREATE UNIQUE INDEX [UQ_Usuario_Email] ON [Usuario] ([Email]);

CREATE UNIQUE INDEX [UQ_Veiculo_Placa] ON [Veiculo] ([Placa]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261109220959_Inicial', N'10.0.14');

COMMIT;
GO


GO
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


GO
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


GO
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


GO
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


GO
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


GO
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


GO
-- Carga de desenvolvimento do AgroConecta.
-- Espelha contracts/seed.md e a classe SeedDados do backend.
-- NUNCA executar em produção.

SET NOCOUNT ON;

IF EXISTS (SELECT 1 FROM dbo.Usuario)
BEGIN
    PRINT 'Seed ignorado: o banco já possui usuários.';
    RETURN;
END;

SET IDENTITY_INSERT dbo.Usuario ON;

INSERT INTO dbo.Usuario (Id, Nome, Email, SenhaHash, Papel, Ativo, CriadoEm, AtualizadoEm) VALUES
    (1, 'Ana Administradora', 'admin@agroconecta.local',
     'AQAAAAIAAYagAAAAEOxd9qTIWYoDiH7UYkH/V2jkGFxCmgMzUIgSZtd12d+hAV3u9Kmb8zi0dSlcVUQlWw==',
     'Admin', 1, '2026-11-01T09:00:00', NULL),
    (2, 'Paulo Produtor', 'produtor@agroconecta.local',
     'AQAAAAIAAYagAAAAEIb+y3xTptEyUW5XQGzD6PGGjlI3AQMVKVfQWSjDTaCRFmlL1xGAUSN4Y+on98QtAQ==',
     'Produtor', 1, '2026-11-01T09:00:00', NULL),
    (3, 'Maria Quilombola', 'maria@agroconecta.local',
     'AQAAAAIAAYagAAAAEIb+y3xTptEyUW5XQGzD6PGGjlI3AQMVKVfQWSjDTaCRFmlL1xGAUSN4Y+on98QtAQ==',
     'Produtor', 1, '2026-11-01T09:00:00', NULL),
    (4, 'Carla Compradora', 'comprador@agroconecta.local',
     'AQAAAAIAAYagAAAAEDFPrBWJMOBMqol3LGAxg17KoogdAnzePryT90OyPe0iSCQGUTzBmsEdhNq/zAjVtA==',
     'Comprador', 1, '2026-11-01T09:00:00', NULL),
    (5, 'Marcos Motorista', 'motorista@agroconecta.local',
     'AQAAAAIAAYagAAAAEHDZmN06Q0wNIyUJiyoDCOQj+jCHvYOxAyKYkwFT4iz73+TPGlLWk/nTqaXbAi4jpg==',
     'Motorista', 1, '2026-11-01T09:00:00', NULL);

SET IDENTITY_INSERT dbo.Usuario OFF;

SET IDENTITY_INSERT dbo.Cooperativa ON;

INSERT INTO dbo.Cooperativa (Id, Nome, Cidade, UF, Tipo, Ativa, CriadoEm, AtualizadoEm) VALUES
    (1, 'Cooperativa Vale Verde', 'Campinas', 'SP', 'Agricultura familiar', 1, '2026-11-01T09:00:00', NULL);

SET IDENTITY_INSERT dbo.Cooperativa OFF;

SET IDENTITY_INSERT dbo.Produtor ON;

INSERT INTO dbo.Produtor
    (Id, UsuarioId, CooperativaId, NomePropriedade, Documento, Cidade, UF, Comunidade,
     StatusCadastro, AprovadoEm, AprovadoPorUsuarioId, CriadoEm, AtualizadoEm)
VALUES
    (1, 2, 1, 'Sítio Boa Esperança', '123.456.789-00', 'Campinas', 'SP', 'Tradicional',
     'Aprovado', '2026-11-03T09:00:00', 1, '2026-11-01T09:00:00', NULL),
    (2, 3, 1, 'Comunidade Quilombo do Vale', '987.654.321-00', 'Campinas', 'SP', 'Quilombola',
     'Aprovado', '2026-11-03T09:00:00', 1, '2026-11-01T09:00:00', NULL);

SET IDENTITY_INSERT dbo.Produtor OFF;

SET IDENTITY_INSERT dbo.Motorista ON;

INSERT INTO dbo.Motorista (Id, UsuarioId, CNH, Ativo, CriadoEm, AtualizadoEm) VALUES
    (1, 5, '12345678900', 1, '2026-11-01T09:00:00', NULL);

SET IDENTITY_INSERT dbo.Motorista OFF;

SET IDENTITY_INSERT dbo.Veiculo ON;

INSERT INTO dbo.Veiculo (Id, Placa, Tipo, CapacidadeKg, Ativo, CriadoEm, AtualizadoEm) VALUES
    (1, 'ABC-1234', 'Van', 1500.00, 1, '2026-11-01T09:00:00', NULL);

SET IDENTITY_INSERT dbo.Veiculo OFF;

SET IDENTITY_INSERT dbo.Produto ON;

INSERT INTO dbo.Produto (Id, Nome, Categoria, Unidade, PrecoBase, Ativo, CriadoEm, AtualizadoEm) VALUES
    (1, 'Tomate italiano', 'Hortifrúti', 'kg', 8.50, 1, '2026-11-01T09:00:00', NULL),
    (2, 'Alface crespa', 'Hortifrúti', 'unidade', 3.20, 1, '2026-11-01T09:00:00', NULL),
    (3, 'Milho verde', 'Grãos', 'kg', 5.40, 1, '2026-11-01T09:00:00', NULL);

SET IDENTITY_INSERT dbo.Produto OFF;

SET IDENTITY_INSERT dbo.LoteColheita ON;

INSERT INTO dbo.LoteColheita
    (Id, ProdutorId, ProdutoId, CodigoRastreio, DataColheita, Quantidade, Status, CriadoEm, AtualizadoEm)
VALUES
    (1, 1, 1, '550e8400-e29b-41d4-a716-446655440001', '2026-11-01', 120.000, 'Disponivel',
     '2026-11-01T14:00:00', NULL),
    (2, 2, 2, '550e8400-e29b-41d4-a716-446655440002', '2026-11-02', 80.000, 'Reservado',
     '2026-11-02T14:00:00', NULL);

SET IDENTITY_INSERT dbo.LoteColheita OFF;

SET IDENTITY_INSERT dbo.Pedido ON;

INSERT INTO dbo.Pedido
    (Id, CompradorId, DataPedido, Status, ValorTotal, EnderecoEntrega, CriadoEm, AtualizadoEm)
VALUES
    (1, 4, '2026-11-03T09:00:00', 'EmRota', 425.00, 'Rua das Acácias, 120 — Campinas/SP',
     '2026-11-03T09:00:00', NULL),
    (2, 4, '2026-11-04T10:30:00', 'Aprovado', 320.00, 'Av. dos Ipês, 45 — Valinhos/SP',
     '2026-11-04T10:30:00', NULL);

SET IDENTITY_INSERT dbo.Pedido OFF;

-- Evita que o trigger gere movimentos duplicados durante a carga.
DISABLE TRIGGER dbo.trg_ItemPedido_MovimentoEstoque ON dbo.ItemPedido;

SET IDENTITY_INSERT dbo.ItemPedido ON;

INSERT INTO dbo.ItemPedido
    (Id, PedidoId, ProdutoId, LoteId, Quantidade, PrecoUnitario, Subtotal, CriadoEm, AtualizadoEm)
VALUES
    (1, 1, 1, 1, 50.000, 8.50, 425.00, '2026-11-03T09:00:00', NULL),
    (2, 2, 2, 2, 100.000, 3.20, 320.00, '2026-11-04T10:30:00', NULL);

SET IDENTITY_INSERT dbo.ItemPedido OFF;

ENABLE TRIGGER dbo.trg_ItemPedido_MovimentoEstoque ON dbo.ItemPedido;

SET IDENTITY_INSERT dbo.MovimentoEstoque ON;

INSERT INTO dbo.MovimentoEstoque (Id, LoteId, Tipo, Quantidade, PedidoId, CriadoEm, AtualizadoEm) VALUES
    (1, 1, 'Entrada', 120.000, NULL, '2026-11-01T14:00:00', NULL),
    (2, 2, 'Entrada', 80.000, NULL, '2026-11-02T14:00:00', NULL),
    (3, 1, 'Saida', 50.000, 1, '2026-11-03T09:05:00', NULL);

SET IDENTITY_INSERT dbo.MovimentoEstoque OFF;

SET IDENTITY_INSERT dbo.Rota ON;

INSERT INTO dbo.Rota (Id, Data, MotoristaId, VeiculoId, Status, CriadoEm, AtualizadoEm) VALUES
    (1, '2026-11-05', 1, 1, 'EmAndamento', '2026-11-04T18:00:00', NULL);

SET IDENTITY_INSERT dbo.Rota OFF;

SET IDENTITY_INSERT dbo.Entrega ON;

INSERT INTO dbo.Entrega
    (Id, PedidoId, RotaId, Status, DataPrevista, DataRealizada, Latitude, Longitude,
     FotoUrl, Observacao, CriadoEm, AtualizadoEm)
VALUES
    (1, 1, 1, 'EmRota', '2026-11-05', NULL, NULL, NULL, NULL, NULL,
     '2026-11-04T18:00:00', NULL),
    (2, 2, 1, 'Pendente', '2026-11-06', NULL, NULL, NULL, NULL, NULL,
     '2026-11-04T18:00:00', NULL);

SET IDENTITY_INSERT dbo.Entrega OFF;

SET IDENTITY_INSERT dbo.Fatura ON;

INSERT INTO dbo.Fatura (Id, PedidoId, Numero, Valor, Status, EmitidaEm, PagoEm, CriadoEm, AtualizadoEm) VALUES
    (1, 1, 'FAT-2026-0001', 425.00, 'Emitida', '2026-11-04T10:30:00', NULL,
     '2026-11-04T10:30:00', NULL);

SET IDENTITY_INSERT dbo.Fatura OFF;

SET IDENTITY_INSERT dbo.LogAcesso ON;

INSERT INTO dbo.LogAcesso (Id, UsuarioId, Acao, Entidade, EntidadeId, IP, UserAgent, CriadoEm) VALUES
    (1, 1, 'Seed', 'Banco', 'inicial', '127.0.0.1', 'seed-dev.sql', '2026-11-01T09:00:00');

SET IDENTITY_INSERT dbo.LogAcesso OFF;

PRINT 'Seed de desenvolvimento aplicado.';


GO
