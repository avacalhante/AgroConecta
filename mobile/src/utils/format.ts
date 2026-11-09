const dataFormatter = new Intl.DateTimeFormat('pt-BR', {
  day: '2-digit',
  month: '2-digit',
  year: 'numeric',
});

const dataHoraFormatter = new Intl.DateTimeFormat('pt-BR', {
  day: '2-digit',
  month: '2-digit',
  year: 'numeric',
  hour: '2-digit',
  minute: '2-digit',
});

/** Formata uma data ISO (AAAA-MM-DD ou ISO completa) para dd/mm/aaaa. */
export function formatarData(iso: string): string {
  const data = interpretarData(iso);
  if (!data) {
    return '—';
  }
  return dataFormatter.format(data);
}

/**
 * Datas sem hora (AAAA-MM-DD) são de negócio e devem ser lidas no fuso local.
 * `new Date('2026-11-01')` seria meia-noite UTC e poderia cair no dia anterior.
 */
function interpretarData(iso: string): Date | null {
  const somenteData = /^(\d{4})-(\d{2})-(\d{2})$/.exec(iso);
  if (somenteData) {
    const [, ano, mes, dia] = somenteData;
    const data = new Date(Number(ano), Number(mes) - 1, Number(dia));
    return Number.isNaN(data.getTime()) ? null : data;
  }

  const data = new Date(iso);
  return Number.isNaN(data.getTime()) ? null : data;
}

/** Formata uma data ISO completa para dd/mm/aaaa hh:mm. */
export function formatarDataHora(iso: string): string {
  const data = new Date(iso);
  if (Number.isNaN(data.getTime())) {
    return '—';
  }
  return dataHoraFormatter.format(data);
}

/** Formata quantidade com no máximo três casas decimais, no padrão brasileiro. */
export function formatarQuantidade(valor: number, unidade = 'kg'): string {
  if (!Number.isFinite(valor)) {
    return '—';
  }
  const numero = new Intl.NumberFormat('pt-BR', {
    maximumFractionDigits: 3,
  }).format(valor);
  return `${numero} ${unidade}`;
}

/** Formata valor monetário em reais. */
export function formatarMoeda(valor: number): string {
  if (!Number.isFinite(valor)) {
    return '—';
  }
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL',
  }).format(valor);
}
