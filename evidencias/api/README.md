# Evidências — API e contrato

API REST versionada em `/api/v1`, com autenticação JWT (Bearer) e respostas de
erro no padrão RFC 7807 (`application/problem+json`).

Contrato: `../contracts/openapi.yaml` (22 caminhos). Swagger disponível em
`/swagger` quando `ASPNETCORE_ENVIRONMENT=Development`.

## Autenticação

```bash
# 1. Login (retorna access + refresh token)
curl -s -X POST http://localhost:8080/api/v1/auth/login \
  -H 'Content-Type: application/json' \
  -d '{"email":"admin@agroconecta.local","senha":"Admin@123"}'

# 2. Uso do token nas demais chamadas
TOKEN='<accessToken>'
curl -s http://localhost:8080/api/v1/produtos -H "Authorization: Bearer $TOKEN"
```

- Access token curto + refresh token, diferenciados pela claim `token_type`.
- Logout é descarte no cliente e responde `204`.
- Rate limit de login: 10 tentativas/minuto por IP em produção.

## Endpoints principais

| Método | Rota | Papel | Descrição |
|---|---|---|---|
| POST | `/api/v1/auth/login` | Anônimo | Autenticação JWT |
| POST | `/api/v1/auth/refresh` | Anônimo | Renovação do token |
| POST | `/api/v1/auth/logout` | Autenticado | Encerra a sessão (204) |
| GET/POST | `/api/v1/produtos` | Autenticado / Admin·Produtor | Catálogo |
| GET/POST | `/api/v1/lotes` | Autenticado / Produtor | Lotes e rastreabilidade |
| GET | `/api/v1/lotes/meus` | Produtor | Lotes do produtor autenticado |
| GET/POST | `/api/v1/pedidos` | Autenticado / Comprador | Pedidos |
| PATCH | `/api/v1/pedidos/{id}/status` | Admin·Produtor·Motorista | Transição de status |
| GET | `/api/v1/entregas/minhas` | Motorista | Entregas atribuídas |
| PATCH | `/api/v1/entregas/{id}/status` | Motorista | Atualização da entrega |
| PUT | `/api/v1/entregas/{id}/evidencia` | Motorista | Foto + coordenadas |
| GET/POST | `/api/v1/rotas` | Admin | Rotas e criação de entregas |
| GET | `/api/v1/faturas` | Admin | Faturas emitidas |
| POST | `/api/v1/faturas/pedido/{pedidoId}` | Admin | Fatura idempotente por pedido |
| GET | `/api/v1/relatorios/vendas-produtor` | Admin | Vendas por produtor |
| GET | `/api/v1/indicadores` | Admin | Indicadores do dashboard |
| GET | `/api/v1/auditoria` | Admin | Trilha de auditoria |
| GET | `/health` | Anônimo | Saúde da aplicação e do banco |

## Exemplos de resposta

Criação de rota (`POST /api/v1/rotas`), corpo:

```json
{
  "data": "2026-11-20",
  "motoristaId": 1,
  "veiculoId": 1,
  "pedidoIds": [3]
}
```

Resposta `201 Created`:

```json
{
  "id": 2,
  "data": "2026-11-20",
  "motorista": "Marcos Motorista",
  "veiculo": "ABC-1234 · Van",
  "status": "Planejada",
  "entregas": [
    { "id": 3, "pedidoId": 3, "comprador": "Carla Compradora", "status": "Pendente", "dataPrevista": "2026-11-20" }
  ]
}
```

Erro de regra de negócio (`422 Unprocessable Entity`):

```json
{
  "type": "https://agroconecta.local/errors/regra-negocio",
  "title": "Regra de negócio violada",
  "status": 422,
  "detail": "O pedido 3 já está vinculado a uma entrega.",
  "instance": "/api/v1/rotas",
  "traceId": "..."
}
```

## Verificação automatizada

`AgroConecta.Tests/Api/` cobre autenticação, escopo por papel, validações,
rotas, faturas e o fluxo completo produtor → produto → lote → pedido → rota →
entrega → fatura.
