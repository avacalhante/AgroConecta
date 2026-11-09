# AgroConecta Mobile

Aplicativo Expo + React Native + TypeScript do AgroConecta, usado pelo **produtor** (registro de lotes) e pelo **motorista** (entregas com evidência), com operação offline.

Parte B do plano em `../PLAN.md`. O contrato da API está em `../contracts/`.

## Requisitos

- Node.js 20+
- npm
- Expo Go no aparelho ou emulador Android/iOS

## Comandos

```bash
npm install          # dependências
npm start            # servidor Expo
npm run android      # emulador/dispositivo Android
npm run ios          # simulador iOS (macOS)
npm test             # Jest + React Native Testing Library
npm run lint         # ESLint (expo lint)
npm run typecheck    # tsc --noEmit
npm run check        # lint + typecheck + testes
```

## Configuração da API

```bash
cp .env.example .env
```

| Variável | Uso |
|---|---|
| `EXPO_PUBLIC_API_URL` | URL base da API, ex.: `http://localhost:5000/api/v1` |
| `EXPO_PUBLIC_API_MOCK` | `true` usa o servidor fake em memória, sem API |

Em dispositivo físico, use o IP da máquina na rede local (`http://192.168.0.10:5000/api/v1`). `localhost` no aparelho aponta para o próprio aparelho.

## Estrutura

```text
src/
├── api/         cliente HTTP, endpoints e configuração (B3)
├── auth/        sessão segura e AuthContext (B2)
├── components/  design system: botão, campo, cartão, badge, estados
├── db/          SQLite local (B6)
├── mock/        fixtures das telas de demonstração
├── navigation/  pilha raiz, pilha autenticada e abas principais
├── screens/     telas do produtor e do motorista
├── sync/        fila offline e sincronização (B6)
├── theme/       tokens, tipografia e tema de status
├── types/       tipos de domínio e, na B3, tipos gerados do OpenAPI
└── utils/       formatadores e helpers
```

## Navegação

- `RootNavigator`: pilha raiz com `Login` e `Main` (autenticado).
- `MainStack`: abas principais mais as telas de detalhe:
  - `LoteDetalhe` (parâmetro `loteId`);
  - `EntregaDetalhe`, `EntregaStatus` e `EntregaEvidencia` (parâmetro `entregaId`).
- `MainTabs`: Início, Lotes, Entregas e Perfil. As telas de aba usam `MainTabScreenProps`,
  que combina a navegação das abas com a pilha principal.

## Autenticação

- Login em `POST /auth/login` com e-mail e senha do seed.
- A sessão (`accessToken`, `refreshToken`, validade e usuário) fica no `expo-secure-store`,
  na chave `agroconecta.sessao`.
- Ao abrir o app, a sessão é restaurada; se o access token estiver vencido, o refresh é
  tentado automaticamente. Enquanto houver sessão, um timer renova o token em 75% da validade.
- `401` no refresh encerra a sessão local e volta ao login. Falha de rede ou `5xx` mantém a
  sessão para permitir operação offline.
- `executarAutenticado` repete uma operação uma vez após renovar o token em caso de `401`.
- O logout chama `POST /auth/logout` (melhor esforço) e sempre limpa a sessão local.

O cliente HTTP usa `fetch` estruturado em `src/api/http.ts`, com `AbortController` para
_timeout_ e normalização de erros RFC 7807 em `ApiError`. O token é injetado pelos
serviços de recurso e o 401 é tratado por `executarAutenticado`; retry com backoff entra
na tarefa B6.

## Cliente de API e mock

- `src/api/http.ts`: requisição JSON ou multipart, timeout e `ApiError` com status e
  `errors` por campo.
- `src/api/lotes.ts`, `entregas.ts`, `produtos.ts` e `indicadores.ts`: serviços tipados,
  sempre recebendo o access token.
- `src/api/mock/servidorFake.ts`: usuários do seed, lotes, entregas, produtos e indicadores
  em memória. Ative com `EXPO_PUBLIC_API_MOCK=true` para desenvolver sem a API.
- Erros seguem `contracts/errors.md`: `400` mantém mensagens por campo; `401` renova ou
  encerra a sessão; `403`/`422` não são repetidos; `409` ao criar lote é sucesso;
  `5xx`/rede ficam como `ApiError` com `status` 0.

## Design system

`src/components/` concentra `Button`, `Field`, `Card`, `StatusBadge`, `EmptyState`,
`LoadingState`, `ErrorState` e `InfoRow`. O tema fica em `src/theme/`:

- `tokens.ts`: cores semânticas, espaçamento, raio, fonte, pesos, alturas de linha e estados;
- `typography.ts`: estilos de texto;
- `status.ts`: rótulo em pt-BR e tom visual de cada status do contrato.

Os componentes mantêm alvo de toque mínimo de 48 px, `accessibilityLabel` explícito e
não fixam altura de texto, para funcionar com fonte ampliada.

## Convenções

- Navegação com **React Navigation** (stack + bottom tabs), não Expo Router.
- Nada de segredo no app; tokens ficam no `expo-secure-store`.
- Toda chamada de rede passa pelo cliente de `src/api/`.
- Operações offline são idempotentes: `codigoRastreio` gerado no cliente, `PATCH` de status e `PUT` de evidência.
- Textos, rótulos e mensagens em português do Brasil.
- Acessibilidade: alvos de toque de 48 px, rótulos explícitos e suporte a fonte ampliada.

## Usuários de desenvolvimento

Ver `../contracts/seed.md`. Exemplo: `produtor@agroconecta.local` / `Produtor@123`.

## Tipos da API

```bash
npm run api:types
```

O comando gera `src/types/api.d.ts` a partir de `../contracts/openapi.yaml`. Esse arquivo é
somente referência do contrato: como o OpenAPI não marca propriedades como obrigatórias,
os tipos usados em runtime ficam em `src/api/tipos.ts`, com os campos que o app espera.

`openapi-typescript` 7.13 declara peer `TypeScript ^5` e o projeto usa TypeScript 6; por isso
o script usa `npx` com versão fixada em vez de uma dependência de desenvolvimento.
