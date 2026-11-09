import { FlatList, StyleSheet, Text, View } from 'react-native';

import { Screen } from '../components/Screen';
import { theme } from '../theme/tokens';

type EntregaExemplo = {
  id: string;
  pedido: string;
  destino: string;
  prevista: string;
  status: string;
};

const ENTREGAS_EXEMPLO: EntregaExemplo[] = [
  {
    id: '1',
    pedido: 'Pedido 1',
    destino: 'Campinas/SP',
    prevista: '05/11/2026',
    status: 'Em rota',
  },
  {
    id: '2',
    pedido: 'Pedido 2',
    destino: 'Valinhos/SP',
    prevista: '06/11/2026',
    status: 'Pendente',
  },
];

/** Lista provisória de entregas do motorista; fluxo real nas tarefas B5 e B7. */
export function EntregasScreen() {
  return (
    <Screen padded={false}>
      <View style={styles.cabecalho}>
        <Text style={styles.titulo}>Minhas entregas</Text>
        <Text style={styles.descricao}>Dados de exemplo até a integração da API.</Text>
      </View>

      <FlatList
        data={ENTREGAS_EXEMPLO}
        keyExtractor={(item) => item.id}
        contentContainerStyle={styles.lista}
        renderItem={({ item }) => (
          <View
            style={styles.cartao}
            accessible
            accessibilityLabel={`${item.pedido}, destino ${item.destino}, prevista para ${item.prevista}, status ${item.status}`}
          >
            <Text style={styles.pedido}>{item.pedido}</Text>
            <Text style={styles.detalhe}>Destino: {item.destino}</Text>
            <Text style={styles.detalhe}>Prevista: {item.prevista}</Text>
            <Text style={styles.status}>{item.status}</Text>
          </View>
        )}
      />
    </Screen>
  );
}

const styles = StyleSheet.create({
  cabecalho: {
    paddingHorizontal: theme.spacing.md,
    paddingTop: theme.spacing.md,
  },
  titulo: {
    fontSize: theme.fontSize.headline,
    fontWeight: '700',
    color: theme.colors.text,
  },
  descricao: {
    marginTop: theme.spacing.xs,
    fontSize: theme.fontSize.body,
    color: theme.colors.textMuted,
  },
  lista: {
    padding: theme.spacing.md,
    gap: theme.spacing.md,
  },
  cartao: {
    backgroundColor: theme.colors.surface,
    borderRadius: theme.radius.lg,
    borderWidth: 1,
    borderColor: theme.colors.border,
    padding: theme.spacing.md,
  },
  pedido: {
    fontSize: theme.fontSize.bodyLarge,
    fontWeight: '600',
    color: theme.colors.text,
  },
  detalhe: {
    marginTop: theme.spacing.xs,
    fontSize: theme.fontSize.body,
    color: theme.colors.textMuted,
  },
  status: {
    marginTop: theme.spacing.sm,
    fontSize: theme.fontSize.caption,
    fontWeight: '700',
    color: theme.colors.info,
    textTransform: 'uppercase',
  },
});
