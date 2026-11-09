# PIM IV — Escopo do Projeto AgroConecta

> **Documento de trabalho — versão 2.0**
>
> **Fonte de verdade:** `docs/manual.md`
> **Substitui:** rascunho v1.0
> **Tema do manual:** Desenvolvimento e Implantação de uma Solução Tecnológica Corporativa Integrada para Apoiar a Transformação Digital de uma Empresa.
> **Título de trabalho:** AgroConecta: plataforma corporativa integrada para gestão da cadeia de suprimentos e inclusão digital na agricultura familiar.

Este documento define **o que será entregue, por quê e onde cada exigência do manual é atendida**. Ele não é o PIM final: é o plano de escopo que orienta a execução e a redação do trabalho em ABNT.

**Legenda:** ✅ coberto · ⚠️ parcial · ❌ fora do MVP

---

## 0. Como usar este documento

1. Cada seção corresponde a uma etapa obrigatória do manual (seção 2.4).
2. A seção 12 consolida o que entra, o que sai e o que fica como trabalho futuro.
3. A seção 13 é a matriz de rastreabilidade: liga cada critério de avaliação ao artefato que o comprova.
4. A seção 14 define a estrutura e o orçamento de páginas do documento final.
5. Tudo que estiver marcado como **fictício** deve ser declarado como tal no texto acadêmico.

---

## 1. Premissas e decisões de escopo

| # | Premissa | Consequência |
|---|---|---|
| 1 | **Sem continuidade do PIM III.** A organização será apresentada integralmente na Etapa 1. | A Etapa 1 precisa ser autossuficiente e detalhada. |
| 2 | **Empresa-caso fictícia, declarada no texto.** | Evita risco de representar dados reais como verdadeiros. |
| 3 | **Um único SGBD: SQL Server 2022.** | Procedures e triggers em T-SQL, coerentes com o ecossistema .NET/Azure. |
| 4 | **Web em .NET 8:** ASP.NET Core Web API + MVC. | Atende à disciplina "Desenvolvimento Web com .NET". |
| 5 | **Mobile em React Native + Expo + TypeScript.** | Ajustável se a disciplina exigir Android/Kotlin ou Flutter. |
| 6 | **Monólito modular**, não microsserviços. | Menos infraestrutura, mais entrega funcional. |
| 7 | **Cloud/DevOps é plano de execução.** Deploy real é opcional. | Atende ao critério 08 sem consumir tempo excessivo. |
| 8 | **Entrega = protótipo funcional + documento ABNT.** | O foco são Web, Mobile e Banco (5,0 dos 10,0 pontos). |
| 9 | **Grupo de até 6 integrantes.** | Se individual, aplicar os cortes da seção 12.3. |

### 1.1 O que o manual não exige

O manual **não exige** sistema em produção, deploy real, app publicado nas lojas, gateway de pagamento, emissão real de nota fiscal, roteirização otimizada ou rastreamento GPS contínuo. Exige uma solução tecnológica completa **documentada e demonstrável**. Essa distinção é o que permite entregar um projeto sólido dentro de 20 a 30 páginas de desenvolvimento.

### 1.2 Conformidade administrativa

- Postar o trabalho no sistema de trabalhos acadêmicos da UNIP dentro do prazo e entregar cópia impressa ao coordenador.
- Trabalho inédito: trechos de terceiros exigem citação e referência ABNT; plágio e autoplágio causam reprovação.
- Grupo de até 6 integrantes do mesmo curso e turma; apenas o líder cadastra o grupo e envia a versão final.
- Formatação ABNT é eliminatória: trabalho fora do padrão pode ser reprovado sumariamente.

---

## 2. Aderência ao manual

| Critério de avaliação | Peso | Como o projeto atende |
|---|---:|---|
| 01 — Formatação ABNT rigorosa | 1,0 | Template ABNT desde a primeira semana; citações, referências, figuras e tabelas com fonte |
| 02 — Caracterização da organização | 1,0 | Empresa-caso completa: organograma, processos, dores quantificadas, SWOT |
| 03 — Planejamento da solução | 1,0 | Lean Canvas, proposta de valor, modelo de receita, custos e viabilidade |
| 04 — Responsabilidade social e diversidade | 0,5 | Ações afirmativas, acessibilidade WCAG aplicada, indicadores e LGPD |
| 05 — Desenvolvimento Web | 2,0 | Aplicação .NET funcional, APIs REST, autenticação, dashboard e telas |
| 06 — Desenvolvimento Mobile | 2,0 | App funcional, telas, navegação, login, API e sincronização offline básica |
| 07 — Projeto do Banco de Dados | 1,0 | MER, modelo lógico/físico, DDL, DML, procedures, triggers e índices |
| 08 — Infraestrutura em Nuvem e DevOps | 0,5 | Plano Azure, Dockerfile, pipeline CI/CD, monitoramento e custos |
| 09 — Gerenciamento Ágil | 1,0 | Product Backlog, 4 sprints, Kanban, DoR/DoD e riscos |
| **Total** | **10,0** | |

**Leitura estratégica:** Web + Mobile + Banco = **5,0**. ABNT é porta de reprovação sumária. Cloud/DevOps vale **0,5** e deve ficar no nível de plano. O esforço de execução prioriza a ordem: Banco → Web → Mobile → evidências → documento.

---

## 3. Etapa 1 — Caracterização da organização ✅

### 3.1 A empresa-caso

| Item | Descrição |
|---|---|
| **Razão social** | AgroConecta Distribuidora Ltda. (**fictícia**) |
| **Sede** | Campinas/SP |
| **Centro de distribuição** | Valinhos/SP |
| **Atuação** | SP, MG e PR |
| **Fundação** | 2016 |
| **Porte** | 48 colaboradores; faturamento anual fictício de R$ 18 milhões |
| **Segmento** | Distribuição de hortifrúti e insumos agrícolas; elo entre agricultura familiar/cooperativas e compradores B2B |
| **Produtos/serviços** | Hortifrúti in natura, grãos selecionados, insumos básicos; consolidação, armazenagem, roteirização e entrega |
| **Clientes** | 320 compradores B2B: supermercados, restaurantes e hortifrútis |
| **Fornecedores** | 140 produtores rurais e 6 cooperativas |

