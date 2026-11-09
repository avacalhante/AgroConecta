# Seed de desenvolvimento

Dados mínimos para desenvolvimento, testes manuais e demonstração. **Nunca usar em produção.**

## Usuários

| Nome | E-mail | Papel | Senha (dev) |
|---|---|---|---|
| Ana Administradora | `admin@agroconecta.local` | `Admin` | `Admin@123` |
| Paulo Produtor | `produtor@agroconecta.local` | `Produtor` | `Produtor@123` |
| Carla Compradora | `comprador@agroconecta.local` | `Comprador` | `Comprador@123` |
| Marcos Motorista | `motorista@agroconecta.local` | `Motorista` | `Motorista@123` |

## Cooperativa

| Campo | Valor |
|---|---|
| Nome | Cooperativa Vale Verde |
| Cidade/UF | Campinas/SP |
| Tipo | Agricultura familiar |
| Ativa | Sim |

## Produtores

| Nome | Propriedade | Comunidade | Cooperativa |
|---|---|---|---|
| Paulo Produtor | Sítio Boa Esperança | `Tradicional` | Vale Verde |
| Maria Quilombola | Comunidade Quilombo do Vale | `Quilombola` | Vale Verde |

## Produtos

| Nome | Categoria | Unidade | Preço-base |
|---|---|---|---:|
| Tomate italiano | Hortifrúti | kg | 8,50 |
| Alface crespa | Hortifrúti | unidade | 3,20 |
| Milho verde | Grãos | kg | 5,40 |

## Lotes

| Produto | Produtor | Código de rastreio | Data | Quantidade | Status |
|---|---|---|---|---:|---|
| Tomate italiano | Paulo | `550e8400-e29b-41d4-a716-446655440001` | 2026-11-01 | 120 | `Disponivel` |
| Alface crespa | Maria | `550e8400-e29b-41d4-a716-446655440002` | 2026-11-02 | 80 | `Reservado` |

## Pedidos

| Comprador | Status | Valor | Itens |
|---|---|---:|---|
| Carla Compradora | `Aprovado` | 425,00 | 50 kg de tomate |
| Carla Compradora | `AguardandoAprovacao` | 320,00 | 100 unidades de alface |

## Rota e entregas

| Rota | Motorista | Veículo | Entregas |
|---|---|---|---|
| Rota 2026-11-05 | Marcos | Van ABC-1234 | Pedido 1 (`EmRota`), Pedido 2 (`Pendente`) |

## Faturas

| Pedido | Número | Valor | Status |
|---|---|---:|---|
| Pedido 1 | FAT-2026-0001 | 425,00 | `Emitida` |

## Regras do seed

- Senhas apenas para desenvolvimento; documentadas no README.
- Datas fixas para testes reproduzíveis.
- Nenhum dado pessoal real.
- O seed deve ser idempotente ou recriar o banco em ambiente local.
