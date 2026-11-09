# Evidências — Testes e qualidade

## Saídas capturadas (2026-11-09)

| Arquivo | Conteúdo |
|---|---|
| `build-release.txt` | Compilação `Release` da solução |
| `testes-release.txt` | Execução da suíte (`159` testes aprovados) |
| `cobertura.txt` | Cobertura por projeto (Meta: Domain e Application ≥ 70%) |

## Cobertura

| Projeto | Linhas | Branches |
|---|---:|---:|
| AgroConecta.Domain | 92,5% | 80,0% |
| AgroConecta.Application | 93,8% | 77,7% |
| AgroConecta.Web | 76,7% | 62,5% |
| AgroConecta.Infrastructure | 27,5% | 82,0% |

Infrastructure concentra acesso a dados e integrações; é exercitada de ponta a
ponta pelos testes de API com o provider InMemory.

## Suítes automatizadas

- `Tests/Dominio` — regras de pedido, entrega, lote, fatura e validações.
- `Tests/Aplicacao` — casos de uso com repositórios mockados (Moq).
- `Tests/Banco` e `Tests/Infraestrutura` — modelo de dados, seed e repositórios.
- `Tests/Api` — integração com `WebApplicationFactory` e banco InMemory:
  autenticação, autorização por papel, produtos, lotes, pedidos, entregas,
  evidência multipart, rotas, faturas, relatórios, auditoria e CORS.
- `Tests/Api/FluxoCompletoApiTests` — fluxo produtor → produto → lote → pedido
  → rota → entrega → fatura.
- `Tests/Api/WebMvcTests` — login por cookie, telas administrativas, cabeçalhos
  de segurança, privacidade e bloqueios de acesso.

## Como reproduzir

```bash
cd ../PIM_4-web
dotnet build AgroConecta.sln -c Release
dotnet test  AgroConecta.sln -c Release
dotnet test  AgroConecta.sln -c Release --collect:"XPlat Code Coverage"
```
