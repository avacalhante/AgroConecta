# PLAN.md — Plano de Desenvolvimento do AgroConecta

> **Projeto:** PIM IV — Análise e Desenvolvimento de Sistemas (UNIP)
> **Tema:** Desenvolvimento e Implantação de uma Solução Tecnológica Corporativa Integrada para Apoiar a Transformação Digital de uma Empresa.
> **Título de trabalho:** AgroConecta: plataforma corporativa integrada para gestão da cadeia de suprimentos e inclusão digital na agricultura familiar.
> **Versão:** 1.0 — 09/11/2026
> **Status:** plano aprovado para execução

**Documentos de referência:**

| Arquivo | Papel |
|---|---|
| `docs/manual.md` | Fonte de verdade do PIM IV. Nenhuma decisão pode contrariar o manual. |
| `docs/rascunho.md` | Escopo funcional v2.0: premissas, MVP, modelo de dados e cortes. |
| `PLAN.md` | Este documento: como o sistema será construído, em que ordem e com quais evidências. |
| `documento/` | Texto final do PIM em ABNT (a criar). |
| `evidencias/` | Capturas, vídeos, diagramas, scripts e relatórios (a criar). |
| `gestao/` | Backlog, sprints, Kanban e riscos (a criar). |

> **Atenção:** O PIM final e as evidências devem ficar em `documento/` e `evidencias/`, que serão versionados.

---

## 1. Objetivo do plano

Levar o projeto do repositório vazio até a entrega final do PIM IV, com:

1. Protótipo funcional Web (.NET 10 + ASP.NET Core MVC/API + SQL Server 2022).
2. Aplicativo Mobile (React Native + Expo + TypeScript) com operação offline básica.
3. Banco de dados modelado, script SQL completo, procedures e triggers.
4. Contrato de API versionado e compartilhado entre as duas frentes.
5. Plano de cloud/DevOps, pipeline CI/CD e evidências de execução.
6. Documento final em ABNT, com 20–30 páginas de desenvolvimento.
7. Artefatos ágeis: Product Backlog, Sprint Backlog, Kanban, DoR/DoD.

O plano é **incremental**: cada tarefa termina com código funcionando, evidência registrada e commit em português.

---

## 2. Resultado final (Definition of Success)

Ao final, uma pessoa deve conseguir:

```bash
git clone <repositório>
docker compose -f docker/docker-compose.yml up -d
dotnet ef database update -p src/AgroConecta.Infrastructure -s src/AgroConecta.Web
dotnet run --project src/AgroConecta.Web
cd mobile && npm install && npx expo start
```

E, a partir disso:

- Autenticar na Web como administrador e no Mobile como motorista/produtor.
- Cadastrar produtor, cooperativa e produto.
- Registrar lote de colheita.
- Criar pedido, aprovar, faturar e programar entrega.
- Atualizar status de entrega no Mobile, inclusive sem internet.
- Sincronizar as ações offline ao reconectar.
- Consultar dashboard, relatório de vendas e painel de inclusão.
- Executar o script SQL completo em banco limpo.
- Ler o PIM final em ABNT e localizar as evidências de cada etapa.

**Critério de aceite global:** todos os critérios 01–09 do manual têm artefato de evidência rastreável na seção 15.

---

## 3. Divisão principal: duas partes paralelas com git worktree

### 3.1 As duas grandes partes

| Parte | Nome | Escopo | Worktree | Branch |
|---|---|---|---|---|
| **A** | Núcleo corporativo e Web | Banco, API .NET, painel Web MVC, segurança, relatórios, DevOps | `../PIM_4-web` | `feat/web-core` |
| **B** | Aplicativo Mobile | App Expo, autenticação, fluxos de produtor e motorista, offline, evidências | `../PIM_4-mobile` | `feat/mobile-app` |

### 3.2 Por que podem rodar em paralelo

- **Diretórios disjuntos:** a Parte A trabalha em `src/`, `db/`, `docker/`, `.github/` e `scripts/`; a Parte B trabalha em `mobile/`.
- **Contrato primeiro:** as duas frentes concordam com `contracts/openapi.yaml`, `contracts/enums.md` e `contracts/errors.md` antes de implementar.
- **Mobile com mock:** a Parte B começa com um servidor mock baseado no contrato (Prism) e troca para a API real no fim do Sprint 2.
- **Integração em pontos definidos:** login (fim do Sprint 1), lotes (fim do Sprint 2), entregas/offline (fim do Sprint 3) e ponta a ponta (fim do Sprint 4).
- **Sem arquivos compartilhados em edição simultânea:** `contracts/`, `README.md`, `AGENTS.md` e `PLAN.md` são editados apenas no worktree principal.

### 3.3 Worktrees e branches

O repositório principal (`/home/v/Projetos/work/PIM_4`) permanece em `main` e concentra contrato, documentação, integração e release.

Após o commit inicial:

```bash
cd /home/v/Projetos/work/PIM_4

# Parte A
git worktree add ../PIM_4-web -b feat/web-core

# Parte B
git worktree add ../PIM_4-mobile -b feat/mobile-app

git worktree list
```

Resultado esperado:

```text
/home/v/Projetos/work/PIM_4          main
/home/v/Projetos/work/PIM_4-web      feat/web-core
/home/v/Projetos/work/PIM_4-mobile   feat/mobile-app
```

### 3.4 Ownership de arquivos

| Caminho | Dono | Regra |
|---|---|---|
| `src/`, `db/`, `docker/`, `.github/`, `scripts/` | Parte A | Só a Parte A edita |
| `mobile/` | Parte B | Só a Parte B edita |
| `contracts/` | Integração (main) | Alterações via main; features apenas consomem |
| `documento/`, `evidencias/`, `gestao/` | Documentação (main) | Alimentados pelas duas partes |
| `README.md`, `AGENTS.md`, `PLAN.md`, `.gitignore` | Integração (main) | Alterações via main |

### 3.5 Contrato primeiro

Fluxo obrigatório para qualquer funcionalidade que cruze a fronteira Web/Mobile:

1. Atualizar `contracts/openapi.yaml` (endpoint, schema, enum, erro).
2. Atualizar `contracts/enums.md` e `contracts/errors.md`, se necessário.
3. Commitar no `main` com mensagem em português.
4. Fazer `git merge main` dentro de `../PIM_4-web` e `../PIM_4-mobile`.
5. Implementar cada lado contra o contrato.
6. Validar com lint + teste de contrato + chamada real.

**Ferramentas:**

