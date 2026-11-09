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

Em dispositivo físico, use o IP da máquina na rede local (`http://192.168.0.10:5000/api/v1`). `localhost` no aparelho aponta para o próprio aparelho.

## Estrutura

```text
src/
├── api/         cliente HTTP e configuração (B3)
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
npx openapi-typescript ../contracts/openapi.yaml -o src/types/api.d.ts
```
