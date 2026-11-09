# AGENTS.md — mobile

Instruções específicas do aplicativo Expo. O `AGENTS.md` da raiz continua valendo.

## Expo muda com frequência

Não confie na memória para APIs do Expo, React Native ou EAS. Antes de codar:

1. Confira a versão do `expo` em `package.json` (hoje SDK 58).
2. Consulte a documentação versionada: `https://docs.expo.dev/versions/v58.0.0/`.
3. Para o restante, use `https://docs.expo.dev/llms.txt` como índice.

## Comandos

```bash
npm install                 # dependências
npx expo install <pacote>   # sempre para libs com código nativo (resolve versão do SDK)
npm start                   # servidor de desenvolvimento
npm run lint                # ESLint
npm run typecheck           # tsc --noEmit
npm test                    # Jest + RNTL
npm run check               # lint + typecheck + testes
npx expo-doctor             # diagnostica dependências e configuração
```

Não declare uma tarefa concluída sem `npm run check` verde.

## Navegação

Este projeto usa **React Navigation** (stack + bottom tabs), não Expo Router.

- `src/navigation/` — navegadores e tipos de rota.
- `src/screens/` — telas.
- Rotas são tipadas em `src/navigation/types.ts`.

## Regras

- `ios/` e `android/` são gerados; nunca edite à mão. Configure em `app.json` e plugins.
- Expo Go só traz os módulos nativos embutidos; ao adicionar módulo nativo, use development build (`npx expo run:android` ou `eas build`).
- Prefira módulos Expo (`expo-sqlite`, `expo-secure-store`, `expo-location`, `expo-image-picker`) a bibliotecas de terceiros.
- Não versione `.env`, tokens nem build (`dist/`, `web-build/`).
