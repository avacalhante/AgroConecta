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

## Validação executada em 2026-11-09

SQL Server 2022 em container (`docker run ... mcr.microsoft.com/mssql/server:2022-latest`):

| Verificação | Resultado |
|---|---|
| `db/script-completo.sql` em banco limpo | Executado com sucesso (tabelas + procedures + triggers + seed) |
| Tabelas criadas | 15 (14 de negócio + `__EFMigrationsHistory`) |
| Procedures | 3 |
| Triggers | 3 |
| Seed | 5 usuários, 2 pedidos, 2 entregas, 1 fatura |
| `dotnet ef database update` em banco limpo | Migration `20261109220959_Inicial` aplicada |
| `sp_CalcularTotalPedido` | Total do pedido 1 = R$ 425,00 |
| `sp_ConsolidarEstoqueLote` | Lote 1: entradas 120, saídas 50, saldo 70 |
| `sp_RelatorioVendasProdutor` | 1 pedido, total R$ 425,00 |
| `trg_Entrega_LogStatus` | Log de auditoria criado na mudança de status |
| `trg_Produtor_AuditaAprovacao` | 2 logs gerados na mudança de situação |
| `trg_ItemPedido_MovimentoEstoque` | Movimento de estoque gerado na inclusão de item |

> O plugin `docker compose` não está instalado neste ambiente (apenas o engine).
> A validação foi feita com comandos `docker run`/`docker exec` equivalentes.
> O build da imagem da aplicação não foi executado por falta de espaço em disco.
