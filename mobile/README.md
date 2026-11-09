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
├── db/          SQLite local: produtos, lotes, entregas e fila (B4/B6)
├── hooks/       consultas e sincronização usadas pelas telas
├── navigation/  pilha raiz, pilha autenticada e abas principais
├── screens/     telas do produtor e do motorista
├── sync/        conectividade, fila e sincronização (B4/B6)
├── theme/       tokens, tipografia e tema de status
├── types/       tipos de domínio e, na B3, tipos gerados do OpenAPI
└── utils/       formatadores e helpers
```

## Navegação

- `RootNavigator`: pilha raiz com `Login` e `Main` (autenticado).
- `MainStack`: abas principais mais as telas de detalhe:
  - `NovoLote` e `LoteDetalhe` (parâmetro `codigoRastreio`);
  - `EntregaDetalhe`, `EntregaStatus` e `EntregaEvidencia` (parâmetro `entregaId`).
- `MainTabs`: Início, Perfil e as abas do papel autenticado — Lotes para produtor/admin,
  Entregas para motorista/admin. As telas de aba usam `MainTabScreenProps`, que combina a
  navegação das abas com a pilha principal.

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
serviços de recurso e o 401 é tratado por `executarAutenticado`. O retry com backoff é
feito pela fila de sincronização, descrita abaixo.

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

## Fluxo do produtor e operação offline

- `NovoLote` valida produto, data (`AAAA-MM-DD`, não futura) e quantidade, gera o
  `codigoRastreio` com `expo-crypto` (`randomUUID`) e grava primeiro no SQLite.
- Cada lote cria uma operação `criar_lote` na fila persistente; o envio não bloqueia a tela.
- `sincronizarLotes` envia a fila, trata `409` como já sincronizado e depois puxa
  `/lotes/meus` e `/produtos` para atualizar o banco local.
- A lista lê sempre do SQLite e mostra “Pendente de envio” enquanto o lote não sobe.
- A sincronização roda ao focar a aba de lotes, no gesto de puxar para atualizar e pelo
  coordenador global de sincronização.
- O catálogo de produtos do seed é gravado na primeira abertura, permitindo criar lote
  sem nenhuma chamada de rede.

## Fluxo do motorista

- A lista lê as entregas do SQLite; cada alteração de status é aplicada localmente antes
  de entrar na fila (`atualizar_status_entrega`).
- As transições seguem `contracts/enums.md` (`Pendente → EmRota → Entregue/Falha`,
  `Pendente → Cancelada`); repetir o status atual não enfileira nada.
- `sincronizarEntregas` envia os PATCHs na ordem da fila, atualiza a entrega com a resposta
  do servidor e depois puxa `/entregas/minhas`, sem sobrescrever alterações pendentes.
- Falhas de rede mantêm a operação na fila e o detalhe da entrega mostra o aviso.
- A sincronização roda ao focar a aba de entregas, no gesto de puxar para atualizar e pelo
  coordenador global de sincronização.
- A tela de evidência já recebe a entrega real; câmera, localização e upload multipart
  entram na B7.

## Sincronização, reconexão e modo avião

`SincronizacaoProvider` fica acima da navegação e coordena a fila quando há usuário
autenticado:

- sincroniza ao restaurar a sessão, quando o NetInfo indica reconexão, quando o app volta
  ao primeiro plano e a cada 30 s se houver operação pronta para envio;
- operações com falha temporária (`5xx`, rede, `408`, `429`, `401` após refresh) ficam na
  fila com backoff exponencial de 1 s, 2 s, 4 s, 8 s e 16 s;
- após 5 tentativas a operação sai da fila e o conflito fica visível na entidade;
- erros definitivos (`400`, `403`, `404`, `422`) não são repetidos: o lote mantém o erro
  local e a entrega volta ao estado do servidor na próxima leitura, preservando o aviso;
- `409` ao criar lote continua sendo tratado como sucesso;
- a aba Perfil mostra conexão, operações na fila, conflitos, última sincronização e o
  botão “Sincronizar agora”.

A fila fica na tabela `fila_sincronizacao` do SQLite e cada operação guarda payload,
contador de tentativas, último erro e próxima tentativa. Fechar o app não perde a fila.

### Teste manual de modo avião

1. Entre como produtor e ative o modo avião do aparelho.
2. Crie um lote; ele deve aparecer na lista como “Pendente de envio”.
3. Feche o app pelo seletor de tarefas e abra de novo: o lote e a pendência continuam.
4. Desative o modo avião e volte ao app; o coordenador sincroniza ao reconectar.
5. Confirme que o lote some de “Pendente de envio” e o detalhe mostra que está no servidor.
6. Repita com uma entrega: altere o status offline, feche e reabra o app, reconecte e
   confira o status confirmado pelo servidor.

Para observar o backoff, deixe a API indisponível: a operação permanece na fila e as
tentativas ficam espaçadas. Ao retornar, a próxima tentativa envia a operação. Um erro
4xx definitivo aparece no lote ou no detalhe da entrega e não é reenviado.

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
