# Acessibilidade do aplicativo mobile — AgroConecta

- **Aplicativo:** AgroConecta Mobile 0.1.0 (Expo SDK 58, React Native 0.88.1)
- **Plataforma avaliada:** Android; estrutura equivalente no iOS
- **Data da inspeção:** 09/11/2026
- **Escopo:** login, início, lotes, novo lote, detalhe do lote, entregas, detalhe da entrega, status, evidência e perfil/sincronização
- **Meta:** WCAG 2.1 nível AA nos critérios aplicáveis a um aplicativo nativo, com textos em português do Brasil

## Método

1. Inspeção estática dos componentes e das telas (`src/components`, `src/screens`).
2. Cálculo dos contrastes do tema (`src/theme/tokens.ts` e `src/theme/status.ts`) pela fórmula de luminância relativa da WCAG.
3. Execução de `npm run check` (ESLint, TypeScript e Jest).
4. Roteiro manual com TalkBack/VoiceOver e fonte ampliada, documentado abaixo.

> **Limitação desta execução:** um aparelho Android 13 (Xiaomi 2210129SG) foi conectado via ADB, mas o Expo Go 58.0.5 não pôde ser instalado: o aparelho tinha 838 MB livres e o APK tem 215 MB; o instalador retornou `INSTALL_FAILED_INSUFFICIENT_STORAGE`. Não foram apagados arquivos pessoais para liberar espaço. O ambiente também não tem Android SDK/emulador para gerar um APK local. Assim, o teste com leitor de tela e a validação visual em 200% ficam registrados como roteiro pendente de execução em aparelho, não como resultado obtido. A inspeção de código e os cálculos de contraste foram concluídos.

## Checklist WCAG 2.1 AA