```bash
# Validar o contrato
npx @redocly/cli lint contracts/openapi.yaml

# Mock para a Parte B
npx @stoplight/prism-cli mock contracts/openapi.yaml --port 4010

# Gerar tipos TypeScript para o Mobile
npx openapi-typescript contracts/openapi.yaml -o mobile/src/types/api.d.ts
```

### 3.6 Cadência de integração

| Momento | Ação |
|---|---|
| Diário | Cada worktree commita o que terminou; nenhuma tarefa fica sem commit |
| Semanal | `git merge main` dentro de cada feature branch |
| Fim de sprint | `git merge --no-ff feat/web-core` e `git merge --no-ff feat/mobile-app` no `main` |
| Fim de sprint | Rodar build, testes, contrato e fluxo E2E |
| Marco | Criar tag `sprint-1`, `sprint-2`, `sprint-3`, `sprint-4`, `rc-1` |

### 3.7 Regras de git

**Permitido:**

- `git add`, `git commit`, `git status`, `git diff`, `git log`.
- `git switch` entre branches.
- `git merge` (com resolução manual de conflitos).
- `git worktree add`, `git worktree list`, `git worktree remove` (somente com worktree limpo).
- `git tag`.

**Proibido (comandos destrutivos):**

- `git reset --hard`
- `git clean -fd`
- `git push --force` / `--force-with-lease`
- `git branch -D`
- `git checkout -- .` / `git restore` em alterações alheias
- `git rebase` sem acordo explícito entre as frentes

Em caso de conflito: resolver manualmente, rodar os testes e commitar. Nunca descartar o trabalho da outra parte.

### 3.8 Política de commits

- Mensagens **em português**, imperativas e curtas.
- **Sem footer e sem identificação de coautor.**
- Um commit por tarefa ou sub-tarefa concluída.
- Exemplos:

```text
adiciona estrutura inicial da solução .NET
modela banco de dados e migrations iniciais
implementa autenticação JWT com perfis
cria tela de login do aplicativo
implementa fila de sincronização offline
documenta etapa 1 do PIM
```

### 3.9 Capacidade e alocação

| Equipe | Alocação sugerida |
|---|---|
| 2 pessoas | 1 na Parte A (caminho crítico) + 1 na Parte B; documentação compartilhada |
| 3 pessoas | 2 na Parte A (dados/API e Web) + 1 na Parte B |
| 4 pessoas | 2 na Parte A + 1 na Parte B + 1 na trilha D/integração |
| 5–6 pessoas | 2 na Parte A + 2 na Parte B + 1 na trilha D + 1 em QA/integração |

> A Parte A é o caminho crítico. Se houver desequilíbrio, mover alguém da Parte B para ajudar em banco, testes e documentação após o B10.

> **Trabalho individual:** aplicar o corte do rascunho (seção 12.3) — uma persona mobile (motorista), 11 tabelas, 6 telas Web e 5 Mobile. Manter autenticação, pedido, entrega, offline básico e ABNT. Nesse cenário, a Parte B começa apenas após A3–A7, e o cronograma deve ser alongado ou o escopo reduzido.

---

## 4. Arquitetura e stack

### 4.1 Visão geral

```text
┌──────────────────────┐        ┌──────────────────────┐
│  Web MVC (admin)     │        │  Mobile (Expo)       │
│  cookie auth         │        │  JWT + SQLite        │
└──────────┬───────────┘        └──────────┬───────────┘
           │                               │
           │        HTTP/JSON              │
           ▼                               ▼
┌──────────────────────────────────────────────────────┐
│  AgroConecta.Web (ASP.NET Core 10)                   │
│  MVC + Web API v1 + Swagger                         │
└──────────────────────┬───────────────────────────────┘
                       │
┌──────────────────────▼───────────────────────────────┐
│  Application (casos de uso, DTOs, validadores)       │
└──────────────────────┬───────────────────────────────┘
                       │
┌──────────────────────▼───────────────────────────────┐
│  Domain (entidades, enums, regras, interfaces)       │
└──────────────────────▲───────────────────────────────┘
                       │
┌──────────────────────┴───────────────────────────────┐
│  Infrastructure (EF Core, repositórios, serviços)    │
└──────────────────────┬───────────────────────────────┘
                       │
                ┌──────▼──────┐
                │ SQL Server  │
                └─────────────┘
```

**Decisão:** monólito modular em 4 camadas. Web MVC e Web API vivem no mesmo processo, compartilhando Application e Domain. O MVC usa cookie authentication; a API usa JWT para o Mobile. Não há microsserviços nem mensageria no MVP.

### 4.2 Stack

| Camada | Tecnologia | Versão/observação |
|---|---|---|
| Runtime | .NET | 10 (LTS, SDK disponível na máquina) |
| Web/API | ASP.NET Core MVC + Web API | Um projeto `AgroConecta.Web` |
| ORM | EF Core | 10 |
| Banco | SQL Server | 2022, via Docker |
| Autenticação Web | ASP.NET Core Identity | Cookie |
| Autenticação API | JWT | Access + refresh |
| Validação | FluentValidation | — |
| Testes backend | xUnit + Moq | — |
| Documentação API | Swashbuckle/OpenAPI | `/swagger` |
| Mobile | React Native + Expo | TypeScript |
| Navegação | React Navigation | Stack + tabs/drawer |
| Offline | expo-sqlite | Fila local |
| Token seguro | expo-secure-store | — |
| HTTP | Axios | Interceptors |
| Testes mobile | Jest + RNTL | — |
| Containers | Docker + Compose | API + SQL Server |
| CI/CD | GitHub Actions | Build, test, deploy opcional |

### 4.3 Ambientes

| Ambiente | Uso | Banco | Deploy |
|---|---|---|---|
| Local | Desenvolvimento e demonstração | SQL Server em Docker | `dotnet run` + Expo |
| CI | Build e testes | SQL Server em container | GitHub Actions |
| Staging | Validação integrada (opcional) | Azure SQL | Azure App Service |
| Produção | Não exigida pelo PIM | — | — |

---

## 5. Estrutura de diretórios

