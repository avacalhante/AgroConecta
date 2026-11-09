#!/usr/bin/env bash
# Gera db/ddl/001-tabelas.sql a partir das migrations e monta db/script-completo.sql
# com procedures, triggers e carga de desenvolvimento.
set -euo pipefail

raiz="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$raiz"

dotnet ef migrations script \
    -p src/AgroConecta.Infrastructure \
    -s src/AgroConecta.Web \
    -o db/ddl/001-tabelas.sql

{
    cat <<'CABECALHO'
-- ============================================================================
-- AgroConecta — script completo do banco (SQL Server 2022)
-- Gerado por scripts/gerar-script-banco.sh; não edite manualmente.
--
-- Uso:
--   sqlcmd -S localhost,1433 -U sa -P "AgroConecta@2026" -C -d AgroConecta -i db/script-completo.sql
--
-- O banco de destino precisa existir. Crie com:
--   sqlcmd -S localhost,1433 -U sa -P "AgroConecta@2026" -C -Q "CREATE DATABASE AgroConecta"
-- ============================================================================
GO

CABECALHO

    sed '1s/^\xEF\xBB\xBF//' db/ddl/001-tabelas.sql

    for arquivo in db/procedures/*.sql db/triggers/*.sql db/seed/*.sql; do
        printf '\nGO\n'
        cat "$arquivo"
        printf '\n'
    done

    printf '\nGO\n'
} > db/script-completo.sql

echo "Gerado db/script-completo.sql ($(wc -l < db/script-completo.sql) linhas)."