| Critério | Verificação no app | Evidência | Situação |
|---|---|---|---|
| 1.1.1 Conteúdo não textual | A prévia da foto tem `accessibilityLabel`; ícones e indicadores de carregamento são ocultados do leitor; status têm texto, não apenas cor | `EntregaEvidenciaScreen`, `Button`, `StatusBadge` | Atendido por inspeção |
| 1.3.1 Informação e relações | Títulos de tela e de cartão usam `accessibilityRole="header"`; `InfoRow` agrupa rótulo e valor em uma frase; campos têm rótulo visível e `accessibilityLabel` | `Card`, `Field`, `InfoRow`, telas | Atendido por inspeção |
| 1.3.2 Sequência significativa | A ordem do código segue a ordem visual: cabeçalho, conteúdo, avisos, ações | Telas de lote, entrega, status e evidência | Atendido por inspeção |
| 1.4.1 Uso da cor | Status sempre acompanhados de rótulo textual; erros e pendências têm texto e `accessibilityRole="alert"`/`liveRegion` | `StatusBadge`, `EntregaDetalheScreen`, `LotesScreen` | Atendido por inspeção |
| 1.4.3 Contraste (texto) | Todos os pares de texto do tema passam em AA; ver tabela abaixo | Cálculo de contraste | Atendido |
| 1.4.4 Redimensionar texto | Nenhum `allowFontScaling={false}` ou `maxFontSizeMultiplier`; botões usam altura mínima, não altura fixa; botões da foto usam `flexWrap` | Componentes e `EntregaEvidenciaScreen` | Pendente de validação em 200% no aparelho |
| 1.4.10 Reflow | Telas usam `Screen scrollable`; grades usam `flexWrap`; conteúdo longo rola verticalmente | `Screen`, `InicioScreen` | Atendido por inspeção |
| 1.4.11 Contraste de componentes | Borda de campo e de opção de rádio usa `borderControl` (`#6F7B6D`), 4,44:1 sobre branco e 4,12:1 sobre o fundo; marcador não selecionado usa a mesma cor | `Field`, `NovoLoteScreen`, `EntregaStatusScreen` | Atendido |
| 1.4.12 Espaçamento de texto | Textos usam estilos sem altura fixa de container; suporta quebra de linha | `typography.ts`, `Card`, `InfoRow` | Pendente de validação visual no aparelho |
| 2.1.1 Teclado | Fluxo principal é por toque; não há dependência de gesto secreto. Teclado externo não foi testado | Componentes `Pressable` nativos | Pendente de teste com teclado externo |
| 2.2.1 Tempo ajustável | Nenhuma ação expira por tempo; o refresh de sessão é automático e não interrompe o preenchimento | `AuthContext` | Atendido por inspeção |
| 2.3.1/2.3.2 Flash e movimento | Não há animação piscante nem movimento automático | Telas e componentes | Atendido por inspeção |
| 2.4.2 Título da tela | Cada tela tem título textual; títulos principais recebem papel de cabeçalho | Telas em `src/screens` | Pendente de confirmação da leitura pelo TalkBack |
| 2.4.3 Ordem de foco | Ordem natural de leitura; sem `accessibilityViewIsModal` ou armadilha de foco | Navegação e telas | Pendente de confirmação no leitor |
| 2.4.6 Cabeçalhos e rótulos | Títulos de cartão e de tela são cabeçalhos; botões usam o próprio texto como rótulo; campos têm rótulo | `Card`, `Button`, `Field` | Atendido por inspeção |
| 2.5.5 Tamanho do alvo | Botões e opções têm `minHeight` de 48 px e `minWidth` de 48 px quando aplicável | `Button`, `Field`, opções de status/produto | Atendido por inspeção |
| 3.2.2 Ao receber entrada | Campos não disparam navegação inesperada; seleção de produto/status é explícita | `NovoLoteScreen`, `EntregaStatusScreen` | Atendido por inspeção |
| 3.3.1 Identificação de erro | Erros de formulário aparecem junto ao campo e no resumo, com texto em português e `liveRegion` | `NovoLoteScreen`, `LoginScreen` | Atendido por inspeção |
| 3.3.3 Sugestão de correção | Mensagens orientam a correção (“informe…”, “use a galeria”, “abra os ajustes”) | Validações e tela de evidência | Atendido por inspeção |
| 4.1.2 Nome, função e valor | Botões têm `accessibilityRole="button"`; opções têm `radio` e `checked`; badges têm rótulo “Status: …” | `Button`, `StatusBadge`, telas | Atendido por inspeção |
| 4.1.3 Mensagens de status | Avisos de sincronização, erros e confirmações usam `accessibilityLiveRegion="polite"` ou `accessibilityRole="alert"` | Perfil, lotes, entregas, evidência | Atendido por inspeção |

## Contraste calculado

Fórmula WCAG 2.1 (luminância relativa). Para texto normal o mínimo é 4,5:1; para texto grande, 3:1.

| Primeiro plano | Fundo | Contraste | Uso |
|---|---|---:|---|
| `#1B1F1B` texto | `#F5F7F4` fundo | 15,49:1 | Texto principal |
| `#4A524A` texto secundário | `#F5F7F4` fundo | 7,51:1 | Legendas e textos de apoio |
| `#4A524A` texto secundário | `#FFFFFF` cartão | 8,09:1 | Legendas em cartões |
| `#4A524A` texto secundário | `#EEF2EC` superfície suave | 7,14:1 | Texto em estados neutros |
| `#FFFFFF` texto inverso | `#1F6A3A` primário | 6,60:1 | Botão primário |
| `#FFFFFF` texto inverso | `#B3261E` perigo | 6,54:1 | Botão de sair |
| `#154C29` primário escuro | `#FFFFFF` cartão | 10,01:1 | Títulos e botão secundário |
| `#8A5A12` aviso | `#F6E7C8` aviso claro | 4,84:1 | Badge de status de aviso |
| `#1E6B3A` sucesso | `#DDF0E3` sucesso claro | 5,48:1 | Badge de status de sucesso |
| `#1B4F8A` informação | `#DCE9F7` informação clara | 6,73:1 | Badge de status de informação |
| `#B3261E` perigo | `#F9DEDC` perigo claro | 5,14:1 | Badge de status de falha |
| `#6F7B6D` borda de controle | `#FFFFFF` cartão | 4,44:1 | Borda de campo e opção |
| `#6F7B6D` borda de controle | `#F5F7F4` fundo | 4,12:1 | Borda de opção sobre o fundo |

