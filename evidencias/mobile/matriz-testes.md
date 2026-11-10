# Matriz de testes do aplicativo mobile — AgroConecta

- **Aplicativo:** AgroConecta Mobile 0.1.0 (Expo SDK 58, React Native 0.88.1)
- **Plataforma dos testes manuais:** Android 13 em Xiaomi 2210129SG, com Expo Go 58.0.5 e API mock (`EXPO_PUBLIC_API_MOCK=true`)
- **Data da execução:** 09/11/2026
- **Escopo:** autenticação, lotes offline, sincronização, entregas, evidência com câmera/galeria e GPS, acessibilidade visual e verificações de build
- **Automação:** Jest 30 e React Native Testing Library 14 (renderização e eventos assíncronos)

## Verificações automatizadas

| Verificação | Comando | Resultado |
|---|---|---|
| ESLint | `npm run lint` | Sem erros |
| TypeScript | `npm run typecheck` | Sem erros |
| Jest/RNTL | `npm test` | 17 suítes, 79 testes passando |
| Bundle Android | `npx expo export -p android` | Bundle gerado em `/tmp/opencode/expo-export-b9` |

### Casos automatizados por suíte

| Suíte | Casos cobertos |
|---|---|
| `src/components/Button.test.tsx` | Rótulo acessível, estado de carregamento, bloqueio de toque e variações visuais |
| `src/components/Field.test.tsx` | Rótulo, valor, mensagem de erro, foco e acessibilidade do campo |
| `src/components/StatusBadge.test.tsx` | Rótulo textual do status e semântica de acessibilidade |
| `src/components/Card.test.tsx` | Título como cabeçalho e conteúdo filho renderizado |
| `src/components/InfoRow.test.tsx` | Rótulo e valor agrupados em uma única frase acessível |
| `src/screens/LoginScreen.test.tsx` | Validação, erro de autenticação, carregamento e login bem-sucedido |
| `src/auth/AuthContext.test.tsx` | Restauração de sessão, login, logout e refresh autenticado |
| `src/api/http.test.ts` | Construção de requisições, timeout, cancelamento e erros RFC 7807 |
| `src/api/mock/servidorFake.test.ts` | Contrato do servidor mock para login, lotes, entregas e evidências |
| `src/api/entregas.test.ts` | Upload multipart nativo da evidência, erros RFC 7807 e exigência da foto |
| `src/sync/lotes.test.ts` | Fila offline, conflito 409, erros definitivos e retentativas |
| `src/sync/entregas.test.ts` | Atualização otimista, fila de status e sincronização |
| `src/sync/evidencias.test.ts` | Fila de evidência, upload multipart, sucesso, falha e limpeza do arquivo |
| `src/utils/backoff.test.ts` | Atraso exponencial limitado a cinco minutos e máximo de tentativas |
| `src/utils/validacaoLote.test.ts` | Campos obrigatórios, pesos e mensagens em português |
| `src/utils/entregas.test.ts` | Rótulos, datas e transições de status |
| `src/utils/format.test.ts` | Formatação de datas, pesos e números no padrão brasileiro |

## Testes manuais no aparelho

| ID | Fluxo | Passos executados | Resultado |
|---|---|---|---|
| M1 | Login de produtor | Informar credenciais do seed, entrar e abrir a tela Início | Aprovado |
| M2 | Lote offline persistente | Ativar modo avião, criar lote de 100 kg de tomate, forçar a parada do app e reabrir | Aprovado: o lote permaneceu listado como pendente |
| M3 | Sincronização após reconexão | Desativar modo avião e reabrir o app | Aprovado: o lote pendente foi sincronizado |
| M4 | Login de motorista | Sair, entrar com o motorista do seed e abrir Entregas | Aprovado |
| M5 | Alteração de status | Abrir o Pedido 1 e alterar o status para Entregue | Aprovado: badge e detalhe atualizados |
| M6 | Permissão de localização | Abrir Registrar evidência e tocar em Obter localização; aceitar as permissões do Expo Go | Aprovado: latitude e longitude exibidas |
| M7 | Galeria e compressão | Escolher uma imagem da galeria do aparelho | Aprovado: prévia exibida com o aviso de redução para 1280 px e JPEG |
| M8 | Envio online da evidência | Com rede, tocar em Enviar evidência | Aprovado: mensagem “Evidência enviada ao servidor.” |
| M9 | Envio offline da evidência | Ativar modo avião, selecionar foto e tocar em Enviar evidência | Aprovado: mensagem “Sem conexão. A evidência ficou salva no aparelho e será enviada ao reconectar.” |
| M10 | Reconexão da evidência | Desativar modo avião, reabrir o app e voltar ao detalhe do Pedido 1 | Aprovado: o aviso de evidência pendente desapareceu após a sincronização |
| M11 | Fonte ampliada | Ajustar a fonte do sistema para 200% e percorrer Início, Lotes, Entregas e Perfil | Aprovado após ajuste da altura da barra de abas; rótulos visíveis e botões acionáveis |
| M12 | Câmera | Tocar em Tirar foto | Não executado nesta rodada; a câmera foi concedida, mas o fluxo foi validado pela galeria |
| M13 | Leitor de tela | Percorrer o roteiro do TalkBack | Bloqueado: o TalkBack não está instalado no aparelho de teste |
| M14 | Teclado externo | Navegar pelos campos com teclado físico | Não executado: sem teclado disponível |

## Limitações registradas

- O TalkBack não está instalado no aparelho e não foi possível validar a leitura de nomes, funções e ordem de foco. O roteiro está detalhado em `acessibilidade.md`.
- A câmera foi autorizada, mas a captura foi validada apenas pela galeria; o mesmo caminho de compressão e upload é usado nos dois casos.
- A matriz principal usou a API mock; a integração com o backend real e o APK estão em `integracao.md`.
- O aviso de evidência pendente é atualizado ao reabrir o app; a tela aberta não é atualizada em tempo real após a reconexão.
