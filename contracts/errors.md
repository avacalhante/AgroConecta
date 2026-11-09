# Erros da API

A API usa **RFC 7807 — Problem Details for HTTP APIs**.

## Formato

```json
{
  "type": "https://agroconecta.local/errors/validacao",
  "title": "Erro de validação",
  "status": 400,
  "detail": "Um ou mais campos são inválidos.",
  "traceId": "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01",
  "errors": {
    "email": ["E-mail inválido."]
  }
}
```

## Códigos

| HTTP | `type` | Uso |
|---:|---|---|
| 400 | `/errors/validacao` | Entrada inválida; inclui `errors` por campo |
| 401 | `/errors/nao-autenticado` | Token ausente, inválido ou expirado |
| 403 | `/errors/sem-permissao` | Usuário autenticado sem o papel necessário |
| 404 | `/errors/nao-encontrado` | Recurso inexistente |
| 409 | `/errors/conflito` | Conflito de unicidade (ex.: `codigoRastreio`) |
| 422 | `/errors/regra-negocio` | Regra de negócio violada |
| 500 | `/errors/inesperado` | Erro não tratado |

## Regras para o Mobile

| Situação | Ação |
|---|---|
| `400` | Exibir mensagens por campo; não repetir automaticamente |
| `401` | Tentar refresh; se falhar, ir para login |
| `403` | Exibir mensagem de permissão; não repetir |
| `404` | Remover item do cache local e avisar |
| `409` em lote | Tratar como sincronizado |
| `422` | Exibir mensagem de negócio; não repetir |
| `5xx` ou rede | Retry com backoff exponencial (máx. 5) |

## Regras para a API

- Nunca retornar stack trace em produção.
- Sempre incluir `traceId`.
- Não expor detalhes de infraestrutura.
- Validar entrada antes de chamar o domínio.
- Diferenciar `403` (sem permissão) de `404` (recurso de outro usuário) quando a existência for sensível: usar `404` para não vazar informação.