Observação: o estado desabilitado usa opacidade de 55%, mas componentes desabilitados são isentos do critério 1.4.3; o texto habilitado permanece com os contrastes acima.

## Roteiro de teste com leitor de tela

Executar em um aparelho Android com TalkBack (ou iPhone com VoiceOver) e registrar o resultado na tabela abaixo.

1. Ativar o TalkBack em **Configurações → Acessibilidade → TalkBack**.
2. Abrir o app e percorrer a tela de login: confirmar a leitura de “AgroConecta”, dos rótulos “E-mail” e “Senha”, do botão “Entrar” e da mensagem de erro ao autenticar com dados inválidos.
3. Entrar como produtor. Na tela Início, confirmar a leitura dos indicadores e das ações rápidas; abrir **Meus lotes**.
4. Criar um lote com o modo avião ativado. Confirmar que o leitor anuncia “Pendente de envio” e que o lote permanece após fechar e reabrir o app.
5. Desativar o modo avião e confirmar a mudança para lote sincronizado.
6. Entrar como motorista. Abrir uma entrega, alterar o status e confirmar a leitura do badge e do aviso de sincronização.
7. Em **Registrar evidência**, testar:
   - negar a permissão da câmera e confirmar a orientação para usar a galeria;
   - conceder a permissão da câmera ou escolher uma foto da galeria;
   - negar a localização e confirmar a mensagem e o botão “Abrir ajustes”;
   - conceder a localização, tirar a foto e enviar;
   - confirmar a leitura de “Evidência enviada ao servidor” ou do aviso offline.
8. Aumentar a fonte do sistema para o máximo (**Configurações → Tela → Tamanho da fonte**) e repetir os fluxos principais, verificando se textos quebram linha sem serem cortados e se os botões continuam acionáveis.
9. Ativar “Remover animações” e confirmar que nenhuma informação depende de animação.

### Registro do teste manual

| Data | Aparelho / Android | Leitor | Resultado | Observações |
|---|---|---|---|---|
| 09/11/2026 | Xiaomi 2210129SG / Android 13 | TalkBack | Bloqueado antes do teste | Expo Go 58.0.5 não instalou: 838 MB livres e `INSTALL_FAILED_INSUFFICIENT_STORAGE`; nenhum arquivo pessoal foi removido |
| A preencher | A preencher | TalkBack | A preencher | Liberar espaço no aparelho e executar o roteiro acima |
| A preencher | A preencher | VoiceOver | A preencher | Executar em iPhone, se disponível |

## Verificações automatizadas

```text
npm run check
```

- ESLint e TypeScript sem erros.
- 11 suítes Jest e 64 testes passando na data desta inspeção.
- Não existe teste automatizado de leitor de tela; a validação depende de aparelho/emulador.

## Lacunas conhecidas e próximos ajustes

- Liberar espaço no aparelho de teste (ou usar um emulador) e executar o roteiro com TalkBack; anexar capturas/vídeo em `evidencias/mobile/`.
- Repetir o roteiro com VoiceOver em um iPhone, se disponível.
- Validar fonte em 200% e 300% em aparelho real; ajustar rótulos longos se houver corte.
- Avaliar indicador de foco visível para teclado externo, caso esse uso seja requisito do produto.
- A prévia de foto remota usa um rótulo genérico; se a imagem tiver descrição específica no contrato, incluir o texto.
- Repetir o cálculo de contraste sempre que os tokens do tema mudarem.
