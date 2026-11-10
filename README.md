# AgroConecta

Plataforma corporativa integrada para gestão da cadeia de suprimentos e inclusão digital na agricultura familiar. Projeto do **PIM IV** do curso de Análise e Desenvolvimento de Sistemas da UNIP.

## Stack

| Camada | Tecnologia |
|---|---|
| Web/API | .NET 10, ASP.NET Core MVC + Web API |
| Banco | SQL Server 2022 |
| ORM | EF Core 10 |
| Mobile | React Native + Expo + TypeScript |
| Offline | expo-sqlite |
| Testes | xUnit (backend) + Jest/RNTL (mobile) |
| Infra | Docker, GitHub Actions, Azure (plano) |

## Estrutura

```text
contracts/   Contrato OpenAPI, enums, erros e seed
db/          Scripts SQL, procedures e triggers
src/         Solução .NET (Domain, Application, Infrastructure, Web, Tests)
mobile/      Aplicativo Expo
docker/      Dockerfile e docker-compose
documento/   PIM final em ABNT
evidencias/  Capturas, vídeos e relatórios
gestao/      Backlog, sprints, Kanban e riscos
docs/        Manual do PIM e escopo de referência
```

## Pré-requisitos

- .NET SDK 10
- Node.js 20+ e npm
- Docker e Docker Compose
- Expo Go ou emulador Android/iOS

## Como rodar (a partir da fundação)

```bash
# 0. Ferramentas locais (EF Core)
dotnet tool restore

# 1. Banco
docker compose -f docker/docker-compose.yml up -d

# 2. Migrations e seed de desenvolvimento
dotnet ef database update -p src/AgroConecta.Infrastructure -s src/AgroConecta.Web

# 3. Web/API
dotnet run --project src/AgroConecta.Web

# 4. Mobile
cd mobile
npm install
npx expo start
```

A API expõe Swagger em `http://localhost:5000/swagger` e health em `http://localhost:5000/health`.

## Aplicativo mobile

O app (Expo + React Native) atende o **produtor** (registro de lotes) e o
**motorista** (entregas com foto e GPS), com operação offline via `expo-sqlite`.

```bash
cd mobile
npm install
npm run check     # lint + typecheck + testes
npm start         # Expo
```

- A URL da API vem de `EXPO_PUBLIC_API_URL` (padrão `http://localhost:5000/api/v1`).
  Em aparelho físico, use o IP da máquina ou `adb reverse tcp:5000 tcp:5000`.
- `EXPO_PUBLIC_API_MOCK=true` executa o app com o servidor fake em memória.
- Build do APK, integração real e limitações em `evidencias/mobile/` e `mobile/README.md`.

### Banco de dados

- SQL Server 2022 em container, banco `AgroConecta`, porta `1433`.
- Senha local do usuário `sa`: `AgroConecta@2026` (padrão de desenvolvimento, veja `docker/.env.example`).
- A string de conexão fica em `src/AgroConecta.Web/appsettings.json` e pode ser sobrescrita por `ConnectionStrings__Default`.
- Em `Development`, a aplicação aplica migrations e o seed automaticamente na inicialização.
- Alternativa manual (banco limpo): `db/script-completo.sql` cria tabelas, procedures, triggers e carga.

```bash
# recriar o script completo depois de alterar o modelo
./scripts/gerar-script-banco.sh
```

## Usuários de desenvolvimento

| E-mail | Papel | Senha |
|---|---|---|
| `admin@agroconecta.local` | Admin | `Admin@123` |
| `produtor@agroconecta.local` | Produtor | `Produtor@123` |
| `maria@agroconecta.local` | Produtor | `Produtor@123` |
| `comprador@agroconecta.local` | Comprador | `Comprador@123` |
| `motorista@agroconecta.local` | Motorista | `Motorista@123` |

## Contrato da API

O contrato fica em `contracts/openapi.yaml` e é a fonte de verdade para Web e Mobile.

```bash
# Validar
npx @redocly/cli lint contracts/openapi.yaml

# Mock para o Mobile
npx @stoplight/prism-cli mock contracts/openapi.yaml --port 4010

# Tipos TypeScript
npx openapi-typescript contracts/openapi.yaml -o mobile/src/types/api.d.ts
```

## Desenvolvimento paralelo

O projeto foi desenvolvido em duas frentes integradas em `main`:

- **Parte A:** `src/`, `db/`, `docker/`, `.github/`, `scripts/`
- **Parte B:** `mobile/`
- **Contrato e documentação:** `main`

Consulte `PLAN.md` para o cronograma, pacotes de trabalho, critérios de aceite e regras de integração.

## Documentação

- `docs/manual.md` — manual do PIM IV (fonte de verdade).
- `docs/rascunho.md` — escopo funcional v2.0.
- `PLAN.md` — plano de desenvolvimento.
- `evidencias/` — pacote de evidências das Partes A e B (Web, API, banco, DevOps, mobile, testes, inclusão e gestão).
- `docs/devops.md` e `docs/seguranca-lgpd.md` — execução, implantação e checklists.
