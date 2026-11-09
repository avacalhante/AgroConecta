# Segurança, LGPD e acessibilidade

Documento de apoio ao critério de avaliação de segurança e qualidade do PIM IV.
Reflete o que está implementado no código da Parte A (Web/API/banco).

## 1. Checklist OWASP Top 10 (2021)

| Risco | Controle implementado | Onde |
|---|---|---|
| A01 — Quebra de controle de acesso | Políticas por papel (`SomenteAdmin`, `GerenciarCatalogo`, `Comprar`, `OperarPedidos`, `Entregar`); escopo de pedidos por comprador; 404 para recursos de terceiros | `Program.cs`, controllers da API e MVC |
| A02 — Falhas criptográficas | HTTPS/HSTS; senhas com PBKDF2 (`PasswordHasher`); JWT HMAC-SHA256 com chave de 32+ bytes; refresh token separado por `token_type` | `Seguranca/`, `appsettings.json` |
| A03 — Injeção | EF Core com consultas parametrizadas; validação com FluentValidation; sem SQL concatenado | `Infrastructure/Repositorios`, `Application/*Validator` |
| A04 — Design inseguro | Transições de status validadas no domínio; aprovação de produtor restrita a Admin; evidência exige foto e coordenadas válidas | `Domain/Entities`, `Application/Entregas` |
| A05 — Configuração incorreta | Cabeçalhos de segurança; Swagger apenas em desenvolvimento; segredos fora do código (appsettings/ambiente); CORS restrito | `Program.cs` |
| A06 — Componentes vulneráveis | Pacotes fixados em versões atuais (.NET 10, EF Core 10, JwtBearer 10) | `*.csproj` |
| A07 — Falhas de autenticação | Rate limit por IP no login; mensagens de erro genéricas; bloqueio de usuário inativo | `Program.cs`, `AutenticarCasoDeUso` |
| A08 — Falhas de integridade | Antiforgery em formulários MVC; `[ValidateAntiForgeryToken]` nas ações POST | `AccountController`, demais controllers MVC |
| A09 — Falhas de log/monitoramento | Trilha de auditoria (`LogAcesso`); `ILogger` com `traceId` nas respostas ProblemDetails | `ApiExceptionHandler`, `Usuarios/Auditoria` |
| A10 — SSRF | Nenhuma requisição de saída a partir de entrada do usuário; armazenamento local com nome gerado pelo servidor | `ArmazenamentoEvidencia` |

### Cabeçalhos HTTP aplicados

- `X-Content-Type-Options: nosniff`
- `X-Frame-Options: DENY`
- `Referrer-Policy: no-referrer`
- `Permissions-Policy: geolocation=(), microphone=(), camera=()`
- `Content-Security-Policy` restritivo (exceto na UI do Swagger em desenvolvimento)
- `Strict-Transport-Security` em ambientes não-desenvolvimento

### Proteção contra força bruta

- Política de rate limit `login`: 10 tentativas por minuto por IP em produção.
- Aplicada em `POST /api/v1/auth/login` e `POST /Account/Login`.
- Nos testes automatizados o limite é ampliado para não interferir na suíte.

## 2. LGPD (Lei 13.709/2018)

| Princípio | Medida |
|---|---|
| Finalidade e minimização | Apenas dados necessários a pedidos, entregas, faturamento e inclusão |
| Segurança | Hash de senha, HTTPS, autorização por papel, auditoria |
| Transparência | Página `/Home/Privacidade` e aviso de consentimento no login |
| Direitos do titular | Canal `privacidade@agroconecta.local`; exclusão/correção sob solicitação |
| Prestação de contas | Trilha de auditoria consultável em `/Usuarios/Auditoria` |
| Retenção | Durante a relação comercial; dados fictícios no ambiente acadêmico |

## 3. Acessibilidade (WCAG 2.1 AA)

| Critério | Implementação |
|---|---|
| 1.3.1 — Informação e relações | Tabelas com `<caption>`, `<thead>` e `scope="col"`; formulários com `<label>` associado |
| 1.4.3 — Contraste | Paleta Bootstrap com contraste adequado (verde escuro/branco) |
| 2.1.1 — Teclado | Navegação completa por teclado; foco visível |
| 2.4.1 — Evitar blocos | Link “Pular para o conteúdo” no topo |
| 2.4.2 — Título da página | `<title>` preenchido em todas as telas |
| 3.3.1/3.3.2 — Erros e rótulos | Mensagens de validação em texto; rótulos explícitos |
| 4.1.2 — Nome, função e valor | Controles nativos com `aria-label`/`role` quando necessário |

Medida acompanhada no painel `/Inclusao` (indicador `TelasAcessiveisPercentual`).

## 4. Pendências e riscos residuais

- Chave JWT de desenvolvimento deve ser trocada por segredo de ambiente em produção.
- Swagger deve permanecer desabilitado em produção.
- Uploads de evidência precisam de storage externo (blob) em ambiente produtivo.
- Teste de carga do rate limit deve ser executado em ambiente real.
