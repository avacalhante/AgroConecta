# Evidências — Inclusão, diversidade e responsabilidade social

## Inclusão digital na agricultura familiar

- Cadastro de produtores por comunidade: `Tradicional`, `Quilombola`,
  `Assentamento`, `Indígena` e `Outro`.
- Painel `/Inclusao` exibe produtores por comunidade, total cadastrado,
  aprovados e pendentes.
- Indicador `produtoresAtivos` e `telasAcessiveisPercentual` na API de indicadores.
- Seed de demonstração inclui produtora quilombola (“Maria Quilombola”,
  comunidade `Quilombola`), evidenciando a diversidade no domínio.
- Aprovação de cadastro por Admin garante curadoria e suporte ao produtor.

## Acessibilidade

- Diretrizes WCAG 2.1 AA aplicadas ao layout: idioma `pt-BR`, link
  “Pular para o conteúdo”, contraste, foco visível, rótulos e tabelas acessíveis.
- Checklist em `../PIM_4-web/docs/seguranca-lgpd.md`.

## LGPD e transparência

- Página pública `/Home/Privacidade` com dados tratados, finalidades, segurança,
  retenção e direitos do titular.
- Aviso de consentimento na tela de login.
- Trilha de auditoria (`/Usuarios/Auditoria`) para ações sensíveis.
- Minimização: apenas dados necessários à operação.
- Senhas armazenadas com PBKDF2; comunicação exigida por HTTPS.

## Responsabilidade social

- O produto conecta agricultores familiares a compradores institucionais,
  reduzindo intermediários e ampliando renda.
- Registro de evidência de entrega com foto e geolocalização dá transparência
  ao escoamento da produção.
- Relatórios de vendas por produtor apoiam políticas de crédito e fomento.
