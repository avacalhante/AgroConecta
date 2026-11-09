# Enums e transições de estado

Este arquivo é a fonte de verdade para os valores compartilhados entre Web, API e Mobile. Qualquer alteração deve ser refletida em `contracts/openapi.yaml` e commitada no `main`.

## Papéis (`Papel`)

| Valor | Descrição |
|---|---|
| `Admin` | Acesso administrativo completo |
| `Produtor` | Acessa os próprios lotes e pedidos |
| `Comprador` | Consulta catálogo e cria os próprios pedidos |
| `Motorista` | Acessa as próprias entregas |

## Pedido (`PedidoStatus`)

| Valor | Significado |
|---|---|
| `Rascunho` | Pedido criado, ainda não enviado |
| `AguardandoAprovacao` | Aguardando análise comercial |
| `Aprovado` | Pedido aprovado |
| `EmSeparacao` | Em separação no centro de distribuição |
| `EmRota` | Em rota de entrega |
| `Entregue` | Entrega concluída |
| `Cancelado` | Pedido cancelado |

**Transições permitidas:**

```text
Rascunho → AguardandoAprovacao → Aprovado → EmSeparacao → EmRota → Entregue
Qualquer estado não final → Cancelado
```

## Entrega (`EntregaStatus`)

| Valor | Significado |
|---|---|
| `Pendente` | Aguardando início |
| `EmRota` | Motorista em rota |
| `Entregue` | Entrega confirmada |
| `Falha` | Tentativa de entrega falhou |
| `Cancelada` | Entrega cancelada |

**Transições permitidas:**

```text
Pendente → EmRota → Entregue
Pendente → EmRota → Falha
Pendente → Cancelada
```

## Lote (`LoteStatus`)

| Valor | Significado |
|---|---|
| `Disponivel` | Lote disponível para pedido |
| `Reservado` | Lote reservado para um pedido |
| `Esgotado` | Lote sem saldo |

```text
Disponivel → Reservado → Esgotado
```

## Fatura (`FaturaStatus`)

| Valor | Significado |
|---|---|
| `Pendente` | Fatura gerada, não emitida |
| `Emitida` | Fatura emitida |
| `Paga` | Fatura paga |
| `Cancelada` | Fatura cancelada |

```text
Pendente → Emitida → Paga
Pendente → Cancelada
```

## Comunidade (`Comunidade`)

| Valor | Descrição |
|---|---|
| `Tradicional` | Agricultura familiar tradicional |
| `Quilombola` | Comunidade quilombola |
| `Assentamento` | Assentamento rural |
| `Indigena` | Comunidade indígena |
| `Outro` | Outra comunidade ou não informado |

## Regras de consistência

- `Pedido.EmRota` exige uma `Entrega` com status `EmRota`.
- `Pedido.Entregue` exige uma `Entrega` com status `Entregue`.
- `Lote.Esgotado` não pode receber novos itens de pedido.
- `Fatura.Paga` não pode ser cancelada.
- O Mobile trata `409` ao criar lote como “já sincronizado”.
