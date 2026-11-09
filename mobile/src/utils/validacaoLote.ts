export type EntradaNovoLote = {
  produtoId: number | null;
  dataColheita: string;
  quantidade: string;
};

export type ErrosNovoLote = Partial<Record<'produtoId' | 'dataColheita' | 'quantidade', string>>;

/** Converte texto com vírgula ou ponto em número. */
export function converterQuantidade(texto: string): number {
  return Number(texto.trim().replace(',', '.'));
}

/** Verifica se a data está no formato AAAA-MM-DD e existe no calendário. */
export function dataIsoValida(iso: string): boolean {
  const partes = /^(\d{4})-(\d{2})-(\d{2})$/.exec(iso.trim());
  if (!partes) {
    return false;
  }

  const ano = Number(partes[1]);
  const mes = Number(partes[2]);
  const dia = Number(partes[3]);
  const data = new Date(ano, mes - 1, dia);

  return (
    data.getFullYear() === ano &&
    data.getMonth() === mes - 1 &&
    data.getDate() === dia
  );
}

/** Data de hoje no fuso local, no formato AAAA-MM-DD. */
export function hojeIsoLocal(): string {
  const agora = new Date();
  const mes = String(agora.getMonth() + 1).padStart(2, '0');
  const dia = String(agora.getDate()).padStart(2, '0');
  return `${agora.getFullYear()}-${mes}-${dia}`;
}

export function validarNovoLote(entrada: EntradaNovoLote): ErrosNovoLote {
  const erros: ErrosNovoLote = {};

  if (entrada.produtoId === null) {
    erros.produtoId = 'Selecione um produto.';
  }

  if (!dataIsoValida(entrada.dataColheita)) {
    erros.dataColheita = 'Informe a data no formato AAAA-MM-DD.';
  } else if (entrada.dataColheita > hojeIsoLocal()) {
    erros.dataColheita = 'A data da colheita não pode ser futura.';
  }

  const quantidade = converterQuantidade(entrada.quantidade);
  if (!Number.isFinite(quantidade) || quantidade <= 0) {
    erros.quantidade = 'Informe uma quantidade maior que zero.';
  }

  return erros;
}