> **Contingência:** se a disciplina exigir uma organização real, substituir a empresa-caso por uma distribuidora ou cooperativa real, utilizando apenas dados públicos e autorizados. A estrutura do projeto permanece a mesma.

### 3.2 Estrutura organizacional

```text
Diretoria Executiva (CEO / CFO)
├── Comercial (12)
├── Logística e Distribuição (18)
├── Tecnologia da Informação (4)
├── Qualidade e Impacto Social (5)
└── Administrativo-Financeiro (9)
```

### 3.3 Processos atuais

1. Cadastro de produtor (formulário em papel ou WhatsApp).
2. Catálogo de produtos controlado em planilhas.
3. Recebimento de pedidos por telefone, e-mail e WhatsApp.
4. Consolidação manual de estoque rural.
5. Montagem de rotas em planilha.
6. Entrega com confirmação por mensagem.
7. Faturamento em sistema legado.
8. Pós-venda e tratamento de ocorrências por telefone.

### 3.4 Dores identificadas (dados internos fictícios)

| Dor | Indicador atual | Impacto |
|---|---:|---|
| Perdas pós-colheita | 18% | Prejuízo financeiro e desperdício de alimentos |
| Triagem manual de pedidos | 4 h/dia | Atraso no despacho e retrabalho |
| Entregas atrasadas | 12% | Insatisfação de compradores |
| Produtores sem acesso digital | 30% | Exclusão e dependência de intermediários |
| Fechamento de faturamento | 2 dias | Baixa previsibilidade financeira |
| Rastreabilidade por lote | inexistente | Impossibilidade de responder a recalls e auditorias |

### 3.5 Oportunidades de melhoria

- Portal B2B para catálogo, pedido e acompanhamento.
- Aplicativo de campo com operação offline para produtores e motoristas.
- Rastreabilidade por lote de colheita.
- Dashboard de indicadores operacionais e de inclusão.
- Acessibilidade digital para produtores com baixo letramento digital.

### 3.6 SWOT

| Forças | Fraquezas |
|---|---|
| Rede regional consolidada de produtores; relacionamento com cooperativas; conhecimento logístico | Processos manuais; baixa digitalização; ausência de dados históricos confiáveis |
| **Oportunidades** | **Ameaças** |
| Transformação digital do setor; demanda por rastreabilidade; ODS 2, 8 e 10; inclusão produtiva | Concorrência de grandes distribuidoras; volatilidade de preços; sazonalidade agrícola |

**Evidências a produzir:** organograma, fluxograma dos processos (BPMN), tabela de dores, matriz SWOT, descrição narrativa da empresa.

---

## 4. Etapa 2 — Planejamento da solução tecnológica ✅

### 4.1 Problema e objetivos

**Problema:** a AgroConecta opera com processos manuais e fragmentados, o que gera perdas, atrasos, baixa rastreabilidade e exclusão de produtores com pouca familiaridade digital.

**Objetivo geral:** implantar uma plataforma corporativa integrada que conecte produtores, cooperativas, compradores e motoristas, cobrindo cadastro, catálogo, pedido, estoque, entrega, faturamento e indicadores.

**Objetivos específicos:**

1. Centralizar cadastro de produtores, cooperativas, produtos e compradores.
2. Digitalizar o fluxo de pedido e faturamento.
3. Dar rastreabilidade por lote de colheita.
4. Permitir operação mobile offline no campo e na entrega.
5. Reduzir retrabalho e tempo de triagem.
6. Produzir indicadores de inclusão e acessibilidade.
7. Aplicar segurança e LGPD no tratamento de dados.

### 4.2 Público-alvo e personas

| Persona | Papel | Necessidade principal | Canal |
|---|---|---|---|
| **Produtor rural** | Registra colheita e acompanha pedidos | Simplicidade, funcionamento sem internet | Mobile |
| **Motorista** | Executa e confirma entregas | Lista do dia, status, foto e geolocalização | Mobile |
| **Comprador B2B** | Consulta catálogo e cria pedidos | Preço, disponibilidade e prazo | Web |
| **Administrador** | Aprova cadastros, gere pedidos, rotas e faturamento | Visão consolidada e controle | Web |

### 4.3 Proposta de valor

> Conectar produtores familiares e cooperativas a compradores B2B em um único fluxo de catálogo, pedido, logística e rastreabilidade, com operação offline no campo e preço transparente.

**Benefícios esperados:**

| Público | Benefício |
|---|---|
| Produtor | Mais previsibilidade, preço transparente e menos perdas |
| Comprador | Catálogo confiável, prazo e rastreio |
| Distribuidora | Menos retrabalho, dados para decisão e escala |
| Sociedade | Inclusão digital e redução do desperdício de alimentos |

**Diferenciais do MVP:**

- Operação offline simplificada no aplicativo.
- Rastreabilidade por lote de colheita.
- Acessibilidade WCAG nas telas principais.
- Painel de inclusão de cooperativas e comunidades.

> **Decisão:** o "algoritmo de roteirização otimizada" **não** entra no MVP. A rota é montada por regra simples de agrupamento por região e janela de entrega; otimização algorítmica fica como trabalho futuro.

### 4.4 Modelo de negócio (Lean Canvas)

| Bloco | Conteúdo |
|---|---|
| **Problema** | Processos manuais, perdas, atrasos, falta de rastreabilidade e exclusão digital |
| **Segmentos de clientes** | Distribuidoras regionais, cooperativas e compradores B2B |
| **Proposta de valor única** | Cadeia curta digital com rastreabilidade e operação offline |
| **Solução** | Portal web + app mobile + banco corporativo + APIs |
| **Canais** | Venda direta, cooperativas parceiras, associações de produtores |
| **Fontes de receita** | Assinatura SaaS por cooperativa + taxa por transação + implantação |
| **Estrutura de custos** | Cloud, suporte, desenvolvimento, comercial |
| **Métricas-chave** | Pedidos/mês, perdas, entregas no prazo, produtores ativos, adesão ao app |
| **Vantagem competitiva** | Relação regional + rastreabilidade por lote + inclusão digital |