```text
PIM_4/
├── AGENTS.md
├── PLAN.md
├── README.md
├── .gitignore
├── contracts/
│   ├── openapi.yaml
│   ├── enums.md
│   ├── errors.md
│   └── seed.md
├── db/
│   ├── ddl/
│   ├── seed/
│   ├── procedures/
│   ├── triggers/
│   └── script-completo.sql
├── src/
│   ├── AgroConecta.Domain/
│   ├── AgroConecta.Application/
│   ├── AgroConecta.Infrastructure/
│   ├── AgroConecta.Web/
│   └── AgroConecta.Tests/
├── mobile/
│   ├── src/
│   │   ├── api/
│   │   ├── components/
│   │   ├── db/
│   │   ├── navigation/
│   │   ├── screens/
│   │   ├── sync/
│   │   ├── theme/
│   │   ├── types/
│   │   └── utils/
│   ├── app.json
│   ├── package.json
│   └── tsconfig.json
├── docker/
│   ├── Dockerfile
│   └── docker-compose.yml
├── .github/
│   └── workflows/
│       ├── backend.yml
│       └── mobile.yml
├── scripts/
│   ├── dev-up.sh
│   ├── db-reset.sh
│   └── check.sh
├── documento/
│   ├── pim.md                  # texto-fonte versionável
│   ├── pim.docx                # diagramação ABNT
│   ├── template-abnt.docx      # template com estilos
│   ├── pim.pdf                 # versão final
│   ├── figuras/
│   └── referencias.md
├── evidencias/
│   ├── web/
│   ├── mobile/
│   ├── banco/
│   ├── api/
│   ├── devops/
│   ├── social/
│   └── agile/
└── gestao/
    ├── product-backlog.md
    ├── sprints/
    ├── kanban.md
    ├── dor-dod.md
    └── riscos.md
```

---

## 6. Contrato compartilhado

### 6.1 Convenções da API

| Item | Regra |
|---|---|
| Base URL | `/api/v1` |
| Formato | JSON, `camelCase` |
| Datas | ISO 8601 UTC (`2026-11-09T14:30:00Z`) |
| IDs | Inteiros |
| Valores | `decimal` com 2 casas |
| Paginação | `?page=1&pageSize=20` → `{ items, page, pageSize, total }` |
| Ordenação | `?sort=campo&order=asc|desc` |
| Filtros | Query string, documentados no OpenAPI |
| Erros | RFC 7807 `ProblemDetails` |
| Versionamento | Prefixo `/v1`; mudança incompatível cria `/v2` |
| Health | `GET /health` |

### 6.2 Autenticação

| Fluxo | Endpoint | Retorno |
|---|---|---|
| Login | `POST /api/v1/auth/login` | `accessToken`, `refreshToken`, `expiresIn`, `usuario` |
| Renovação | `POST /api/v1/auth/refresh` | Novo par de tokens |
| Logout | `POST /api/v1/auth/logout` | `204` |
| Perfil | `GET /api/v1/me` | Usuário autenticado |

- Access token: 15 minutos.
- Refresh token: 7 dias, com rotação.
- Header: `Authorization: Bearer <token>`.
- Papéis: `Admin`, `Produtor`, `Comprador`, `Motorista`.
- Senhas: hash PBKDF2 via ASP.NET Core Identity.
- Bloqueio temporário após 5 tentativas inválidas.

### 6.3 Endpoints do MVP

| Método | Rota | Papéis | Finalidade |
|---|---|---|---|
| POST | `/api/v1/auth/login` | Público | Autenticar |
| POST | `/api/v1/auth/refresh` | Autenticado | Renovar token |
| POST | `/api/v1/auth/logout` | Autenticado | Encerrar sessão |
| GET | `/api/v1/me` | Autenticado | Perfil atual |
| GET/POST/PUT | `/api/v1/produtores` | Admin | Gerenciar produtores |
| PATCH | `/api/v1/produtores/{id}/aprovacao` | Admin | Aprovar cadastro |
| GET/POST/PUT | `/api/v1/cooperativas` | Admin | Gerenciar cooperativas |
| GET | `/api/v1/produtos` | Admin/Comprador | Consultar catálogo |
| POST/PUT | `/api/v1/produtos` | Admin | Gerenciar catálogo |
| GET/POST | `/api/v1/lotes` | Admin/Produtor | Listar e criar lotes |
| GET | `/api/v1/lotes/meus` | Produtor | Lotes do produtor |
| GET/POST | `/api/v1/pedidos` | Admin/Comprador | Criar e listar pedidos |
| GET | `/api/v1/pedidos/{id}` | Admin/Comprador | Detalhe do pedido |
| PATCH | `/api/v1/pedidos/{id}/status` | Admin | Alterar status |
| GET/POST | `/api/v1/rotas` | Admin | Programar rotas |
| GET | `/api/v1/entregas/minhas` | Motorista | Entregas do motorista |
| PATCH | `/api/v1/entregas/{id}/status` | Motorista | Atualizar entrega |
| PUT | `/api/v1/entregas/{id}/evidencia` | Motorista | Foto + localização |
| GET | `/api/v1/faturas` | Admin | Listar faturas |
| GET | `/api/v1/relatorios/vendas-produtor` | Admin | Relatório |
| GET | `/api/v1/indicadores` | Admin | Dashboard |
| GET | `/api/v1/auditoria` | Admin | Logs de acesso |
| GET | `/health` | Público | Saúde da aplicação |

> **Acesso por linha:** Comprador só acessa seus próprios pedidos/faturas; Motorista só acessa suas entregas; Produtor só acessa seus lotes. O filtro é aplicado no Application, não apenas na UI.

### 6.4 Enums e status

| Enum | Valores |
|---|---|
| `Papel` | `Admin`, `Produtor`, `Comprador`, `Motorista` |
| `PedidoStatus` | `Rascunho`, `AguardandoAprovacao`, `Aprovado`, `EmSeparacao`, `EmRota`, `Entregue`, `Cancelado` |
| `EntregaStatus` | `Pendente`, `EmRota`, `Entregue`, `Falha`, `Cancelada` |
| `LoteStatus` | `Disponivel`, `Reservado`, `Esgotado` |
| `FaturaStatus` | `Pendente`, `Emitida`, `Paga`, `Cancelada` |
| `Comunidade` | `Tradicional`, `Quilombola`, `Assentamento`, `Indigena`, `Outro` |

### 6.5 Erros

Formato padrão:

```json
{
  "type": "https://agroconecta.local/errors/validacao",
  "title": "Erro de validação",
  "status": 400,
  "detail": "Um ou mais campos são inválidos.",
  "traceId": "00-...",
  "errors": {
    "email": ["E-mail inválido."]
  }
}
```

| Código | Uso |
|---|---|
| 400 | Validação de entrada |
| 401 | Não autenticado |
| 403 | Sem permissão |
| 404 | Recurso inexistente |
| 409 | Conflito (ex.: código de rastreio duplicado) |
| 422 | Regra de negócio violada |
| 500 | Erro inesperado |

### 6.6 Idempotência e offline

O MVP evita tabela de idempotência usando chaves naturais:

| Operação | Estratégia |
|---|---|
| Criar lote | `codigoRastreio` UUID gerado no cliente; `UNIQUE` no banco; `409` é tratado como sucesso no Mobile |
| Atualizar status | `PATCH` define o estado-alvo; repetir a chamada não altera o resultado |
| Enviar evidência | `PUT` substitui a evidência da entrega; repetir é seguro |

**Fila offline:**

1. Ação grava no SQLite local e na fila.
2. UI atualiza de forma otimista.
3. Ao detectar conexão, a fila é processada em ordem.
4. Sucesso → remove da fila; `409` de lote → trata como já sincronizado.
5. `4xx` → marca como falha e exibe ao usuário.
6. `5xx`/rede → retry com backoff exponencial (máx. 5 tentativas).
7. Conflito → servidor vence; cache local é atualizado e o evento é registrado.

### 6.7 Modelo de dados

- 14 tabelas de negócio definidas em `docs/rascunho.md`, seção 9.
- Tabelas de infraestrutura do ASP.NET Core Identity e `RefreshToken` não entram no modelo de negócio, mas aparecem no script completo.
- `Usuario` é mapeado para a tabela de usuários do Identity.
- Script completo em `db/script-completo.sql`, gerado a partir das migrations e complementado com procedures/triggers.

---

## 7. Parte A — Núcleo corporativo e Web

### 7.1 Objetivo e branch

Entregar banco, API, painel administrativo, segurança, relatórios e infraestrutura. É o **caminho crítico** do projeto: o Mobile depende do contrato e da API.

- Worktree: `../PIM_4-web`
- Branch: `feat/web-core`
- Diretórios: `src/`, `db/`, `docker/`, `.github/`, `scripts/`

### 7.2 Pacotes de trabalho

| ID | Tarefa | Entregável | Pronto quando | Dias |
|---|---|---|---|---:|
| **A0** | Bootstrap da solução .NET | Solution, 5 projetos, referências, pacotes, README | `dotnet build` verde; estrutura versionada | 1 |
| **A1** | Modelo de dados e migrations | Entidades, DbContext, configurações, migration inicial, seed | `dotnet ef database update` em banco limpo; FKs e índices criados | 3 |
| **A2** | Camadas, DI e repositórios | Domain, Application, Infrastructure, casos de uso base | Build verde; testes de domínio iniciais | 2 |
| **A3** | Autenticação e autorização | Identity, JWT, refresh, papéis, policies, seed de usuários | Login/refresh funcionam; 401/403 corretos; senha com hash | 2 |
| **A4** | Cadastros | Produtor, cooperativa, produto e comprador; aprovação | CRUD + validação + paginação + autorização | 2 |
| **A5** | Catálogo e pedidos | Produtos, pedidos, itens, cálculo de total, transições | Pedido criado; total correto; transições válidas | 3 |
| **A6** | Lotes e estoque | LoteColheita, MovimentoEstoque, rastreabilidade | Lote criado; movimento gerado; saldo consultável | 2 |
| **A7** | Rotas, entregas e faturamento | Rota, Entrega, Fatura, evidência | Rota com entregas; motorista vê as suas; fatura gerada | 3 |
| **A8** | Web administrativo | Layout, login, 10 telas, navegação, autorização | Todas as telas funcionais com dados do seed | 4 |
| **A9** | Relatórios e indicadores | Dashboard, vendas por produtor, inclusão, auditoria | Consultas com filtros; dados corretos | 2 |
| **A10** | Segurança, LGPD e acessibilidade | Checklist OWASP, headers, rate limit, consentimento, WCAG | Checklist preenchido; telas acessíveis | 2 |
| **A11** | Testes backend | Unitários, integração, smoke tests | `dotnet test` verde; cobertura ≥70% em Domain/Application | 2 |
| **A12** | DevOps | Dockerfile, compose, CI, plano Azure | `docker compose up` funciona; CI verde | 2 |
| **A13** | Contrato e documentação da API | Swagger, OpenAPI versionado, exemplos | Contrato válido; Swagger UI acessível | 1 |
| **A14** | Suporte à integração | Seed para Mobile, CORS, health, correções | Mobile autentica e consome a API real | 1 |

**Total estimado:** 32 dias de uma pessoa. Com duas pessoas na Parte A, dividir em “dados/API” (A1–A7, A13) e “Web/qualidade/infra” (A8–A12, A14).

### 7.3 Ordem de execução

```text
A0 → A1 → A2 → A3 → A4 → A5 → A6 → A7 → A8 → A9 → A10 → A11 → A12 → A13 → A14
                                      │
                                      └── A8 pode começar após A3 (telas de cadastro)
```

- **A1 antes de tudo:** o banco sustenta API e Mobile.
- **A3 cedo:** destrava o login ponta a ponta no Sprint 1.
- **A8 em paralelo com A5–A7:** o layout e as telas de cadastro podem avançar enquanto os fluxos de pedido/entrega são implementados.
- **A10 e A11 contínuos:** não deixar acessibilidade e testes para o fim.

### 7.4 Critérios de aceite da Parte A

> **Status em 2026-11-09:** critérios de aceite verificados neste ambiente.
> SQL Server 2022 foi validado em container (script completo, migrations,
> procedures e triggers). O plugin `docker compose` não está instalado e o build
> da imagem da aplicação ficou pendente por limite de espaço em disco; a
> aplicação foi executada diretamente contra o banco real.

- [x] `dotnet build` e `dotnet test` verdes.
- [x] Banco criado do zero com um comando.
- [x] Script SQL completo executável em banco limpo.
- [x] Swagger disponível e contrato válido.
- [x] Login Web (cookie) e API (JWT) funcionando.
- [x] 10 telas Web funcionais com dados reais do seed.
- [x] Fluxo completo: produtor → produto → lote → pedido → rota → entrega → fatura.
- [x] Procedures e triggers executando.
- [x] Acessibilidade mínima validada.
- [x] Nenhum segredo commitado (apenas chaves de desenvolvimento documentadas).
- [x] Evidências em `evidencias/web/`, `evidencias/api/`, `evidencias/banco/`.

---

## 8. Parte B — Aplicativo Mobile

### 8.1 Objetivo e branch

Entregar o aplicativo do produtor e do motorista, com autenticação, operação offline e integração com a API.

- Worktree: `../PIM_4-mobile`
- Branch: `feat/mobile-app`
- Diretório: `mobile/`

### 8.2 Pacotes de trabalho

