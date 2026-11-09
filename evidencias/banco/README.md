# Evidências — Projeto do Banco de Dados

Banco SQL Server 2022, acessado via EF Core 10.

## Artefatos versionados

| Arquivo | Conteúdo |
|---|---|
| `../PIM_4-web/db/ddl/001-tabelas.sql` | DDL das tabelas, chaves e índices |
| `../PIM_4-web/db/procedures/*.sql` | Procedures de negócio |
| `../PIM_4-web/db/triggers/*.sql` | Triggers de auditoria e estoque |
| `../PIM_4-web/db/seed/seed-dev.sql` | Carga de desenvolvimento |
| `../PIM_4-web/db/script-completo.sql` | Script único, executável em banco limpo |
| `../PIM_4-web/src/AgroConecta.Infrastructure/Migrations/` | Migrations do EF Core |

## Modelo de dados

14 tabelas de negócio (além de `__EFMigrationsHistory`):

`Cooperativa`, `Produto`, `Usuario`, `Veiculo`, `LogAcesso`, `Motorista`,
`Pedido`, `Produtor`, `Rota`, `Fatura`, `LoteColheita`, `Entrega`,
`ItemPedido`, `MovimentoEstoque`.

Relações principais:

- `Produtor` → `Usuario`, `Cooperativa`, aprovação por `Usuario`.
- `LoteColheita` → `Produtor`, `Produto`; código de rastreio único.
- `Pedido` → `Comprador` (`Usuario`); `ItemPedido` → `Pedido`, `Produto`, `LoteColheita`.
- `Rota` → `Motorista`, `Veiculo`; `Entrega` → `Pedido`, `Rota` (1:1 com pedido).
- `Fatura` → `Pedido` (1:1, número único).
- `MovimentoEstoque` → `LoteColheita`, `Pedido`.
- `LogAcesso` → `Usuario`.

Índices relevantes: `UQ_Entrega_PedidoId`, `UQ_Fatura_PedidoId`,
`UQ_Fatura_Numero`, `UQ_LoteColheita_CodigoRastreio`, `UQ_Produtor_Documento`,
`IX_Entrega_Status_DataPrevista`, `IX_Pedido_Status_DataPedido`.

## Procedures

| Procedure | Função |
|---|---|
| `sp_CalcularTotalPedido` | Recalcula o total do pedido a partir dos itens |
| `sp_ConsolidarEstoqueLote` | Consolida entradas e saídas por lote |
| `sp_RelatorioVendasProdutor` | Relatório de vendas por produtor no período |

## Triggers

| Trigger | Função |
|---|---|
| `trg_Entrega_LogStatus` | Registra mudanças de status da entrega |
| `trg_ItemPedido_MovimentoEstoque` | Gera movimento de saída ao incluir item |
| `trg_Produtor_AuditaAprovacao` | Audita aprovação de produtor |

## Como aplicar

```bash
# Via Docker (aplicação aplica migrations e seed no ambiente de demonstração)
docker compose -f docker/docker-compose.yml up -d --build

# Ou manualmente, em um SQL Server acessível
sqlcmd -S localhost,1433 -U sa -P 'AgroConecta@2026' -C -i db/script-completo.sql
```

> A execução contra SQL Server real ainda não foi capturada: o socket do Docker
> não está acessível neste ambiente. Os scripts compilam e estão versionados.