### 4.5 Viabilidade (premissas fictícias para fins acadêmicos)

| Item | Premissa |
|---|---|
| Receita SaaS | R$ 450/mês por cooperativa |
| Receita transacional | 2,5% sobre o GMV |
| Custo fixo mensal | R$ 8.000 (cloud + suporte) |
| Ponto de equilíbrio estimado | 12 cooperativas ativas **ou** 8 cooperativas + R$ 160 mil de GMV/mês |
| Investimento inicial | Desenvolvimento interno e infraestrutura em camada gratuita/básica |

**Análise:** a viabilidade é qualitativa e operacional. O projeto acadêmico não precisa de valuation; precisa demonstrar que a solução reduz custo, melhora prazo e cria receita recorrente.

### 4.6 Métricas e metas (não são resultados medidos)

| Indicador | Linha de base fictícia | Meta | Premissa |
|---|---:|---:|---|
| Tempo de triagem de pedidos | 4 h/dia | 1 h/dia | 80% dos pedidos via plataforma |
| Perdas pós-colheita | 18% | 12% | Rastreabilidade por lote + rota agrupada |
| Entregas atrasadas | 12% | 6% | Status em tempo real no app |
| Produtores com cadastro digital | 70% | 100% | Apoio presencial e interface simples |
| Entregas com evidência digital | 0% | 90% | Foto + geolocalização no app |
| Telas principais em WCAG AA | 0% | 100% | Checklist aplicado no MVP |

### 4.7 Escopo do MVP

**Entra:**

- Autenticação com 4 papéis: Administrador, Produtor, Comprador, Motorista.
- Cadastro e aprovação de produtores, cooperativas e produtos.
- Registro de lote de colheita.
- Criação e acompanhamento de pedidos.
- Consolidação de estoque e faturamento.
- Programação de rota e alocação de motorista/veículo.
- Aplicativo mobile para produtor e motorista.
- Sincronização offline básica.
- Dashboard de indicadores e painel de inclusão.
- Auditoria de acessos.

**Não entra (trabalho futuro):**

- Gateway de pagamento e conciliação financeira real.
- Emissão de nota fiscal eletrônica.
- Roteirização otimizada por algoritmo.
- Rastreamento GPS contínuo.
- Integração com ERP externo.
- Publicação nas lojas de aplicativos.
- Multicloud ou microsserviços.
- Comandos de voz.

**Evidências a produzir:** Lean Canvas, tabela de viabilidade, backlog inicial, diagrama de contexto do sistema.

---

## 5. Etapa 3 — Responsabilidade social e diversidade ✅

### 5.1 Base legal e conceitual

- Constituição Federal de 1988: dignidade da pessoa humana e igualdade.
- Lei nº 10.639/2003: educação das relações étnico-raciais.
- Lei nº 12.288/2010: Estatuto da Igualdade Racial.
- Lei nº 13.709/2018: LGPD.
- WCAG 2.1: diretrizes de acessibilidade.
- ODS 2, 8 e 10 da Agenda 2030.

### 5.2 Ações previstas

| Eixo | Ação | Evidência no projeto |
|---|---|---|
| **Combate à discriminação** | Regra de precificação transparente e auditável, sem ajuste manual por comunidade | Procedure de cálculo + log de alteração de preço |
| **Valorização da diversidade** | Prioridade de onboarding para cooperativas quilombolas, assentamentos e produtoras negras | Campo `Comunidade` + painel de indicadores |
| **Inclusão digital** | Interface simples, linguagem clara, suporte a fonte ampliada e alto contraste | Checklist WCAG + telas |
| **Acessibilidade** | Navegação por teclado, labels, contraste AA, foco visível | Checklist aplicado nas telas Web e Mobile |
| **Cidadania** | Módulo de capacitação digital e financeira | Conteúdo + controle de progresso (MVP simplificado) |
| **Privacidade** | Consentimento, minimização de dados, controle de acesso e auditoria | Política + `LogAcesso` + perfis |

### 5.3 Acessibilidade aplicada (mínimo demonstrável)

- Contraste mínimo AA (4,5:1 para texto).
- Fonte ajustável no app.
- Labels e textos alternativos em imagens.
- Navegação por teclado na Web.
- Mensagens de erro claras e associadas ao campo.
- Teste com leitor de tela em pelo menos uma tela.

> **Decisão:** comandos de voz ficam como trabalho futuro. O MVP entrega acessibilidade visual e de navegação, que é demonstrável e mensurável.

### 5.4 Indicadores de inclusão

| Indicador | Como medir |
|---|---|
| % de cooperativas lideradas por mulheres | Cadastro da cooperativa |
| % de produtores de comunidades tradicionais | Campo `Comunidade` |
| % de telas aprovadas no checklist WCAG | Checklist versionado |
| % de produtores treinados | Módulo de capacitação |

**Evidências a produzir:** checklist WCAG preenchido, capturas de tela, consulta de indicadores, política de privacidade.

---

## 6. Etapa 4 — Desenvolvimento da solução Web (.NET) ✅

### 6.1 Stack

| Camada | Tecnologia |
|---|---|
| Backend | .NET 8, ASP.NET Core Web API, MVC |
| ORM | Entity Framework Core 8 |
| Banco | SQL Server 2022 |
| Autenticação | ASP.NET Core Identity + JWT (access + refresh) |
| Documentação de API | Swagger / OpenAPI |
| Validação | FluentValidation |
| Testes | xUnit + Moq |
| Logs | Serilog |
| CI/CD | GitHub Actions |

### 6.2 Módulos

1. Autenticação e autorização.
2. Cadastro de produtores, cooperativas e compradores.
3. Catálogo de produtos.
4. Pedidos e itens.
5. Estoque por lote.
6. Rotas e entregas.
7. Faturamento.
8. Indicadores e relatórios.
9. Auditoria.

