import { useQuery } from '@tanstack/react-query';
import { StyleSheet, Text, View } from 'react-native';

import { Button, Card, ErrorState, InfoRow, LoadingState, Screen, StatusBadge } from '../components';
import { buscarEntregaLocal } from '../db/entregas';
import type { MainStackScreenProps } from '../navigation/types';
import { theme } from '../theme/tokens';
import { typography } from '../theme/typography';
import { formatarData, formatarDataHora } from '../utils/format';

/** Detalhe da entrega com acesso ao status e à evidência. */
export function EntregaDetalheScreen({
  route,
  navigation,
}: MainStackScreenProps<'EntregaDetalhe'>) {
  const { entregaId } = route.params;
  const consulta = useQuery({
    queryKey: ['entrega-local', entregaId],
    queryFn: () => buscarEntregaLocal(entregaId),
  });

  if (consulta.isLoading) {
    return (
      <Screen>
        <LoadingState message="Carregando entrega…" />
      </Screen>
    );
  }

  const entrega = consulta.data;

  if (!entrega) {
    return (
      <Screen>
        <ErrorState
          title="Entrega não encontrada"
          message="A entrega pode ter sido removida na última sincronização."
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
          Pedido {entrega.pedidoId}
        </Text>
        <StatusBadge status={entrega.status} />
      </View>

      <Card title="Dados da entrega">
        <InfoRow label="Data prevista" value={formatarData(entrega.dataPrevista)} />
        {entrega.dataRealizada ? (
          <InfoRow label="Realizada em" value={formatarDataHora(entrega.dataRealizada)} />
        ) : null}
        {entrega.observacao ? (
          <InfoRow label="Observação" value={entrega.observacao} />
        ) : null}
        {entrega.latitude !== null && entrega.longitude !== null ? (
          <InfoRow
            label="Local"
            value={`${entrega.latitude.toFixed(5)}, ${entrega.longitude.toFixed(5)}`}
          />
        ) : null}
      </Card>

      {entrega.pendente ? (
        <Text style={styles.pendente} accessibilityLiveRegion="polite">
          Alteração salva no aparelho. O envio acontece quando houver conexão.
        </Text>
      ) : null}

      {entrega.erroSincronizacao ? (
        <Text style={styles.erro} accessibilityRole="alert">
          Falha ao sincronizar: {entrega.erroSincronizacao}
        </Text>
      ) : null}

      {entrega.evidenciaPendente ? (
        <Text style={styles.pendente} accessibilityLiveRegion="polite">
          Evidência salva no aparelho. O envio acontece quando houver conexão.
        </Text>
      ) : null}

      {entrega.evidenciaErro ? (
        <Text style={styles.erro} accessibilityRole="alert">
          Falha na evidência: {entrega.evidenciaErro}
        </Text>
      ) : null}

      <Button
        title="Alterar status"
        onPress={() => navigation.navigate('EntregaStatus', { entregaId: entrega.id })}
        style={styles.acao}
        testID="botao-alterar-status"
      />
      <Button
        title="Registrar evidência"
        variant="secondary"
        onPress={() => navigation.navigate('EntregaEvidencia', { entregaId: entrega.id })}
        style={styles.acao}
        testID="botao-registrar-evidencia"
      />
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
  pendente: {
    ...typography.legenda,
    color: theme.colors.warning,
    marginTop: theme.spacing.md,
  },
  erro: {
    ...typography.corpo,
    color: theme.colors.danger,
    marginTop: theme.spacing.md,
  },
  acao: {
    marginTop: theme.spacing.sm,
  },
});