| ID | Tarefa | Entregável | Pronto quando | Dias |
|---|---|---|---|---:|
| **B0** | Bootstrap Expo | App TypeScript, libs, lint, testes, estrutura de pastas | `npx expo start` funciona; `npm test` e `npm run lint` verdes | 1 |
| **B1** | Design system e navegação | Tema, componentes, auth stack, tabs/drawer | Navegação completa com telas mock | 1 |
| **B2** | Autenticação mobile | Login, secure store, refresh, logout, biometria opcional | Login real; sessão persistida; 401 redireciona | 2 |
| **B3** | Cliente de API | Axios, base URL, interceptors, tipos gerados, mock | Chamadas mock e real; erros tratados | 1 |
| **B4** | Fluxo do produtor | Meus lotes, novo lote, validação, fila | Criar lote offline e sincronizar | 2 |
| **B5** | Fluxo do motorista | Minhas entregas, detalhe, status, evidência | Atualizar status online/offline | 2 |
| **B6** | Sincronização offline | SQLite, fila, retry, conflitos, listener de conexão | Teste em modo avião; fila persiste após reinício | 3 |
| **B7** | Câmera e localização | Permissões, foto, compressão, upload | Evidência salva e enviada | 1 |
| **B8** | Acessibilidade mobile | Font scaling, labels, contraste, leitor de tela | Checklist WCAG; teste com leitor | 1 |
| **B9** | Testes mobile | Jest, RNTL, matriz manual | `npm test` verde; matriz executada | 2 |
| **B10** | Build e integração | App config, APK/EAS, env, E2E | APK instalável; fluxo completo com API real | 2 |

**Total estimado:** 18 dias de uma pessoa. A Parte B pode começar no dia 1 com mock e trocar para a API real no fim do Sprint 2.

> Em dispositivo físico, configurar `EXPO_PUBLIC_API_URL` com o IP da máquina (ex.: `http://192.168.0.10:5000/api/v1`), não `localhost`.

### 8.3 Ordem de execução

```text
B0 → B1 → B2 → B3 → B4 → B5 → B6 → B7 → B8 → B9 → B10
              │
              └── B4/B5 podem iniciar com mock antes da API real
```

- **B0–B3 no Sprint 1:** app rodando, navegação, login com mock e cliente de API.
- **B4–B5 no Sprint 2:** fluxos principais contra o mock; integração real ao final.
- **B6–B7 no Sprint 3:** offline e evidências.
- **B8–B10 no Sprint 4:** acessibilidade, testes, build e integração final.

### 8.4 Critérios de aceite da Parte B

> **Status em 2026-11-09:** critérios verificados. `npm run check` (lint +
> typecheck + Jest) passou com **17 suítes e 79 testes**; a integração real com a
> API e o APK de release estão registrados em `evidencias/mobile/`.

- [x] `npm run lint` e `npm test` verdes.
- [x] App roda em Expo Go/emulador.
- [x] 6 telas principais implementadas: login, home, lista, detalhe, status/evidência e perfil/sincronização.
- [x] Login com usuário do seed.
- [x] Produtor registra lote offline e sincroniza.
- [x] Motorista atualiza entrega com foto e localização.
- [x] Fila offline persiste após fechar e reabrir o app.
- [x] Conflito e erro de rede são tratados e visíveis.
- [x] Checklist de acessibilidade aplicado.
- [x] APK/EAS build gerado.
- [x] Evidências em `evidencias/mobile/`.

> O APK de release (`arm64-v8a`, 27 MB) foi gerado localmente com Gradle e
> instalado no aparelho de teste; o EAS cloud não foi executado por falta de
> credenciais da conta Expo, mas os perfis estão em `mobile/eas.json`.

---

## 9. Trilha transversal — Documentação, ABNT e evidências

Esta trilha roda no worktree principal, em paralelo às Partes A e B. Não é uma terceira parte de código; é o que transforma o protótipo em PIM válido.

### 9.1 Pacotes de trabalho

| ID | Tarefa | Entregável | Pronto quando | Dias |
|---|---|---|---|---:|
| **D0** | Template ABNT e estrutura | Template DOCX/ODT com estilos, capa, folha de rosto, resumo, abstract, sumário; fonte em Markdown | Template aprovado; pasta `documento/` criada | 1 |
| **D1** | Etapas 1 e 2 | Empresa-caso, organograma, BPMN, dores, SWOT, Lean Canvas, viabilidade | 6 páginas com citações | 3 |
| **D2** | Etapa 3 | Base legal, ações, acessibilidade, indicadores, LGPD | 2 páginas com evidências | 2 |
| **D3** | Etapas 4 e 5 | Web, Mobile, diagramas, telas, endpoints, offline | 8 páginas com figuras | 3 |
| **D4** | Etapas 6 e 7 | Arquitetura, MER, dicionário, DDL, procedures, triggers | 7 páginas com diagramas e script | 3 |
| **D5** | Etapas 8 e 9 | Cloud, DevOps, backlog, sprints, Kanban, DoR/DoD | 4 páginas com artefatos | 2 |
| **D6** | Resumo, abstract, conclusão e referências | Texto final | 3–4 páginas em ABNT | 2 |
| **D7** | Pacote de evidências | Capturas, vídeos, relatórios, saídas SQL, checklist | `evidencias/` completo | 2 |
| **D8** | Revisão final | ABNT, plágio, PDF, pacote de entrega | Checklist final assinado | 2 |

### 9.2 Orçamento de páginas do desenvolvimento

| Seção | Páginas |
|---|---:|
| Etapa 1 — Organização | 3 |
| Etapa 2 — Planejamento e viabilidade | 3 |
| Etapa 3 — Social e diversidade | 2 |
| Etapa 4 — Web | 4 |
| Etapa 5 — Mobile | 4 |
| Etapa 6 — Arquitetura | 3 |
| Etapa 7 — Banco | 4 |
| Etapa 8 — Cloud/DevOps | 2 |
| Etapa 9 — Ágil | 2 |
| **Total** | **27** |

Introdução: 2–3 páginas. Conclusão: 1–2 páginas. Referências: 2–3 páginas.

O texto é escrito em `documento/pim.md` (versionável) e diagramado em `documento/pim.docx` com o template ABNT. A entrega final é `documento/pim.pdf`. A formatação ABNT é validada no DOCX, não no Markdown.

### 9.3 Evidências por critério

| Critério | Evidências |
|---|---|
| 01 — ABNT | PDF final, checklist ABNT, referências |
| 02 — Organização | Perfil da empresa, organograma, BPMN, SWOT |
| 03 — Planejamento | Lean Canvas, tabela de viabilidade, backlog |
| 04 — Social | Checklist WCAG, capturas, consulta de indicadores, política LGPD |
| 05 — Web | Repositório, Swagger, 10 telas, vídeo de demonstração |
| 06 — Mobile | APK, 6 telas, vídeo com modo avião, teste de sync |
| 07 — Banco | MER, dicionário, DDL, saída de execução, procedures/triggers |
| 08 — Cloud/DevOps | Diagrama Azure, Dockerfile, workflow YAML, custos |
| 09 — Ágil | Product Backlog, Sprint Backlog, Kanban, burndown, DoR/DoD |