### 6.3 Telas principais (10 no MVP)

1. Login.
2. Dashboard administrativo.
3. Produtores — lista, detalhe e aprovação.
4. Cooperativas.
5. Produtos.
6. Pedidos — lista, detalhe e mudança de status.
7. Entregas e rotas.
8. Faturas e relatórios.
9. Painel de inclusão e acessibilidade.
10. Usuários e auditoria.

### 6.4 Endpoints REST (mínimo)

| Método | Rota | Finalidade | Papéis |
|---|---|---|---|
| POST | `/api/auth/login` | Autenticar | Público |
| POST | `/api/auth/refresh` | Renovar token | Autenticado |
| GET/POST/PUT | `/api/produtores` | Gerenciar produtores | Admin |
| PATCH | `/api/produtores/{id}/aprovacao` | Aprovar cadastro | Admin |
| GET/POST | `/api/cooperativas` | Gerenciar cooperativas | Admin |
| GET/POST/PUT | `/api/produtos` | Gerenciar catálogo | Admin |
| GET/POST | `/api/pedidos` | Criar e listar pedidos | Comprador/Admin |
| PATCH | `/api/pedidos/{id}/status` | Alterar status | Admin |
| GET/POST | `/api/rotas` | Programar rotas | Admin |
| GET | `/api/entregas/minhas` | Entregas do motorista | Motorista |
| PATCH | `/api/entregas/{id}/status` | Atualizar entrega | Motorista |
| POST | `/api/entregas/{id}/evidencia` | Foto + geolocalização | Motorista |
| GET | `/api/relatorios/vendas-produtor` | Relatório | Admin |
| GET | `/api/indicadores` | Dashboard | Admin |
| GET | `/api/auditoria` | Logs de acesso | Admin |

### 6.5 Autenticação e segurança

- Senhas com hash **PBKDF2** via ASP.NET Core Identity (não BCrypt nativo).
- JWT de acesso curto + refresh token.
- Autorização por políticas e papéis.
- Validação de entrada e proteção contra mass assignment.
- EF Core com consultas parametrizadas.
- HTTPS obrigatório, CORS restrito, rate limiting no login.
- Segredos em variáveis de ambiente/Key Vault.
- Checklist OWASP Top 10 documentado.
- LGPD: consentimento, minimização e auditoria.

### 6.6 Integração entre módulos

Monólito modular com serviços de aplicação internos. A comunicação com o mobile é **REST/JSON**. Não há mensageria no MVP.

**Evidências a produzir:** repositório, Swagger, capturas das telas, testes, seed, vídeo curto de demonstração.

---

## 7. Etapa 5 — Desenvolvimento da solução Mobile ✅

### 7.1 Stack

| Item | Tecnologia |
|---|---|
| Framework | React Native + Expo |
| Linguagem | TypeScript |
| Navegação | React Navigation |
| Armazenamento local | expo-sqlite |
| Token seguro | expo-secure-store |
| HTTP | Axios |
| Conectividade | NetInfo |
| Câmera/localização | expo-camera + expo-location |
| Biometria (opcional) | expo-local-authentication |

> Se a disciplina de Desenvolvimento Mobile exigir Android/Kotlin ou Flutter, substituir a stack mantendo o mesmo escopo funcional.

### 7.2 Telas principais (6 no MVP)

1. Login.
2. Home com tarefas do dia.
3. Lista de entregas (motorista) ou lotes (produtor).
4. Detalhe da entrega/lote.
5. Atualização de status com foto e geolocalização.
6. Perfil e sincronização.

### 7.3 Fluxos de navegação

**Motorista (fluxo principal):**

```text
Login → Home → Minhas entregas → Detalhe → Alterar status
      → Foto + localização → Salvar local → Sincronizar
```

**Produtor (fluxo secundário):**

```text
Login → Home → Meus lotes → Novo lote → Salvar local → Sincronizar
```

Navegação por **Bottom Navigation** (Home, Tarefas, Perfil) e **Drawer** para itens secundários.

### 7.4 Autenticação

- Login com e-mail e senha.
- Token JWT armazenado em `expo-secure-store`.
- Renovação automática via refresh token.
- Biometria opcional para reabrir o app.

### 7.5 Sincronização offline (MVP)

**Estratégia:**

1. Toda ação de campo é gravada primeiro no SQLite local com um `UUID` e timestamp.
2. A fila local é enviada ao servidor quando há conexão.
3. O servidor responde com o estado atualizado.
4. Política de conflito: **last-write-wins** com timestamp do servidor.
5. Conflitos são registrados em log para análise.

**O que o MVP garante:** registrar lote e atualizar status de entrega sem internet, com envio posterior. **O que não garante:** sincronização bidirecional completa de todos os cadastros.

### 7.6 Integração com APIs

Consumo dos endpoints da seção 6.4. O app nunca acessa o banco diretamente.

**Evidências a produzir:** build APK/Expo, capturas das telas, vídeo com modo avião, teste de sincronização, repositório.

---

## 8. Etapa 6 — Arquitetura de software ✅

### 8.1 Estilo arquitetural

**Monólito modular em 4 camadas**, alinhado aos princípios de Clean Architecture sem o excesso de projetos:

```text
Presentation (API + MVC)
        ↓
Application (casos de uso, DTOs, validadores)
        ↓
Domain (entidades, enums, interfaces, regras)
        ↑
Infrastructure (EF Core, repositórios, serviços externos)
```

**Projetos da solução:**

- `AgroConecta.Domain`
- `AgroConecta.Application`
- `AgroConecta.Infrastructure`
- `AgroConecta.Web` (API + MVC)
- `AgroConecta.Tests`

### 8.2 Orientação a objetos — evidências no código

