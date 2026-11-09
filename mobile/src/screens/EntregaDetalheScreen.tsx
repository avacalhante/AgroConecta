import { StyleSheet, Text, View } from 'react-native';

import { Button, Card, ErrorState, InfoRow, Screen, StatusBadge } from '../components';
import { buscarEntrega } from '../mock/dados';
import type { MainStackScreenProps } from '../navigation/types';
import { theme } from '../theme/tokens';
import { typography } from '../theme/typography';
import { formatarData } from '../utils/format';

/** Detalhe da entrega com dados de exemplo; o fluxo real entra nas tarefas B5 e B7. */
export function EntregaDetalheScreen({
  route,
  navigation,
}: MainStackScreenProps<'EntregaDetalhe'>) {
  const entrega = buscarEntrega(route.params.entregaId);

  if (!entrega) {
    return (
      <Screen scrollable>
        <ErrorState
          title="Entrega não encontrada"
          message="A entrega pode ter sido removida na última sincronização."
          onRetry={() => navigation.goBack()}
          retryLabel="Voltar"
        />
      </Screen>
    );
  }

  return (
    <Screen scrollable>
      <View style={styles.cabecalho}>
        <Text style={[typography.titulo, styles.titulo]}>Pedido {entrega.pedidoId}</Text>
        <StatusBadge status={entrega.status} />
      </View>

      <Card title="Dados da entrega">
        <InfoRow label="Destino" value={entrega.destino} />
        <InfoRow label="Data prevista" value={formatarData(entrega.dataPrevista)} />
        {entrega.observacao ? (
          <InfoRow label="Observação" value={entrega.observacao} />
        ) : null}
      </Card>

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

      <Text style={styles.aviso}>
        Dados de exemplo. As ações são demonstrativas até as tarefas B5 e B7.
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
  acao: {
    marginTop: theme.spacing.sm,
  },
  aviso: {
    ...typography.legenda,
    marginTop: theme.spacing.md,
  },
});
