# Evidências — AgroConecta (Partes A e B)

Este diretório reúne as evidências produzidas para o PIM IV.
Cada subpasta corresponde a um critério de avaliação do manual.

| Pasta | Critério do manual | Conteúdo |
|---|---|---|
| `web/` | 05 — Desenvolvimento da solução Web | Telas implementadas, rotas, controle de acesso |
| `api/` | 05/13 — API e contrato | Endpoints, autenticação e exemplos de uso |
| `banco/` | 07 — Projeto do Banco de Dados | Modelo, procedures, triggers e índices |
| `devops/` | 08 — Infraestrutura e DevOps | Docker, CI e plano de implantação |
| `mobile/` | 06 — Desenvolvimento da solução Mobile | Integração real, matriz de testes e acessibilidade |
| `testes/` | 05/11 — Qualidade | Saídas de build, testes e cobertura |
| `inclusao/` | 04 — Responsabilidade social e diversidade | Inclusão digital, acessibilidade e LGPD |
| `gestao/` | 09 — Gerenciamento ágil | Planejamento, backlog e histórico de entregas |

## Status

- Build e testes do backend executados em 2026-11-09: **159 testes aprovados**, 0 falhas.
- Cobertura do backend: **Domain 92,5%** e **Application 93,8%** (meta ≥ 70%).
- Contrato OpenAPI com **22 caminhos**, incluindo rotas e faturas.
- SQL Server 2022 validado em container: script completo, migrations, 3 procedures
  e 3 triggers executados com sucesso (detalhes em `banco/`).
- Fluxo completo validado via API real e telas capturadas em `web/capturas/`.
- Mobile: `npm run check` verde com **17 suítes e 79 testes**; integração real com
  a API, APK de release e matriz manual em `mobile/`.
- Build da imagem Docker da aplicação pendente por limite de espaço em disco e
  ausência do plugin `docker compose` (engine disponível).

## Como reproduzir

```bash
# Backend (na raiz do repositório)
dotnet build AgroConecta.sln -c Release
dotnet test  AgroConecta.sln -c Release
dotnet test  AgroConecta.sln -c Release --collect:"XPlat Code Coverage"

# Mobile
cd mobile
npm install
npm run check

# Ambiente completo
docker compose -f docker/docker-compose.yml up -d --build
```

As capturas de tela de `web/capturas/` foram geradas com a aplicação em execução
contra o SQL Server em container (ver `devops/`).
