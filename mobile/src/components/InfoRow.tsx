import { StyleSheet, Text, View } from 'react-native';

import { colors, spacing } from '../theme/tokens';
import { typography } from '../theme/typography';

export type InfoRowProps = {
  label: string;
  value: string;
};

/** Linha de detalhe lida como uma única frase pelo leitor de tela. */
export function InfoRow({ label, value }: InfoRowProps) {
  return (
    <View style={styles.row} accessible accessibilityLabel={`${label}: ${value}`}>
      <Text style={styles.label}>{label}</Text>
      <Text style={styles.value}>{value}</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  row: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
    gap: spacing.md,
    paddingVertical: spacing.xs,
    borderBottomWidth: 1,
    borderBottomColor: colors.surfaceMuted,
  },
  label: {
    ...typography.corpoMuted,
    flexShrink: 0,
  },
  value: {
    ...typography.corpo,
    flexShrink: 1,
    textAlign: 'right',
  },
});