---

## 10. Cronograma e marcos

### 10.1 Visão semanal

> O cronograma-base de 10 semanas assume 3–4 pessoas ou cerca de 3 dias úteis por semana. Com 2 pessoas, estender para 12–14 semanas ou aplicar o plano de corte da seção 14.

| Semana | Parte A | Parte B | Documentação | Marco |
|---|---|---|---|---|
| **0** | A0 + contrato v0.1 | B0 | D0 | **M0** — Repositório, contrato e template |
| **1–2** | A1–A3 | B1–B2 | D0–D1 | **M1** — Login ponta a ponta e banco v1 |
| **3–4** | A4–A6 | B3–B4 | D1–D2 | **M2** — Cadastros e lote |
| **5–6** | A7–A8 | B5–B6 | D3–D4 | **M3** — Entregas e offline |
| **7–8** | A9–A12 | B7–B9 | D4–D5 | **M4** — Features completas |
| **9** | A13–A14 + hardening | B10 + correções | D6–D7 | **M5** — Release candidate |
| **10** | Correções finais | Correções finais | D8 | **M6** — Entrega |

### 10.2 Marcos

| Marco | Critério |
|---|---|
| **M0** | `main` com plano, contrato v0.1, template ABNT e worktrees criados |
| **M1** | Login real no Mobile; Web admin autenticando; banco com migrations |
| **M2** | Cadastros e lotes funcionando; contrato de pedidos fechado |
| **M3** | Rota/entrega/fatura na Web; Motorista atualiza entrega offline |
| **M4** | Dashboard, relatórios, acessibilidade, testes e CI verdes |
| **M5** | APK gerado; script SQL completo; PIM em revisão; evidências coletadas |
| **M6** | PDF final em ABNT; pacote completo; entrega no sistema da UNIP |

### 10.3 Dependências críticas

```text
Contrato v0.1
   ├── A1 (banco)
   │     └── A2 → A3 → A4 → A5 → A6 → A7 → A8
   └── B1 → B2 → B3 → B4/B5
                    │
                    └── B6 (offline) depende de A7 (endpoints de entrega)
```

- **B3 depende do contrato v0.1.**
- **B4/B5 dependem de B3; integração real depende de A3–A7.**
- **B6 depende de A7.**
- **D3 depende de A8 e B5 para capturas reais.**
- **D4 depende de A1.**
- **D5 depende de A12 e do backlog.**

---

## 11. Qualidade

### 11.1 Definition of Ready (DoR)

Uma tarefa só entra em execução quando:

- [ ] Tem critério de aceite claro.
- [ ] Tem contrato de API definido, se cruzar Web/Mobile.
- [ ] Tem tela/protótipo definido, se for UI.
- [ ] Tem dependências identificadas.
- [ ] Tem estimativa e responsável.
- [ ] Tem evidência esperada definida.

### 11.2 Definition of Done (DoD)

Uma tarefa só é considerada pronta quando:

- [ ] Código implementado e revisado.
- [ ] Build e testes verdes.
- [ ] Contrato atualizado (se aplicável).
- [ ] Migration aplicada (se aplicável).
- [ ] Acessibilidade verificada (se UI).
- [ ] Documentação da seção correspondente atualizada.
- [ ] Evidência capturada.
- [ ] Commit em português realizado.
- [ ] Nenhum segredo ou artefato gerado commitado.

### 11.3 Code review

- [ ] Nomes claros e consistentes.
- [ ] Sem duplicação desnecessária.
- [ ] Regras de negócio no Domain/Application, não no Controller.
- [ ] DTOs separados de entidades.
- [ ] Validação de entrada.
- [ ] Tratamento de erro padronizado.
- [ ] Consultas eficientes (sem N+1).
- [ ] Testes cobrindo caminho feliz e um erro.
- [ ] Sem `TODO` esquecido.

### 11.4 Acessibilidade

**Web:**

- [ ] Contraste AA.
- [ ] Navegação por teclado.
- [ ] Foco visível.
- [ ] Labels associados aos campos.
- [ ] Mensagens de erro claras.
- [ ] Tabelas com cabeçalhos.
- [ ] Imagens com `alt`.

**Mobile:**

- [ ] `accessibilityLabel` e `accessibilityRole`.
- [ ] Área de toque mínima de 44×44.
- [ ] Fonte ajustável.
- [ ] Contraste AA.
- [ ] Teste com leitor de tela em um fluxo completo.

### 11.5 Segurança e LGPD

- [ ] Senhas com hash PBKDF2.
- [ ] JWT com expiração curta e refresh rotativo.
- [ ] Autorização por papel em todos os endpoints.
- [ ] Validação de entrada e proteção contra mass assignment.
- [ ] EF Core com consultas parametrizadas.
- [ ] HTTPS, CORS restrito e rate limit no login.
- [ ] Segredos em variáveis de ambiente/Key Vault.
- [ ] Checklist OWASP Top 10.
- [ ] Consentimento no cadastro.
- [ ] Minimização de dados.
- [ ] Auditoria de acessos e alterações.
- [ ] Política de privacidade no sistema.

---

## 12. Estratégia de testes

### 12.1 Backend

| Tipo | Ferramenta | Escopo |
|---|---|---|
| Unitário | xUnit + Moq | Entidades, regras de pedido, cálculo de frete, validadores |
| Aplicação | xUnit | Casos de uso com repositórios mockados |
| Integração | `WebApplicationFactory` + SQL Server de teste | Endpoints, autenticação, persistência |
| Contrato | Validação OpenAPI + smoke tests | Schemas, status codes, enums |
| Banco | Script em banco limpo | DDL, DML, procedures, triggers |

**Comandos:**

```bash
dotnet test
dotnet test --collect:"XPlat Code Coverage"
```

**Meta:** 70% de cobertura em Domain/Application. Não é gate eliminatório, mas entra no DoD.

### 12.2 Mobile

| Tipo | Ferramenta | Escopo |
|---|---|---|
| Unitário | Jest | Fila offline, validadores, formatadores |
| Componente | React Native Testing Library | Login, formulários, lista, detalhe |
| Manual | Matriz de dispositivos | Offline, permissões, câmera, erros |

**Comandos:**

```bash
cd mobile
npm test
npm run lint
```

### 12.3 Matriz manual mínima

