import * as SQLite from 'expo-sqlite';
import type { SQLiteDatabase } from 'expo-sqlite';

const NOME_BANCO = 'agroconecta.db';
const VERSAO_ESQUEMA = 3;

let promessaBanco: Promise<SQLiteDatabase> | null = null;

/** Abre (uma única vez) o banco local e aplica o esquema. */
export function obterBanco(): Promise<SQLiteDatabase> {
  promessaBanco ??= abrirBanco();
  return promessaBanco;
}

async function abrirBanco(): Promise<SQLiteDatabase> {
  const banco = await SQLite.openDatabaseAsync(NOME_BANCO);
  const linha = await banco.getFirstAsync<{ user_version: number }>('PRAGMA user_version');

  const versao = linha?.user_version ?? 0;

  if (versao < 1) {
    await banco.execAsync(`
PRAGMA journal_mode = WAL;

CREATE TABLE IF NOT EXISTS produtos (
  id INTEGER PRIMARY KEY NOT NULL,
  nome TEXT NOT NULL,
  categoria TEXT NOT NULL,
  unidade TEXT NOT NULL,
  preco_base REAL NOT NULL,
  ativo INTEGER NOT NULL DEFAULT 1
);

CREATE TABLE IF NOT EXISTS lotes (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  id_remoto INTEGER,
  codigo_rastreio TEXT NOT NULL UNIQUE,
  produto_id INTEGER NOT NULL,
  produto_nome TEXT NOT NULL,
  produto_unidade TEXT NOT NULL DEFAULT 'kg',
  data_colheita TEXT NOT NULL,
  quantidade REAL NOT NULL,
  status TEXT NOT NULL,
  criado_em TEXT NOT NULL,
  sincronizado INTEGER NOT NULL DEFAULT 0
);

CREATE INDEX IF NOT EXISTS idx_lotes_sincronizado ON lotes (sincronizado);

CREATE TABLE IF NOT EXISTS fila_sincronizacao (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  tipo TEXT NOT NULL,
  referencia TEXT NOT NULL,
  payload TEXT NOT NULL,
  criado_em TEXT NOT NULL,
  tentativas INTEGER NOT NULL DEFAULT 0,
  ultimo_erro TEXT,
  UNIQUE (tipo, referencia)
);

PRAGMA user_version = 1;
`);

    await semearProdutos(banco);
  }

  if (versao < 2) {
    await banco.execAsync(`
CREATE TABLE IF NOT EXISTS entregas (
  id INTEGER PRIMARY KEY NOT NULL,
  pedido_id INTEGER NOT NULL,
  status TEXT NOT NULL,
  data_prevista TEXT NOT NULL,
  data_realizada TEXT,
  latitude REAL,
  longitude REAL,
  foto_url TEXT,
  observacao TEXT,
  atualizado_em TEXT NOT NULL,
  pendente INTEGER NOT NULL DEFAULT 0,
  erro_sincronizacao TEXT
);

CREATE INDEX IF NOT EXISTS idx_entregas_pendentes ON entregas (pendente);

PRAGMA user_version = 2;
`);
  }

  if (versao < VERSAO_ESQUEMA) {
    await banco.execAsync(`
ALTER TABLE lotes ADD COLUMN erro_sincronizacao TEXT;
ALTER TABLE fila_sincronizacao ADD COLUMN proxima_tentativa_em TEXT;
PRAGMA user_version = 3;
`);
  }

  return banco;
}

/** Produtos do seed permitem registrar um lote já na primeira abertura, offline. */
async function semearProdutos(banco: SQLiteDatabase): Promise<void> {
  await banco.execAsync(`
INSERT OR IGNORE INTO produtos (id, nome, categoria, unidade, preco_base, ativo) VALUES
  (1, 'Tomate italiano', 'Hortifrúti', 'kg', 8.5, 1),
  (2, 'Alface crespa', 'Hortifrúti', 'unidade', 3.2, 1),
  (3, 'Milho verde', 'Grãos', 'kg', 5.4, 1);
`);
}
