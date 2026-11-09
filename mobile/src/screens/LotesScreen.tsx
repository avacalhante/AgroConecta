import { FlatList, StyleSheet, Text, View } from 'react-native';

import { Screen } from '../components/Screen';
import { theme } from '../theme/tokens';

type LoteExemplo = {
  id: string;
  produto: string;
  quantidade: string;
  colheita: string;
  status: string;
};

const LOTES_EXEMPLO: LoteExemplo[] = [
  {
    id: '1',
    produto: 'Tomate italiano',
    quantidade: '120 kg',
    colheita: '01/11/2026',
    status: 'Disponível',
  },
  {
    id: '2',
    produto: 'Alface crespa',
    quantidade: '80 un',
    colheita: '02/11/2026',
    status: 'Reservado',
  },
];

/** Lista provisória de lotes do produtor; a fonte real entra nas tarefas B4 e B6. */
export function LotesScreen() {
  return (
    <Screen padded={false}>
      <View style={styles.cabecalho}>
        <Text style={styles.titulo}>Meus lotes</Text>
        <Text style={styles.descricao}>
          Dados de exemplo até a integração da fila offline.
        </Text>
      </View>

      <FlatList
        data={LOTES_EXEMPLO}
        keyExtractor={(item) => item.id}
        contentContainerStyle={styles.lista}
        renderItem={({ item }) => (
          <View
            style={styles.cartao}
            accessible
            accessibilityLabel={`${item.produto}, ${item.quantidade}, colhido em ${item.colheita}, status ${item.status}`}
          >
            <Text style={styles.produto}>{item.produto}</Text>
            <Text style={styles.detalhe}>{item.quantidade} • colheita {item.colheita}</Text>
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
  produto: {
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
    color: theme.colors.primary,
    textTransform: 'uppercase',
  },
});
