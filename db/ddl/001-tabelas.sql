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

