import { StyleSheet, Text, View } from 'react-native';

import { useAuth } from '../auth/AuthContext';
import { Button, Card, Screen } from '../components';
import type { MainTabScreenProps } from '../navigation/types';
import { theme } from '../theme/tokens';
import { typography } from '../theme/typography';

type IndicadorProps = {
  rotulo: string;
  valor: string;
};

function Indicador({ rotulo, valor }: IndicadorProps) {
  return (
    <Card style={styles.cartao} accessibilityLabel={`${rotulo}: ${valor}`}>
      <Text style={styles.valor}>{valor}</Text>
      <Text style={styles.rotuloIndicador}>{rotulo}</Text>
    </Card>
  );
}

/** Tela inicial provisória; os indicadores reais chegam com a integração da API. */
export function InicioScreen({ navigation }: MainTabScreenProps<'Inicio'>) {
  const { usuario } = useAuth();
  const mostrarLotes = usuario?.papel === 'Produtor' || usuario?.papel === 'Admin';
  const mostrarEntregas = usuario?.papel === 'Motorista' || usuario?.papel === 'Admin';

  return (
    <Screen scrollable>
      <Text style={typography.titulo} accessibilityRole="header">
        Início
      </Text>
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

      {mostrarLotes || mostrarEntregas ? (
        <>
          <Text
            style={[typography.secao, styles.secaoTitulo]}
            accessibilityRole="header"
          >
            Ações rápidas
          </Text>
          <View style={styles.acoes}>
            {mostrarLotes ? (
              <Button
                title="Ver meus lotes"
                variant="secondary"
                onPress={() => navigation.navigate('Lotes')}
              />
            ) : null}
            {mostrarEntregas ? (
              <Button
                title="Ver minhas entregas"
                variant="secondary"
                onPress={() => navigation.navigate('Entregas')}
              />
            ) : null}
          </View>
        </>
      ) : null}
    </Screen>
  );
}

const styles = StyleSheet.create({
  descricao: {
    ...typography.corpoMuted,
    marginTop: theme.spacing.sm,
    marginBottom: theme.spacing.lg,
  },
  grade: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: theme.spacing.md,
  },
  cartao: {
    flexBasis: '45%',
    flexGrow: 1,
  },
  valor: {
    fontSize: theme.fontSize.title,
    fontWeight: theme.fontWeight.bold,
    color: theme.colors.primaryDark,
  },
  rotuloIndicador: {
    ...typography.legenda,
  },
  secaoTitulo: {
    marginTop: theme.spacing.xl,
    marginBottom: theme.spacing.sm,
  },
  acoes: {
    gap: theme.spacing.sm,
  },
});
