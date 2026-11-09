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
