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
├── components/  componentes reutilizáveis
├── db/          SQLite local (B6)
├── navigation/  stack raiz e abas principais (B1)
├── screens/     telas do produtor e do motorista
├── sync/        fila offline e sincronização (B6)
├── theme/       cores, espaçamentos e tipografia
├── types/       tipos gerados do contrato OpenAPI (B3)
└── utils/       formatadores e helpers
```

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
