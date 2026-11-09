import { useFocusEffect } from '@react-navigation/native';
import { useCallback } from 'react';
import { FlatList, StyleSheet, Text, View } from 'react-native';

import { Card, EmptyState, ErrorState, LoadingState, Screen, StatusBadge } from '../components';
import type { EntregaLocal } from '../db/entregas';
import { useEntregas } from '../hooks/useEntregas';
import type { MainTabScreenProps } from '../navigation/types';
import { statusMeta } from '../theme/status';
import { theme } from '../theme/tokens';
import { typography } from '../theme/typography';
import { formatarData } from '../utils/format';

/** Minhas entregas: estado local, atualização pendente e sincronização sob demanda. */
export function EntregasScreen({ navigation }: MainTabScreenProps<'Entregas'>) {
  const { data, error, isLoading, offline, refetch, sincronizar, sincronizando } =
    useEntregas();

  useFocusEffect(
    useCallback(() => {
      void sincronizar();
    }, [sincronizar]),
  );

  if (isLoading) {
    return (
      <Screen>
        <LoadingState message="Carregando entregas…" />
      </Screen>
    );
  }

  if (error) {
    return (
      <Screen>
        <ErrorState
          title="Não foi possível ler as entregas"
          message="O banco local não respondeu. Tente novamente."
          onRetry={() => {
            void refetch();
          }}
        />
      </Screen>
    );
  }

  const entregas = data ?? [];

  return (
    <Screen padded={false}>
      <View style={styles.cabecalho}>
        <Text style={typography.titulo} accessibilityRole="header">
          Minhas entregas
        </Text>
        <Text style={styles.descricao}>
          Atualize o status mesmo sem internet; a lista reflete o estado local.
        </Text>
        {offline ? (
          <Text style={styles.avisoOffline} accessibilityLiveRegion="polite">
            Sem conexão. As alterações pendentes serão enviadas ao reconectar.
          </Text>
        ) : null}
      </View>

      <FlatList
        data={entregas}
        keyExtractor={(item) => String(item.id)}
        contentContainerStyle={styles.lista}
        refreshing={sincronizando}
        onRefresh={() => {
          void sincronizar();
        }}
        ListEmptyComponent={
          <EmptyState
            title="Nenhuma entrega atribuída"
            description="As entregas aparecem aqui após a sincronização com a API."
          />
        }
        renderItem={({ item }) => <ItemEntrega item={item} navigation={navigation} />}
      />
    </Screen>
  );
}

type ItemEntregaProps = {
  item: EntregaLocal;
  navigation: MainTabScreenProps<'Entregas'>['navigation'];
};

function ItemEntrega({ item, navigation }: ItemEntregaProps) {
  const situacao = item.pendente ? ', pendente de envio' : '';

  return (
    <Card
      onPress={() => navigation.navigate('EntregaDetalhe', { entregaId: item.id })}
      accessibilityLabel={`Pedido ${item.pedidoId}, prevista para ${formatarData(
        item.dataPrevista,
      )}, status ${statusMeta[item.status].label}${situacao}`}
      testID={`entrega-${item.id}`}
    >
      <View style={styles.linhaTitulo}>
        <Text style={[typography.secao, styles.pedido]}>Pedido {item.pedidoId}</Text>
        <StatusBadge status={item.status} />
      </View>
      <Text style={styles.detalhe}>Prevista: {formatarData(item.dataPrevista)}</Text>
      {item.pendente ? <Text style={styles.pendente}>Pendente de envio</Text> : null}
    </Card>
  );
}

const styles = StyleSheet.create({
  cabecalho: {
    paddingHorizontal: theme.spacing.md,
    paddingTop: theme.spacing.md,
    gap: theme.spacing.xs,
  },
  descricao: {
    ...typography.corpoMuted,
  },
  avisoOffline: {
    ...typography.legenda,
    color: theme.colors.warning,
  },
  lista: {
    padding: theme.spacing.md,
    gap: theme.spacing.md,
  },
  linhaTitulo: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    gap: theme.spacing.sm,
  },
  pedido: {
    flexShrink: 1,
  },
  detalhe: {
    ...typography.corpoMuted,
  },
  pendente: {
    ...typography.legenda,
    color: theme.colors.warning,
    fontWeight: theme.fontWeight.bold,
  },
});
