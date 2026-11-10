import { useQuery } from '@tanstack/react-query';
import { StyleSheet, Text, View } from 'react-native';

import { Card, ErrorState, InfoRow, LoadingState, Screen, StatusBadge } from '../components';
import { buscarLoteLocal } from '../db/lotes';
import type { MainStackScreenProps } from '../navigation/types';
import { theme } from '../theme/tokens';
import { typography } from '../theme/typography';
import { formatarData, formatarQuantidade } from '../utils/format';

/** Detalhe do lote salvo localmente, inclusive quando ainda não sincronizou. */
export function LoteDetalheScreen({ route }: MainStackScreenProps<'LoteDetalhe'>) {
  const { codigoRastreio } = route.params;
  const consulta = useQuery({
    queryKey: ['lote-local', codigoRastreio],
    queryFn: () => buscarLoteLocal(codigoRastreio),
  });

  if (consulta.isLoading) {
    return (
      <Screen>
        <LoadingState message="Carregando lote…" />
      </Screen>
    );
  }

  const lote = consulta.data;

  if (!lote) {
    return (
      <Screen>
        <ErrorState
          title="Lote não encontrado"
          message="O lote pode ter sido removido do dispositivo."
          onRetry={() => {
            void consulta.refetch();
          }}
        />
      </Screen>
    );
  }

  return (
    <Screen scrollable>
      <View style={styles.cabecalho}>
        <Text style={[typography.titulo, styles.titulo]} accessibilityRole="header">
          {lote.produtoNome}
        </Text>
        <StatusBadge status={lote.status} />
      </View>

      <Card title="Dados do lote">
        <InfoRow
          label="Quantidade"
          value={formatarQuantidade(lote.quantidade, lote.produtoUnidade)}
        />
        <InfoRow label="Data da colheita" value={formatarData(lote.dataColheita)} />
        <InfoRow label="Código de rastreio" value={lote.codigoRastreio} />
        <InfoRow
          label="Envio"
          value={lote.sincronizado ? 'Sincronizado' : 'Pendente de envio'}
        />
      </Card>

      <Text style={styles.aviso}>
        {lote.sincronizado
          ? 'Este lote já está no servidor.'
          : 'O lote está salvo no aparelho e será enviado quando houver conexão.'}
      </Text>

      {lote.erroSincronizacao ? (
        <Text style={styles.erro} accessibilityRole="alert">
          Falha ao sincronizar: {lote.erroSincronizacao}
        </Text>
      ) : null}
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
  erro: {
    ...typography.corpo,
    color: theme.colors.danger,
    marginTop: theme.spacing.sm,
  },
});
