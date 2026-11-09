# DevOps e implantação

## 1. Execução local

```bash
# Sobe SQL Server e a aplicação Web
docker compose -f docker/docker-compose.yml up -d --build

# Acompanha os logs da aplicação
docker compose -f docker/docker-compose.yml logs -f web

# Encerra
docker compose -f docker/docker-compose.yml down
```

- Aplicação: <http://localhost:8080>
- Swagger (ambiente de demonstração): <http://localhost:8080/swagger>
- SQL Server: `localhost:1433` (usuário `sa`, senha padrão `AgroConecta@2026` — apenas desenvolvimento)

O serviço `web` aguarda o banco ficar saudável antes de iniciar. No ambiente
de demonstração (`ASPNETCORE_ENVIRONMENT=Development`) as migrations e o seed
são aplicados automaticamente na inicialização.

### Variáveis de ambiente

| Variável | Uso | Padrão local |
|---|---|---|
| `MSSQL_SA_PASSWORD` | Senha do SQL Server | `AgroConecta@2026` |
| `JWT_CHAVE` | Chave HMAC de assinatura dos tokens (32+ bytes) | chave de desenvolvimento |
| `ConnectionStrings__Default` | String de conexão | aponta para o serviço `db` |

## 2. Pipeline de CI (GitHub Actions)

`.github/workflows/ci.yml` executa em cada push para `main`/`feat/**` e em pull requests:

1. `dotnet restore`, `dotnet build -c Release` e `dotnet test`.
2. Publicação dos resultados `.trx` como artefato.
3. Build da imagem Docker da aplicação.

## 3. Banco de dados

- DDL, procedures, triggers e seed completos em `db/script-completo.sql`.
- Script regenerável com `scripts/gerar-script-banco.sh`.
- Em produção, aplicar migrations com `dotnet ef database update` ou o script versionado.

## 4. Plano de implantação no Azure

| Componente | Serviço Azure | Observações |
|---|---|---|
| Aplicação Web/API | Azure App Service (Linux, .NET 10) ou Azure Container Apps | HTTPS gerenciado, autoescala |
| Banco de dados | Azure SQL Database | Backup automático, geo-replicação opcional |
| Imagens de evidência | Azure Blob Storage | Substituir `ArmazenamentoEvidencia` por implementação de blob |
| Segredos | Azure Key Vault | String de conexão e `Jwt:Chave` |
| Observabilidade | Application Insights | Logs, métricas e `traceId` |
| CI/CD | GitHub Actions | Build, testes e deploy via `azure/webapps-deploy` |

### Etapas de implantação

1. Provisionar grupo de recursos, App Service Plan e Azure SQL.
2. Configurar Key Vault e referências de segredo no App Service.
3. Publicar a imagem/artefato e aplicar migrations.
4. Configurar domínio, certificado TLS e regras de firewall.
5. Habilitar Application Insights e alertas de erro 5xx.
6. Executar teste de fumaça (login, catálogo, pedido e entrega).

## 5. Checklist de produção

- [ ] `Jwt:Chave` e senha do banco somente em Key Vault.
- [ ] `ASPNETCORE_ENVIRONMENT=Production` (Swagger desabilitado).
- [ ] HTTPS obrigatório e HSTS ativo.
- [ ] Storage de evidências em Blob/objeto externo.
- [ ] Backup e retenção do Azure SQL configurados.
- [ ] Alertas de disponibilidade e erro configurados.