| Pilar | Aplicação no projeto |
|---|---|
| **Abstração** | `EntidadeBase` com `Id`, `CriadoEm`, `AtualizadoEm`; interfaces de repositório |
| **Encapsulamento** | `Pedido` com lista privada de itens e métodos `AdicionarItem`, `CalcularTotal`, `AlterarStatus` |
| **Herança** | `Usuario` base; perfis `Produtor` e `Motorista` com dados específicos |
| **Polimorfismo** | `IValidadorDocumento` com `ValidadorCpf` e `ValidadorCnpj`; `IEstrategiaFrete` com cálculo fixo e por peso |

### 8.3 Princípios SOLID

- **SRP:** cada caso de uso tem uma responsabilidade.
- **OCP:** novas estratégias de frete sem alterar o serviço de pedido.
- **LSP:** validadores de documento intercambiáveis.
- **ISP:** interfaces de repositório específicas por agregado.
- **DIP:** injeção de dependência aponta para interfaces do domínio.

### 8.4 Modularização

Módulos por domínio dentro do monólito: Cadastro, Catálogo, Pedidos, Estoque, Logística, Faturamento, Indicadores. Cada módulo tem seus casos de uso e repositórios, sem acoplamento direto entre si.

**Evidências a produzir:** diagrama de camadas, diagrama de classes, trechos de código, testes unitários.

---

## 9. Etapa 7 — Projeto do banco de dados ✅

### 9.1 SGBD e convenções

- **SQL Server 2022** (T-SQL).
- Schema `dbo`.
- PK `Id` `int identity` em todas as tabelas; `LogAcesso.Id` como `bigint identity`.
- FK no padrão `{Entidade}Id`.
- Datas em `datetime2`; valores em `decimal(18,2)`.
- `CriadoEm` e `AtualizadoEm` em todas as tabelas principais.
- Nomes de tabelas e colunas em PascalCase.

### 9.2 Entidades (14 tabelas)

| Tabela | Finalidade | Campos principais |
|---|---|---|
| `Usuario` | Autenticação e papéis | Nome, Email, SenhaHash, Papel, Ativo |
| `Produtor` | Perfil do produtor | UsuarioId, CooperativaId, NomePropriedade, Documento, Cidade, UF, Comunidade, StatusCadastro |
| `Cooperativa` | Cooperativas parceiras | Nome, Cidade, UF, Tipo, Ativa |
| `Produto` | Catálogo | Nome, Categoria, Unidade, PrecoBase, Ativo |
| `LoteColheita` | Rastreabilidade | ProdutorId, ProdutoId, CodigoRastreio, DataColheita, Quantidade, Status |
| `MovimentoEstoque` | Entradas e saídas | LoteId, Tipo, Quantidade, PedidoId, CriadoEm |
| `Pedido` | Pedido B2B | CompradorId, DataPedido, Status, ValorTotal, EnderecoEntrega |
| `ItemPedido` | Itens do pedido | PedidoId, ProdutoId, LoteId, Quantidade, PrecoUnitario, Subtotal |
| `Veiculo` | Frota | Placa, Tipo, CapacidadeKg, Ativo |
| `Motorista` | Perfil do motorista | UsuarioId, CNH, Ativo |
| `Rota` | Agrupamento de entregas | Data, MotoristaId, VeiculoId, Status |
| `Entrega` | Execução da entrega | PedidoId, RotaId, Status, DataPrevista, DataRealizada, Latitude, Longitude, FotoUrl, Observacao |
| `Fatura` | Faturamento | PedidoId, Numero, Valor, Status, EmitidaEm, PagoEm |
| `LogAcesso` | Auditoria | UsuarioId, Acao, Entidade, EntidadeId, IP, UserAgent, CriadoEm |

> **Observação:** o comprador não tem tabela própria. Ele é um `Usuario` com `Papel = Comprador`, referenciado em `Pedido.CompradorId`. O veículo é alocado na `Rota`, não no cadastro do motorista.

### 9.3 Relacionamentos

| Relação | Cardinalidade | Exemplo |
|---|---|---|
| `Usuario` — `Produtor` | 1:1 | Um usuário é um produtor |
| `Usuario` — `Motorista` | 1:1 | Um usuário é um motorista |
| `Cooperativa` — `Produtor` | 1:N | Uma cooperativa tem vários produtores |
| `Produtor` — `LoteColheita` | 1:N | Um produtor registra vários lotes |
| `Produto` — `LoteColheita` | 1:N | Um produto aparece em vários lotes |
| `LoteColheita` — `MovimentoEstoque` | 1:N | Um lote tem vários movimentos |
| `Pedido` — `ItemPedido` | 1:N | Um pedido tem vários itens |
| `Pedido` — `Produto` | N:M | Via `ItemPedido` |
| `Pedido` — `Entrega` | 1:1 | Um pedido tem uma entrega |
| `Rota` — `Entrega` | 1:N | Uma rota agrupa várias entregas |
| `Motorista` — `Rota` | 1:N | Um motorista executa várias rotas |
| `Pedido` — `Fatura` | 1:1 | Um pedido tem uma fatura |
| `Usuario` — `LogAcesso` | 1:N | Um usuário gera vários logs |

### 9.4 Procedures, triggers e índices

**Stored procedures (3):**

| Procedure | Função |
|---|---|
| `sp_CalcularTotalPedido` | Recalcula subtotais e total do pedido |
| `sp_RelatorioVendasProdutor` | Relatório de vendas por produtor e período |
| `sp_ConsolidarEstoqueLote` | Consolida saldo por lote |

**Triggers (3):**

| Trigger | Evento | Função |
|---|---|---|
| `trg_ItemPedido_MovimentoEstoque` | INSERT/UPDATE/DELETE em `ItemPedido` | Gera movimento de estoque |
| `trg_Entrega_LogStatus` | UPDATE de status em `Entrega` | Registra histórico de status |
| `trg_Produtor_AuditaAprovacao` | UPDATE de `StatusCadastro` | Registra aprovação e responsável |

**Índices principais:**

