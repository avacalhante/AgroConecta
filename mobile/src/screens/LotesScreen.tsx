import { FlatList, StyleSheet, Text, View } from 'react-native';

import { Card, Screen, StatusBadge } from '../components';
import { LOTES_EXEMPLO, type LoteExemplo } from '../mock/dados';
import type { MainTabScreenProps } from '../navigation/types';
import { statusMeta } from '../theme/status';
import { theme } from '../theme/tokens';
import { typography } from '../theme/typography';
import { formatarData, formatarQuantidade } from '../utils/format';

/** Lista provisória de lotes do produtor; a fonte real entra nas tarefas B4 e B6. */
export function LotesScreen({ navigation }: MainTabScreenProps<'Lotes'>) {
  return (
    <Screen padded={false}>
      <View style={styles.cabecalho}>
        <Text style={typography.titulo}>Meus lotes</Text>
        <Text style={styles.descricao}>Dados de exemplo até a integração da fila offline.</Text>
      </View>

      <FlatList
        data={LOTES_EXEMPLO}
        keyExtractor={(item) => item.id}
        contentContainerStyle={styles.lista}
        renderItem={({ item }) => <ItemLote item={item} navigation={navigation} />}
      />
    </Screen>
  );
}

type ItemLoteProps = {
  item: LoteExemplo;
  navigation: MainTabScreenProps<'Lotes'>['navigation'];
};

function ItemLote({ item, navigation }: ItemLoteProps) {
  return (
    <Card
      onPress={() => navigation.navigate('LoteDetalhe', { loteId: item.id })}
      accessibilityLabel={`${item.produto}, ${formatarQuantidade(
        item.quantidade,
        item.unidade,
      )}, colhido em ${formatarData(item.dataColheita)}, status ${
        statusMeta[item.status].label
      }`}
      testID={`lote-${item.id}`}
    >
      <View style={styles.linhaTitulo}>
        <Text style={[typography.secao, styles.produto]}>{item.produto}</Text>
        <StatusBadge status={item.status} />
      </View>
      <Text style={styles.detalhe}>
        {formatarQuantidade(item.quantidade, item.unidade)} • colheita{' '}
        {formatarData(item.dataColheita)}
      </Text>
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
  produto: {
    flexShrink: 1,
  },
  detalhe: {
    ...typography.corpoMuted,
  },
});