| Cenário | Resultado esperado |
|---|---|
| Login válido | Entra e persiste sessão |
| Login inválido | Mensagem clara; não entra |
| Token expirado | Refresh automático ou redireciona ao login |
| Criar lote offline | Salva local; sincroniza ao reconectar |
| Fechar e reabrir app com fila | Fila permanece |
| Duplicar lote | Servidor retorna 409; app trata como sincronizado |
| Atualizar entrega offline | status local; sincroniza depois |
| Foto + localização | Evidência salva e enviada |
| Servidor 500 | Retry com backoff; erro visível |
| Sem permissão de câmera | App orienta e não quebra |

---

## 13. CI/CD e ambientes

### 13.1 Ambiente local

```bash
# Banco
docker compose -f docker/docker-compose.yml up -d db

# API/Web
dotnet run --project src/AgroConecta.Web

# Mobile
cd mobile
npx expo start
```

### 13.2 Pipeline CI

**Backend (`backend.yml`):**

1. Checkout.
2. Setup .NET 10.
3. Restore.
4. Build.
5. Testes com cobertura.
6. Validação do contrato OpenAPI.
7. Build da imagem Docker (opcional).

**Mobile (`mobile.yml`):**

1. Checkout.
2. Setup Node 20.
3. `npm ci`.
4. `npm run lint`.
5. `npm test`.
6. Verificação de tipos TypeScript.

### 13.3 Pipeline CD (opcional)

1. Deploy em staging no Azure App Service.
2. Aplicar migrations no Azure SQL.
3. Health check.
4. Aprovação manual.
5. Deploy em produção.

> O manual exige **plano de execução** de cloud/DevOps. O deploy real é diferencial; o pipeline e o plano documentado já atendem ao critério 08.

### 13.4 Segredos e configuração

| Variável | Onde |
|---|---|
| `ConnectionStrings__Default` | `.env` local / GitHub Secrets / Key Vault |
| `Jwt__Key` | `.env` local / GitHub Secrets / Key Vault |
| `Jwt__Issuer`, `Jwt__Audience` | `appsettings.json` |
| `EXPO_PUBLIC_API_URL` | `mobile/.env` |
| `MSSQL_SA_PASSWORD` | `.env` local / CI secret |

- Commitar apenas `.env.example` e `appsettings.Example.json`.
- Nunca commitar `.env`, senhas ou tokens.

---

## 14. Riscos e contingências

| Risco | Prob. | Impacto | Mitigação | Gatilho |
|---|---|---|---|---|
| Escopo maior que o prazo | Alta | Alto | MVP já cortado; priorizar Web, Mobile e Banco | Atraso > 3 dias em um marco |
| Contrato desalinhado | Média | Alto | Contract-first + lint + integração semanal | Teste de contrato falhando |
| Conflito de sincronização offline | Média | Médio | Last-write-wins + log + testes em modo avião | Duplicidade ou perda de dado |
| SQL Server indisponível no Linux | Baixa | Alto | Docker Compose; plano B: PostgreSQL com adaptação documentada | Container não sobe |
| Curva de aprendizado Mobile | Média | Médio | Expo + mock + começar cedo | B2 atrasado |
| Atraso na ABNT | Alta | Alto | Template na semana 0; escrita paralela | Menos de 10 páginas no fim do Sprint 2 |
| Perda de trabalho | Baixa | Alto | Commits pequenos e frequentes; sem comandos destrutivos | Worktree sujo por muitos dias |
| Dados fictícios pouco críveis | Média | Médio | Declarar ficção; seed consistente; fontes externas | Revisão da Etapa 1 |
| Falha de segurança | Baixa | Alto | Checklist OWASP; revisão; testes de autorização | Endpoint sem policy |
| LGPD | Baixa | Médio | Consentimento, minimização, auditoria | Dados pessoais sem base legal |

### Plano de corte se o prazo apertar

**Cortar nesta ordem:**

1. Biometria.
2. Módulo de capacitação.
3. Relatórios secundários.
4. Fluxo do produtor no Mobile (manter motorista).
5. Deploy real em Azure.

**Nunca cortar:**

- Autenticação.
- Cadastros essenciais.
- Pedido, entrega e faturamento.
- Script SQL completo.
- Documento ABNT.
- Evidências dos critérios 05, 06 e 07.

---

## 15. Rastreabilidade com o manual

| Critério | Peso | Etapa do manual | Tarefas | Evidência | Seção do PIM |
|---|---:|---|---|---|---|
| 01 — ABNT | 1,0 | Transversal | D0, D1–D8 | PDF, checklist, referências | Todo o documento |
| 02 — Organização | 1,0 | 1 | D1 | Empresa-caso, organograma, BPMN, SWOT | 6.1 |
| 03 — Planejamento | 1,0 | 2 | D1, A5–A7 | Lean Canvas, viabilidade, backlog | 6.2 |
| 04 — Social | 0,5 | 3 | A10, B8, D2 | Checklist WCAG, indicadores, LGPD | 6.3 |
| 05 — Web | 2,0 | 4 | A3–A8, A13 | Repositório, Swagger, 10 telas, vídeo | 6.4 |
| 06 — Mobile | 2,0 | 5 | B1–B10 | APK, 6 telas, vídeo offline | 6.5 |
| 07 — Banco | 1,0 | 7 | A1, A6 | MER, DDL, script, procedures/triggers | 6.7 |
| 08 — Cloud/DevOps | 0,5 | 8 | A12 | Diagrama Azure, Dockerfile, pipeline | 6.8 |
| 09 — Ágil | 1,0 | 9 | D5, gestão | Backlog, sprints, Kanban, DoR/DoD | 6.9 |

**Etapa 6 (arquitetura)** não tem critério próprio, mas sustenta os critérios 05, 06 e 07; será documentada em 6.6 com diagramas de camadas e classes.

---

## 16. Primeiras 48 horas

### Dia 1 — Fundação

- [ ] Commitar `AGENTS.md`, `.gitignore` e `PLAN.md` no `main`.
- [ ] Criar `contracts/openapi.yaml` v0.1 com auth, health e enums.
- [ ] Criar `contracts/enums.md`, `contracts/errors.md` e `contracts/seed.md`.
- [ ] Commitar o contrato.
- [ ] Criar os worktrees `../PIM_4-web` e `../PIM_4-mobile`.
- [ ] Criar `README.md` com visão geral, comandos e estrutura.

### Dia 1 — Parte A

- [ ] `dotnet new sln`, projetos e referências (A0).
- [ ] `dotnet build` verde.
- [ ] Commit: `adiciona estrutura inicial da solução .NET`.

### Dia 1 — Parte B