- `IX_Pedido_Status_DataPedido`
- `IX_Entrega_Status_DataPrevista`
- `IX_LoteColheita_ProdutorId_DataColheita`
- `IX_LogAcesso_UsuarioId_CriadoEm`
- `UQ_Produtor_UsuarioId`, `UQ_Motorista_UsuarioId`
- `UQ_Entrega_PedidoId`, `UQ_Fatura_PedidoId`

### 9.5 Script e evidências

- DDL completo: `CREATE TABLE`, `ALTER TABLE`, `CREATE INDEX`.
- DML de carga: usuários, cooperativa, produtores, produtos, lotes, pedidos, rota, entregas e faturas.
- Dicionário de dados.
- MER em imagem (notação Chen ou pé de galinha).
- Modelo lógico e físico.
- Execução do script com saída de validação.

---

## 10. Etapa 8 — Infraestrutura em nuvem e DevOps ✅ (plano)

> **Escopo:** plano de execução. O deploy real é diferencial opcional e não é pré-requisito para a nota.

### 10.1 Arquitetura Azure proposta

| Componente | Serviço |
|---|---|
| Aplicação Web/API | Azure App Service (Linux, .NET 8) |
| Banco de dados | Azure SQL Database |
| Armazenamento de imagens | Azure Blob Storage |
| Segredos | Azure Key Vault |
| Monitoramento | Application Insights |
| Containers | Docker + Azure Container Registry (opcional) |
| CI/CD | GitHub Actions |
| Segurança de borda | Azure Front Door/WAF (configuração proposta) |

### 10.2 Containers e pipeline

- `Dockerfile` multi-stage para a API/Web.
- `docker-compose` local com API + SQL Server.
- Pipeline CI: restore, build, testes, cobertura, build da imagem.
- Pipeline CD: deploy em staging, migrations, aprovação manual, produção.
- Versionamento de migrations do EF Core.

### 10.3 Monitoramento, escalabilidade e segurança

- Application Insights: requisições, falhas, dependências e alertas.
- Auto-scale do App Service por CPU > 70%.
- Escala do Azure SQL por DTU/vCore.
- TLS/HTTPS obrigatório, managed identity e Key Vault.
- Backup e retenção do banco.
- LGPD: região de dados, controle de acesso e retenção.

### 10.4 Custos estimados (cenário acadêmico)

| Item | Faixa mensal |
|---|---:|
| App Service B1 | R$ 70–120 |
| Azure SQL Basic/S0 | R$ 25–80 |
| Blob Storage | R$ 5–20 |
| Application Insights | R$ 0–30 |
| **Total** | **R$ 100–250** |

**Evidências a produzir:** diagrama de arquitetura, `Dockerfile`, workflow YAML, tabela de custos, plano de monitoramento.

---

## 11. Etapa 9 — Gerenciamento ágil do projeto ✅

### 11.1 Papéis e cerimônias

| Papel | Responsabilidade |
|---|---|
| Product Owner | Priorizar backlog e validar entregas |
| Scrum Master | Remover impedimentos e conduzir cerimônias |
| Dev Team | Desenvolver, testar e documentar |

**Cerimônias:** planning, daily, review e retrospectiva, em sprints de 2 semanas.

### 11.2 Product Backlog (MVP)

| ID | História | Prioridade | Estimativa |
|---|---|---|---:|
| US01 | Como usuário, quero autenticar-me com e-mail e senha para acessar o sistema | Alta | 5 |
| US02 | Como admin, quero cadastrar e aprovar produtores | Alta | 5 |
| US03 | Como admin, quero cadastrar cooperativas e vinculá-las a produtores | Alta | 3 |
| US04 | Como admin, quero cadastrar produtos e preços-base | Alta | 3 |
| US05 | Como produtor, quero registrar um lote de colheita pelo app | Alta | 8 |
| US06 | Como comprador, quero consultar o catálogo e criar um pedido | Alta | 8 |
| US07 | Como admin, quero aprovar pedidos e gerar fatura | Alta | 5 |
| US08 | Como admin, quero consolidar estoque por lote | Média | 5 |
| US09 | Como admin, quero programar rota e alocar motorista/veículo | Média | 5 |
| US10 | Como motorista, quero ver minhas entregas do dia | Alta | 5 |
| US11 | Como motorista, quero atualizar o status com foto e localização | Alta | 8 |
| US12 | Como motorista, quero sincronizar ações feitas offline | Alta | 8 |
| US13 | Como admin, quero visualizar indicadores no dashboard | Média | 5 |
| US14 | Como admin, quero relatório de vendas por produtor | Média | 3 |
| US15 | Como admin, quero auditar acessos e alterações | Média | 3 |
| US16 | Como gestor, quero acompanhar indicadores de inclusão | Baixa | 3 |

### 11.3 Sprint Backlog e cronograma (4 × 2 semanas)

| Sprint | Objetivo | Entregas |
|---|---|---|
| **1** | Fundação | Repositório, arquitetura, banco, autenticação, CI |
| **2** | Web administrativo | Cadastros, catálogo, pedidos, faturamento, rotas |
| **3** | Mobile | Login, entregas/lotes, status, offline, integração |
| **4** | Fechamento | Dashboard, acessibilidade, testes, pipeline, deploy opcional, documentação |

### 11.4 Kanban

```text
Backlog → To Do → In Progress → Code Review → Testing → Done
```

Limites de WIP: 2 em `In Progress`, 2 em `Code Review`, 2 em `Testing`.

### 11.5 DoR e DoD

**Definition of Ready (DoR):**

- História com critério de aceite.
- Tela/protótipo definido.
- Dependências identificadas.
- Estimativa aprovada.

**Definition of Done (DoD):**

- Código revisado.
- Testes unitários e de integração verdes.
- Migration aplicada.
- Documentação atualizada.
- Acessibilidade verificada.
- Deploy em staging quando aplicável.

### 11.6 Riscos

