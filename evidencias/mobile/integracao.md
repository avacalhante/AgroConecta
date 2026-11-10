# Integração e build do aplicativo mobile — AgroConecta

- **Data:** 09/11/2026
- **Aplicativo:** AgroConecta Mobile 0.1.0 (Expo SDK 58, React Native 0.88.1)
- **Aparelho:** Xiaomi 2210129SG, Android 13, conectado por USB/ADB
- **Backend:** ASP.NET Core/.NET 10 local na porta 5000, com SQL Server 2022 em container
- **API:** `http://127.0.0.1:5000/api/v1` via `adb reverse tcp:5000 tcp:5000`

## Resultado da integração real

| Verificação | Resultado |
|---|---|
| `GET /health` | `200`, versão `1.0.0` |
| Login do produtor do seed | Aprovado no app |
| Lote criado pelo app | `409aa954-e28f-4ab2-8263-731bc9baf79a`, tomate, 25 kg, persistido no SQL Server |
| Login do motorista do seed | Aprovado no app |
| Entrega atualizada para `Entregue` | Persistida no SQL Server com `DataRealizada` |
| Evidência com foto e GPS | JPEG de 73.918 bytes em `/uploads/entregas/e460d3626f724a0881b0f208396f589d.jpg`, latitude `-23.6517824` e longitude `-46.6118979` |
| APK release instalado | Aprovado com `adb install -r` |
| App standalone sem Metro | Aprovado: login e carga de dados com a API real |

O APK de release foi gerado com o bundle JavaScript embutido, instalado no aparelho e aberto
com o Metro parado. O login do produtor e a tela Início carregaram dados da API real.

## Correções feitas durante a integração

1. **Sincronização por papel:** o produtor chamava `/entregas/minhas` e recebia `403`,
   contabilizando uma falha falsa a cada sincronização. O coordenador agora só sincroniza
   lotes para produtor/admin e entregas/evidências para motorista/admin.
2. **Upload de evidência no Android:** o `fetch` com `FormData` falhava antes de chegar ao
   servidor e esgotava as cinco tentativas. O envio passou a usar
   `FileSystem.uploadAsync` com multipart nativo, mantendo o contrato `PUT` e o tratamento
   de erros RFC 7807.
3. **HTTP no APK de release:** o Android bloqueia cleartext em release. O config plugin
   `plugins/withCleartextTraffic.js` libera HTTP somente quando `EXPO_PUBLIC_API_URL`
   começa com `http://`; builds com HTTPS mantêm o bloqueio padrão.

## Build do APK

```bash
export ANDROID_HOME="$HOME/Android/Sdk"
export JAVA_HOME="$HOME/.gradle/jdks/eclipse_adoptium-21-amd64-linux.2"
cd mobile
npx expo prebuild --platform android
cd android
./gradlew assembleRelease -PreactNativeArchitectures=arm64-v8a --no-daemon --max-workers=2
```

- **Artefato:** `android/app/build/outputs/apk/release/app-release.apk`
- **Tamanho:** 27 MB (arm64-v8a)
- **SHA-256:** `1c4cf04acbc06d6e1feb4ac533df6f79bb963193f857bdf0efc8c86004adc75c`
- **Assinatura:** chave de debug do template; serve para demonstração, não para loja.
- **Ferramentas instaladas:** Gradle 9.4.1, NDK 27.1.12297006 e CMake 3.22.1.

O APK não é versionado porque é um artefato de build. O hash acima identifica a execução
validada. Para publicar, configurar uma keystore de release e usar o perfil `preview` ou
`production` do `eas.json`, com `EXPO_PUBLIC_API_URL` HTTPS.

## Limitações

- O APK gerado contém apenas `arm64-v8a`, suficiente para o aparelho de teste; um build
  universal precisa incluir as demais ABIs.
- A URL `127.0.0.1:5000` funcionou por `adb reverse` (USB). Em rede local, o build deve usar
  o IP da máquina, por exemplo `http://192.168.15.16:5000/api/v1`, e liberar a porta 5000
  no firewall do host.
- O APK usa a chave de debug e HTTP local; não deve ser distribuído como versão de produção.
- O EAS cloud não foi executado por falta de credenciais da conta Expo; os perfis estão
  configurados em `mobile/eas.json`.
