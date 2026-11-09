import { StyleSheet, Text, View } from 'react-native';

import { Screen } from '../components/Screen';
import { theme } from '../theme/tokens';

type IndicadorProps = {
  rotulo: string;
  valor: string;
};

function Indicador({ rotulo, valor }: IndicadorProps) {
  return (
    <View style={styles.cartao} accessible accessibilityLabel={`${rotulo}: ${valor}`}>
      <Text style={styles.valor}>{valor}</Text>
      <Text style={styles.rotuloIndicador}>{rotulo}</Text>
    </View>
  );
}

/** Tela inicial provisória; os indicadores reais chegam com a integração da API. */
export function InicioScreen() {
  return (
    <Screen scrollable>
      <Text style={styles.titulo}>Início</Text>
      <Text style={styles.descricao}>
        Visão rápida da operação. Os números abaixo são ilustrativos até a integração com
        `/api/v1/indicadores`.
      </Text>

      <View style={styles.grade}>
        <Indicador rotulo="Pedidos no mês" valor="12" />
        <Indicador rotulo="Entregas no prazo" valor="92%" />
        <Indicador rotulo="Produtores ativos" valor="7" />
        <Indicador rotulo="Entregas com evidência" valor="85%" />
      </View>
    </Screen>
  );
}

const styles = StyleSheet.create({
  titulo: {
    fontSize: theme.fontSize.headline,
    fontWeight: '700',
    color: theme.colors.text,
  },
  descricao: {
    marginTop: theme.spacing.sm,
    marginBottom: theme.spacing.lg,
    fontSize: theme.fontSize.body,
    color: theme.colors.textMuted,
  },
  grade: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: theme.spacing.md,
  },
  cartao: {
    flexBasis: '45%',
    flexGrow: 1,
    backgroundColor: theme.colors.surface,
    borderRadius: theme.radius.lg,
    borderWidth: 1,
    borderColor: theme.colors.border,
    padding: theme.spacing.md,
  },
  valor: {
    fontSize: theme.fontSize.title,
    fontWeight: '700',
    color: theme.colors.primaryDark,
  },
  rotuloIndicador: {
    marginTop: theme.spacing.xs,
    fontSize: theme.fontSize.caption,
    color: theme.colors.textMuted,
  },
});
