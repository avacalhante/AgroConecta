# Evidências — Infraestrutura e DevOps

## Artefatos

| Arquivo | Função |
|---|---|
| `../PIM_4-web/Dockerfile` | Build multi-stage (.NET 10) e imagem final |
| `../PIM_4-web/.dockerignore` | Exclusões do contexto de build |
| `../PIM_4-web/docker/docker-compose.yml` | SQL Server 2022 + aplicação Web |
| `../PIM_4-web/docker/.env.example` | Variáveis de ambiente esperadas |
| `../PIM_4-web/.github/workflows/ci.yml` | Pipeline de build, testes e imagem |
| `../PIM_4-web/docs/devops.md` | Execução local e plano de implantação no Azure |

## Execução local

```bash
docker compose -f docker/docker-compose.yml up -d --build
# Aplicação: http://localhost:8080
# Swagger:   http://localhost:8080/swagger (ambiente de demonstração)
```

O serviço `web` aguarda o healthcheck do banco e, em `Development`, aplica
migrations e seed automaticamente.

## Pipeline de CI

Em cada push para `main`/`feat/**` e em pull requests:

1. `dotnet restore`.
2. `dotnet build -c Release`.
3. `dotnet test -c Release` com publicação dos resultados `.trx`.
4. Build da imagem Docker da aplicação.

## Plano de implantação no Azure

| Componente | Serviço |
|---|---|
| Web/API | Azure App Service (Linux, .NET 10) ou Container Apps |
| Banco | Azure SQL Database (backup e geo-replicação) |
| Evidências | Azure Blob Storage |
| Segredos | Azure Key Vault |
| Observabilidade | Application Insights |
| CI/CD | GitHub Actions |

Detalhamento (etapas, variáveis e checklist de produção) em
`../PIM_4-web/docs/devops.md`.

## Status

- `Dockerfile`, `docker-compose.yml` e workflow versionados.
- Sintaxe YAML validada.
- Execução real do `docker compose` pendente: o socket do Docker não está
  acessível no ambiente atual.
