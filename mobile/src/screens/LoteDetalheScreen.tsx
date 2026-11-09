import { StyleSheet, Text, View } from 'react-native';

import { Card, ErrorState, InfoRow, Screen, StatusBadge } from '../components';
import { buscarLote } from '../mock/dados';
import type { MainStackScreenProps } from '../navigation/types';
import { theme } from '../theme/tokens';
import { typography } from '../theme/typography';
import { formatarData, formatarQuantidade } from '../utils/format';

/** Detalhe do lote com dados de exemplo; a fonte real entra na tarefa B4. */
export function LoteDetalheScreen({ route, navigation }: MainStackScreenProps<'LoteDetalhe'>) {
  const lote = buscarLote(route.params.loteId);

  if (!lote) {
    return (
      <Screen scrollable>
        <ErrorState
          title="Lote não encontrado"
          message="O lote pode ter sido removido ou ainda não foi sincronizado."
          onRetry={() => navigation.goBack()}
          retryLabel="Voltar"
        />
      </Screen>
    );
  }

  return (
    <Screen scrollable>
      <View style={styles.cabecalho}>
        <Text style={[typography.titulo, styles.titulo]}>{lote.produto}</Text>
        <StatusBadge status={lote.status} />
      </View>

      <Card title="Dados do lote">
        <InfoRow
          label="Quantidade"
          value={formatarQuantidade(lote.quantidade, lote.unidade)}
        />
        <InfoRow label="Data da colheita" value={formatarData(lote.dataColheita)} />
        <InfoRow label="Código de rastreio" value={lote.codigoRastreio} />
      </Card>

      <Text style={styles.aviso}>
        Dados de exemplo. A edição e a sincronização do lote entram na tarefa B4.
      </Text>
    </Screen>
  );
}

const styles = StyleSheet.create({
  cabecalho: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    gap: theme.spacing.sm,
    marginBottom: theme.spacing.md,
  },
  titulo: {
    flexShrink: 1,
  },
  aviso: {
    ...typography.legenda,
    marginTop: theme.spacing.md,
  },
});
