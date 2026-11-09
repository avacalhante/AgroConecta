import { FlatList, StyleSheet, Text, View } from 'react-native';

import { Card, Screen, StatusBadge } from '../components';
import { ENTREGAS_EXEMPLO, type EntregaExemplo } from '../mock/dados';
import type { MainTabScreenProps } from '../navigation/types';
import { statusMeta } from '../theme/status';
import { theme } from '../theme/tokens';
import { typography } from '../theme/typography';
import { formatarData } from '../utils/format';

/** Lista provisória de entregas do motorista; fluxo real nas tarefas B5 e B7. */
export function EntregasScreen({ navigation }: MainTabScreenProps<'Entregas'>) {
  return (
    <Screen padded={false}>
      <View style={styles.cabecalho}>
        <Text style={typography.titulo}>Minhas entregas</Text>
        <Text style={styles.descricao}>Dados de exemplo até a integração da API.</Text>
      </View>

      <FlatList
        data={ENTREGAS_EXEMPLO}
        keyExtractor={(item) => String(item.id)}
        contentContainerStyle={styles.lista}
        renderItem={({ item }) => <ItemEntrega item={item} navigation={navigation} />}
      />
    </Screen>
  );
}

type ItemEntregaProps = {
  item: EntregaExemplo;
  navigation: MainTabScreenProps<'Entregas'>['navigation'];
};

function ItemEntrega({ item, navigation }: ItemEntregaProps) {
  return (
    <Card
      onPress={() => navigation.navigate('EntregaDetalhe', { entregaId: item.id })}
      accessibilityLabel={`Pedido ${item.pedidoId}, destino ${item.destino}, prevista para ${formatarData(
        item.dataPrevista,
      )}, status ${statusMeta[item.status].label}`}
      testID={`entrega-${item.id}`}
    >
      <View style={styles.linhaTitulo}>
        <Text style={[typography.secao, styles.pedido]}>Pedido {item.pedidoId}</Text>
        <StatusBadge status={item.status} />
      </View>
      <Text style={styles.detalhe}>Destino: {item.destino}</Text>
      <Text style={styles.detalhe}>Prevista: {formatarData(item.dataPrevista)}</Text>
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
});