| Risco | Impacto | Mitigação |
|---|---|---|
| Escopo maior que o prazo | Alto | Priorizar Web, Mobile e Banco; cortar diferenciais |
| Curva de aprendizado mobile | Médio | Usar Expo e protótipo incremental |
| Conflitos de sincronização | Médio | Política last-write-wins + log |
| Dados fictícios pouco críveis | Médio | Declarar ficção e usar seed consistente |
| Atraso na formatação ABNT | Alto | Template e escrita paralela desde a semana 1 |

**Evidências a produzir:** quadro Kanban, backlog, cronograma, burndown, atas de review.

---

## 12. Escopo consolidado

### 12.1 Manter no MVP

- Autenticação e papéis.
- Cadastros de produtor, cooperativa, produto e comprador.
- Lote de colheita e movimentação de estoque.
- Pedido, faturamento e relatório.
- Rota e entrega.
- App mobile com offline básico.
- Dashboard e painel de inclusão.
- Banco completo com procedures e triggers.
- Plano cloud/DevOps.
- Pacote ágil.
- Documento ABNT.

### 12.2 Cortar ou rebaixar

| Item do rascunho original | Decisão |
|---|---|
| Algoritmo de roteirização otimizada | Regra simples de agrupamento; otimização como trabalho futuro |
| Assinatura digital | Foto + data/hora + geolocalização |
| Biometria | Opcional, não obrigatória |
| Mensageria interna | REST síncrono |
| Módulo de pagamento | Fora do MVP |
| WAF e auto-scale reais | Apenas configuração proposta |
| Clean Architecture com módulos por domínio | 4 camadas |
| Multicloud e múltiplos SGBDs | Um SGBD: SQL Server |
| Comandos de voz | Trabalho futuro |

### 12.3 Ajuste para trabalho individual

Se o PIM for individual, reduzir para:

- 1 persona mobile prioritária: **motorista**.
- 11 tabelas: remover `Cooperativa`, `Rota` e `Veiculo` do MVP; manter `Motorista` simplificado.
- 1 procedure e 2 triggers.
- 6 telas web e 5 telas mobile.
- Sem biometria e sem módulo de capacitação.
- Mesmo assim, manter: MER, DDL, autenticação, pedido, entrega, offline básico e ABNT.

---

## 13. Matriz de rastreabilidade

| Critério do manual | Peso | Etapa | Artefato/evidência mínima | Seção do PIM final |
|---|---:|---|---|---|
| 01 — ABNT rigorosa | 1,0 | Transversal | Template, citações, referências, figuras/tabelas com fonte, resumo, abstract, sumário | Todo o documento |
| 02 — Organização | 1,0 | 1 | Empresa-caso, organograma, processos, dores, SWOT | 6.1 |
| 03 — Planejamento | 1,0 | 2 | Lean Canvas, proposta de valor, viabilidade, métricas, escopo | 6.2 |
| 04 — Social e diversidade | 0,5 | 3 | Base legal, ações, checklist WCAG, indicadores, LGPD | 6.3 |
| 05 — Web | 2,0 | 4 | Repositório, 10 telas, endpoints, JWT, segurança, testes | 6.4 |
| 06 — Mobile | 2,0 | 5 | App, 6 telas, login, API, offline, vídeo | 6.5 |
| 07 — Banco | 1,0 | 7 | MER, modelo lógico/físico, DDL, DML, procedures, triggers | 6.7 |
| 08 — Cloud/DevOps | 0,5 | 8 | Diagrama Azure, Dockerfile, pipeline, monitoramento, custos | 6.8 |
| 09 — Ágil | 1,0 | 9 | Backlog, sprints, Kanban, DoR/DoD, riscos | 6.9 |

**Etapa 6 (arquitetura)** não tem critério próprio, mas sustenta os critérios 05, 06 e 07. Deve aparecer como subseção do desenvolvimento (6.6) com diagramas de camadas e classes.

---

## 14. Documento final e ABNT

### 14.1 Estrutura obrigatória

1. Capa.
2. Folha de rosto (recomendada).
3. Resumo (150–500 palavras) + palavras-chave.
4. Abstract + keywords.
5. Sumário.
6. Introdução.
7. Desenvolvimento (20–30 páginas).
8. Conclusão.
9. Referências.

Apêndices (backlog, script SQL, checklist WCAG) são opcionais e **não contam** nas 20–30 páginas do desenvolvimento.

### 14.2 Orçamento de páginas do desenvolvimento

| Seção | Páginas |
|---|---:|
| 6.1 Etapa 1 — Organização | 3 |
| 6.2 Etapa 2 — Planejamento e viabilidade | 3 |
| 6.3 Etapa 3 — Social e diversidade | 2 |
| 6.4 Etapa 4 — Web | 4 |
| 6.5 Etapa 5 — Mobile | 4 |
| 6.6 Etapa 6 — Arquitetura | 3 |
| 6.7 Etapa 7 — Banco | 4 |
| 6.8 Etapa 8 — Cloud/DevOps | 2 |
| 6.9 Etapa 9 — Ágil | 2 |
| **Total** | **27** |

Introdução: 2–3 páginas, contendo objetivo da pesquisa, metodologia e breve apresentação da empresa. Conclusão: 1–2 páginas, retomando as propostas iniciais e os resultados alcançados. Referências: 2–3 páginas.

**Regra de redação:** cada seção deve combinar fundamentação teórica (com citações ABNT) e decisões do projeto. Não basta descrever a solução: é preciso justificar as escolhas com literatura e analisar os resultados do protótipo.

### 14.3 Checklist ABNT

| Item | Regra |
|---|---|
| Papel | A4 (210 × 297 mm), branco |
| Margens | Superior e esquerda 3,0 cm; inferior e direita 2,0 cm |
| Fonte | Arial ou Times New Roman 12; citações longas e notas em 10 |
| Espaçamento | 1,5 entrelinhas; citações longas, notas, referências e resumo em espaço simples |
| Parágrafo | Recuo de 1,5 cm na primeira linha |
| Alinhamento | Justificado |
| Paginação | Contada a partir da folha de rosto; numerada a partir da introdução, no canto superior direito |
| Citações | Diretas, indiretas e citação de citação conforme NBR 10520 |
| Referências | Ordem alfabética, conforme NBR 6023 |
| Figuras e tabelas | Título, número, fonte e chamada no texto |
| Resumo | 150–500 palavras, seguido de palavras-chave |
| Abstract | Versão em inglês, com keywords |

