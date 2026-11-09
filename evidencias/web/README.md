# Evidências — Solução Web

Aplicação ASP.NET Core MVC com autenticação por cookie e autorização por papéis.

## Telas implementadas

| # | Tela | Rota | Acesso | Recursos |
|---|---|---|---|---|
| 1 | Login | `/Account/Login` | Anônimo | Antiforgery, rate limit, aviso de LGPD |
| 2 | Painel administrativo | `/` | Autenticado | Indicadores do dashboard |
| 3 | Produtores | `/Produtores` | Admin (aprovação) | Lista, aprovação de cadastro |
| 4 | Cooperativas | `/Cooperativas` | Autenticado; criar = Admin | Lista e cadastro |
| 5 | Produtos | `/ProdutosWeb` | Autenticado; criar = Admin/Produtor | Catálogo, filtro por situação |
| 6 | Pedidos | `/PedidosWeb` | Autenticado (escopo por comprador) | Lista, detalhe, transições de status |
| 7 | Entregas e rotas | `/EntregasWeb` | Autenticado; criar = Admin | Rotas e entregas, evidência |
| 8 | Faturas | `/Faturas` | Autenticado | Totais em aberto/pagos e lista |
| 9 | Relatórios | `/Relatorios/VendasPorProdutor` | Admin | Vendas por produtor e período |
| 10 | Inclusão | `/Inclusao` | Autenticado | Produtores por comunidade e acessibilidade |
| 11 | Usuários | `/Usuarios` | Admin | Contas e papéis |
| 12 | Auditoria | `/Usuarios/Auditoria` | Admin | Trilha de ações sensíveis |
| 13 | Privacidade | `/Home/Privacidade` | Anônimo | Política de privacidade (LGPD) |
| 14 | Acesso negado | `/Account/AcessoNegado` | Autenticado | Mensagem de permissão insuficiente |

## Fluxos cobertos

- Login por cookie com redirecionamento e logout.
- Aprovação de produtor e cadastro de cooperativa.
- Cadastro de produto e consulta do catálogo.
- Detalhe do pedido com transições válidas de status.
- Programação de rota com seleção de pedidos sem entrega.
- Emissão de fatura a partir do detalhe do pedido (Admin).

## Acessibilidade (WCAG 2.1 AA)

- Link “Pular para o conteúdo” no topo do layout.
- `lang="pt-BR"`, títulos de página em todas as telas.
- Tabelas com `<caption>`, `<thead>` e `scope="col"`.
- Formulários com `<label>` associado e mensagens de erro em texto.
- Foco visível e navegação completa por teclado.
- Contraste conferido na paleta Bootstrap utilizada.

Checklist detalhado em `../PIM_4-web/docs/seguranca-lgpd.md`.

## Capturas

As imagens das telas foram geradas em 2026-11-09 com a aplicação conectada ao
SQL Server 2022 em container, usando Chromium headless em 1280×900:

- `capturas/01-login.png` … `capturas/15-pedido-detalhe.png`.

Inclui painel, cadastros, pedidos, entregas/rotas, faturas, relatórios,
inclusão, usuários, auditoria, privacidade e formulários de criação.
