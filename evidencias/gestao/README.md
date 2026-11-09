# Evidências — Gerenciamento ágil

## Planejamento

O plano completo (visão, arquitetura, cronograma, pacotes de trabalho e
critérios de aceite) está em `../PLAN.md` e em `../PIM_4-web/PLAN.md`.

- **Parte A (Web/API/Banco/DevOps)** — worktree `PIM_4-web`, branch `feat/web-core`.
- **Parte B (Mobile)** — worktree `PIM_4-mobile`, branch `feat/mobile-app`.
- Pacotes A0–A14 organizados em incrementos pequenos, cada um commitado ao final.

## Método de trabalho

- Entregas em fatias verticais pequenas (domínio → aplicação → infraestrutura → API/Web).
- Cada fatia termina com build verde, testes e commit em português.
- Contrato OpenAPI mantido no worktree principal e atualizado antes do consumo
  pelo Mobile.
- Trabalho paralelo separado por worktrees, sem comandos git destrutivos.

## Histórico de entregas (Parte A)

| Incremento | Commit |
|---|---|
| Plano e documentação inicial | `bdda5e0`, `f638ebb`, `bae41f6` |
| Contrato inicial e README | `82f8bb1`, `3cf188e` |
| Estrutura da solução .NET | `2a23505` |
| Domínio (entidades e enums) | `a4580a3` |
| Contexto, seed e migration inicial | `324c520`, `be7cdda`, `3b88096` |
| Scripts de banco, procedures, triggers e Docker | `1159858` |
| Testes do modelo de banco e seed | `8d4db7c` |
| Regras de domínio e validações | `d58b186`, `e910955` |
| API com ProblemDetails, Swagger e testes | `3ab5f49` |
| Autenticação JWT, refresh e políticas | `4004feb` |
| Lotes, pedidos, entregas, evidência e indicadores | `bb7cd1b`, `eb3f6d9`, `084cf2f` |
| Consultas e cadastros Web | `294e656` |
| Telas administrativas com login | `2cbc478` |
| Relatórios, auditoria, segurança e LGPD | `64b15c1`, `f86ffd6` |
| Docker, CI e plano de implantação | `813a2b4` |
| CORS e ajustes de consultas | `781ce47`, `bc421b5` |
| Rotas e faturamento (API + Web + testes) | `18eb579`, `f3c8b18` |
| Testes de fumo e fluxo completo | `3020f87`, `9d1ba8b` |

## Indicadores

- 159 testes automatizados aprovados.
- Cobertura de Domain 92,5% e Application 93,8%.
- Build sem avisos e sem erros em `Release`.

## Riscos e pendências

| Risco | Mitigação |
|---|---|
| Plugin `docker compose` ausente no ambiente | Validação feita com `docker run`/`docker exec`; compose versionado para ambientes com o plugin |
| Limite de disco impede build da imagem Docker | Dockerfile versionado; build deve rodar em CI ou máquina com espaço |
| Segredos de desenvolvimento nos arquivos de configuração | Trocar por Key Vault/ambiente em produção (documentado) |