> O manual adota a NBR 14724:2002. Se o professor exigir a versão vigente, usar a NBR 14724:2011, mantendo o restante do guia da UNIP.

---

## 15. Referências preliminares

> **Conferir edição, ano e disponibilidade antes de citar no PIM final.** As referências abaixo são um ponto de partida, não uma lista fechada.

ASSOCIAÇÃO BRASILEIRA DE NORMAS TÉCNICAS. **NBR 14724**: informação e documentação — trabalhos acadêmicos — apresentação. Rio de Janeiro: ABNT, 2002.

ASSOCIAÇÃO BRASILEIRA DE NORMAS TÉCNICAS. **NBR 6023**: informação e documentação — referências — elaboração. Rio de Janeiro: ABNT, 2018.

ASSOCIAÇÃO BRASILEIRA DE NORMAS TÉCNICAS. **NBR 10520**: informação e documentação — citações em documentos — apresentação. Rio de Janeiro: ABNT, 2002.

ASSOCIAÇÃO BRASILEIRA DE NORMAS TÉCNICAS. **NBR 6027**: informação e documentação — sumário — apresentação. Rio de Janeiro: ABNT, 2012.

ASSOCIAÇÃO BRASILEIRA DE NORMAS TÉCNICAS. **NBR 6028**: informação e documentação — resumo — apresentação. Rio de Janeiro: ABNT, 2003.

BRASIL. **Constituição da República Federativa do Brasil de 1988**. Brasília, DF: Presidência da República, 1988.

BRASIL. **Lei nº 10.639, de 9 de janeiro de 2003**. Altera a Lei nº 9.394, de 20 de dezembro de 1996. Diário Oficial da União, Brasília, DF, 10 jan. 2003.

BRASIL. **Lei nº 12.288, de 20 de julho de 2010**. Institui o Estatuto da Igualdade Racial. Diário Oficial da União, Brasília, DF, 21 jul. 2010.

BRASIL. **Lei nº 13.709, de 14 de agosto de 2018**. Lei Geral de Proteção de Dados Pessoais (LGPD). Diário Oficial da União, Brasília, DF, 15 ago. 2018.

FAO. **Global food losses and food waste**: extent, causes and prevention. Rome: FAO, 2011.

FIELDING, Roy T. **Architectural styles and the design of network-based software architectures**. 2000. Tese (Doutorado) — University of California, Irvine, 2000.

IBGE. **Censo Agropecuário 2017**: resultados definitivos. Rio de Janeiro: IBGE, 2019.

MARTIN, Robert C. **Clean architecture**: a craftsman's guide to software structure and design. Boston: Prentice Hall, 2017.

MICROSOFT. **ASP.NET Core documentation**. Disponível em: https://learn.microsoft.com/aspnet/core. Acesso em: 9 nov. 2026.

MICROSOFT. **Azure Architecture Center**. Disponível em: https://learn.microsoft.com/azure/architecture. Acesso em: 9 nov. 2026.

MICROSOFT. **Entity Framework Core documentation**. Disponível em: https://learn.microsoft.com/ef/core. Acesso em: 9 nov. 2026.

ONU. **Transformando nosso mundo**: a Agenda 2030 para o desenvolvimento sustentável. Nova York: ONU, 2015.

OSTERWALDER, Alexander; PIGNEUR, Yves. **Business model generation**. Hoboken: Wiley, 2010.

OWASP FOUNDATION. **OWASP Top 10:2021**. 2021. Disponível em: https://owasp.org/Top10/. Acesso em: 9 nov. 2026.

PRESSMAN, Roger S.; MAXIM, Bruce R. **Engenharia de software**. 9. ed. Porto Alegre: McGraw Hill, 2021.

REACT NATIVE. **React Native documentation**. Disponível em: https://reactnative.dev/docs. Acesso em: 9 nov. 2026.

SCHWABER, Ken; SUTHERLAND, Jeff. **The Scrum Guide**. 2020. Disponível em: https://scrumguides.org. Acesso em: 9 nov. 2026.

SOMMERVILLE, Ian. **Engenharia de software**. 10. ed. São Paulo: Pearson, 2019.

TURBAN, Efraim; POLLARD, Carol; WOOD, Gregory. **Information technology for management**: on-demand strategies for performance, growth and sustainability. 11. ed. Hoboken: Wiley, 2018.

W3C. **Web Content Accessibility Guidelines (WCAG) 2.1**. 2018. Disponível em: https://www.w3.org/TR/WCAG21/. Acesso em: 9 nov. 2026.

---

## 16. Próximos passos

1. **Validar a empresa-caso** e o nome do projeto (verificar originalidade de "AgroConecta").
2. **Confirmar a stack mobile** com a disciplina de Desenvolvimento Mobile.
3. **Criar o template ABNT** e o repositório do projeto na primeira semana.
4. **Modelar o banco primeiro** (MER, DDL, seed), pois ele sustenta Web e Mobile.
5. **Implementar autenticação e cadastros** antes dos fluxos de pedido e entrega.
6. **Desenvolver o mobile em paralelo** ao web, priorizando o fluxo do motorista.
7. **Produzir evidências continuamente**: capturas, vídeos, Swagger, testes, checklist WCAG.
8. **Escrever o documento em paralelo**, com 2–3 páginas por etapa.
9. **Revisar ABNT e referências** antes da entrega.
10. **Montar o pacote final**: documento, repositório, script SQL, diagramas e vídeo de demonstração.

---

> **Resumo da decisão:** o tema é viável e aderente ao manual. O projeto deixa de tentar ser um sistema de produção e passa a ser um **protótipo corporativo demonstrável**, com escopo controlado, evidências claras e documentação ABNT. O risco principal não é o tema; é a execução dispersa. O caminho é: banco → web → mobile → evidências → documento.
