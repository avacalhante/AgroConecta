# Evidências — Parte A (Web/API/Banco/DevOps)

Este diretório reúne as evidências produzidas para a Parte A do PIM IV.
Cada subpasta corresponde a um critério de avaliação do manual.

| Pasta | Critério do manual | Conteúdo |
|---|---|---|
| `web/` | 05 — Desenvolvimento da solução Web | Telas implementadas, rotas, controle de acesso |
| `api/` | 05/13 — API e contrato | Endpoints, autenticação e exemplos de uso |
| `banco/` | 07 — Projeto do Banco de Dados | Modelo, procedures, triggers e índices |
| `devops/` | 08 — Infraestrutura e DevOps | Docker, CI e plano de implantação |
| `testes/` | 05/11 — Qualidade | Saídas de build, testes e cobertura |
| `inclusao/` | 04 — Responsabilidade social e diversidade | Inclusão digital, acessibilidade e LGPD |
| `gestao/` | 09 — Gerenciamento ágil | Planejamento, backlog e histórico de entregas |

## Status

- Build e testes automatizados executados em 2026-11-09: **159 testes aprovados**, 0 falhas.
- Cobertura: **Domain 92,5%** e **Application 93,8%** (meta ≥ 70%).
- Contrato OpenAPI com **22 caminhos**, incluindo rotas e faturas.
- SQL Server 2022 validado em container: script completo, migrations, 3 procedures
  e 3 triggers executados com sucesso (detalhes em `banco/`).
- Fluxo completo validado via API real e telas capturadas em `web/capturas/`.
- Build da imagem Docker da aplicação pendente por limite de espaço em disco e
  ausência do plugin `docker compose` (engine disponível).

## Como reproduzir

```bash
# A partir de ../PIM_4-web
dotnet build AgroConecta.sln -c Release
dotnet test  AgroConecta.sln -c Release
dotnet test  AgroConecta.sln -c Release --collect:"XPlat Code Coverage"

docker compose -f docker/docker-compose.yml up -d --build
```

As capturas de tela dependem da aplicação em execução com SQL Server; devem ser
geradas após subir o ambiente descrito em `devops/`.