- [ ] `npx create-expo-app` e instalar dependências (B0).
- [ ] Estrutura de pastas, lint e teste inicial.
- [ ] Commit: `cria estrutura inicial do aplicativo mobile`.

### Dia 2 — Banco e navegação

- [ ] Parte A: Docker Compose do SQL Server + migration inicial (A1).
- [ ] Parte A: seed com usuários, cooperativa, produtores e produtos.
- [ ] Commit: `modela banco de dados e carga inicial`.
- [ ] Parte B: tema, navegação e telas mock (B1).
- [ ] Commit: `adiciona navegação e tema do aplicativo`.
- [ ] Integração: `git merge main` nos dois worktrees; rodar builds.
- [ ] Marcar `M0` com tag `m0-fundacao`.

### Critério de saída das 48 horas

- `main` com contrato v0.1 e README.
- Parte A compilando e banco subindo.
- Parte B rodando com navegação mock.
- Worktrees limpos e commits em português.
- Nenhum comando destrutivo usado.

---

## 17. Apêndices

### A. Variáveis de ambiente

```env
# .env.example
MSSQL_SA_PASSWORD=SenhaForte123!
ConnectionStrings__Default=Server=localhost,1433;Database=AgroConecta;User Id=sa;Password=SenhaForte123!;TrustServerCertificate=True;
Jwt__Key=chave-local-com-pelo-menos-32-caracteres
Jwt__Issuer=AgroConecta
Jwt__Audience=AgroConectaApp
ASPNETCORE_ENVIRONMENT=Development
```

```env
# mobile/.env.example
EXPO_PUBLIC_API_URL=http://localhost:5000/api/v1
```

### B. Seed mínimo

| Usuário | Papel | Senha (dev) |
|---|---|---|
| `admin@agroconecta.local` | Admin | `Admin@123` |
| `produtor@agroconecta.local` | Produtor | `Produtor@123` |
| `comprador@agroconecta.local` | Comprador | `Comprador@123` |
| `motorista@agroconecta.local` | Motorista | `Motorista@123` |

Dados: 1 cooperativa, 2 produtores, 3 produtos, 2 lotes, 2 pedidos, 1 rota, 2 entregas e 2 faturas.

> Senhas de desenvolvimento, documentadas e nunca usadas em produção.

### C. Comandos úteis

```bash
# .NET
dotnet new sln -n AgroConecta
dotnet new classlib -n AgroConecta.Domain -o src/AgroConecta.Domain
dotnet new classlib -n AgroConecta.Application -o src/AgroConecta.Application
dotnet new classlib -n AgroConecta.Infrastructure -o src/AgroConecta.Infrastructure
dotnet new mvc -n AgroConecta.Web -o src/AgroConecta.Web
dotnet new xunit -n AgroConecta.Tests -o src/AgroConecta.Tests
dotnet sln add src/AgroConecta.Domain/AgroConecta.Domain.csproj
dotnet sln add src/AgroConecta.Application/AgroConecta.Application.csproj
dotnet sln add src/AgroConecta.Infrastructure/AgroConecta.Infrastructure.csproj
dotnet sln add src/AgroConecta.Web/AgroConecta.Web.csproj
dotnet sln add src/AgroConecta.Tests/AgroConecta.Tests.csproj
dotnet build
dotnet test

# EF Core
dotnet ef migrations add InitialCreate -p src/AgroConecta.Infrastructure -s src/AgroConecta.Web
dotnet ef database update -p src/AgroConecta.Infrastructure -s src/AgroConecta.Web
dotnet ef migrations script -p src/AgroConecta.Infrastructure -s src/AgroConecta.Web -o db/script-completo.sql

# Docker
docker compose -f docker/docker-compose.yml up -d
docker compose -f docker/docker-compose.yml logs -f db

# Mobile
npx create-expo-app@latest mobile --template blank-typescript
cd mobile
npm install
npx expo start
npm test
npm run lint

# Contrato
npx @redocly/cli lint contracts/openapi.yaml
npx @stoplight/prism-cli mock contracts/openapi.yaml --port 4010
npx openapi-typescript contracts/openapi.yaml -o mobile/src/types/api.d.ts

# Git
git worktree add ../PIM_4-web -b feat/web-core
git worktree add ../PIM_4-mobile -b feat/mobile-app
git worktree list
git merge main
git merge --no-ff feat/web-core
git tag sprint-1
```

### D. Checklist ABNT resumido

- [ ] Papel A4; margens 3/2/3/2 cm.
- [ ] Arial ou Times New Roman 12; citações longas e notas em 10.
- [ ] Espaçamento 1,5; citações longas, notas, referências e resumo em espaço simples.
- [ ] Recuo de 1,5 cm na primeira linha.
- [ ] Paginação contada a partir da folha de rosto; numerada a partir da introdução.
- [ ] Resumo de 150–500 palavras + palavras-chave.
- [ ] Abstract + keywords.
- [ ] Sumário com páginas.
- [ ] Figuras e tabelas com título, número e fonte.
- [ ] Citações conforme NBR 10520.
- [ ] Referências em ordem alfabética conforme NBR 6023.
- [ ] Desenvolvimento com 20–30 páginas.
- [ ] Trabalho inédito; sem plágio ou autoplágio.

### E. Exemplos de mensagens de commit

```text
adiciona contrato inicial da API
cria estrutura da solução .NET
modela banco de dados e migrations
implementa autenticação JWT
cria cadastro de produtores
implementa criação de pedidos
adiciona painel administrativo
cria aplicativo mobile com navegação
implementa login no aplicativo
adiciona fila de sincronização offline
documenta etapa 1 do PIM
revisa formatação ABNT do capítulo 6
```

### F. Glossário operacional

| Termo | Significado |
|---|---|
| **Parte A** | Núcleo corporativo e Web |
| **Parte B** | Aplicativo Mobile |
| **Trilha D** | Documentação, ABNT e evidências |
| **Contrato** | `contracts/openapi.yaml` + enums + erros |
| **Ponto de integração** | Momento em que as duas partes validam o contrato juntas |
| **Marco** | Entrega verificável com tag no git |
| **Evidência** | Artefato que comprova um critério do manual |

---

> **Resumo executivo:** a Parte A constrói o núcleo corporativo e o painel Web; a Parte B constrói o aplicativo Mobile; as duas avançam em paralelo com contrato primeiro e integração em quatro pontos. A trilha D transforma o que foi construído em PIM válido. O caminho crítico é: contrato → banco → autenticação → cadastros → pedidos → entregas → integração → documento. O projeto está pronto quando o fluxo completo funciona, o script SQL roda em banco limpo, as evidências estão coletadas e o PIM está em ABNT.
