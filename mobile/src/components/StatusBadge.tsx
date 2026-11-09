import { StyleSheet, Text, View } from 'react-native';

import { statusMeta, statusToneColors } from '../theme/status';
import { fontWeight, radius, spacing } from '../theme/tokens';
import type { StatusValor } from '../types/dominio';

export type StatusBadgeProps = {
  status: StatusValor;
  testID?: string;
};

/** Badge com rótulo em pt-BR e cor semântica para um status do contrato. */
export function StatusBadge({ status, testID }: StatusBadgeProps) {
  const meta = statusMeta[status];
  const tone = statusToneColors[meta.tone];

  return (
    <View
      accessible
      accessibilityLabel={`Status: ${meta.label}`}
      testID={testID}
      style={[styles.badge, { backgroundColor: tone.background, borderColor: tone.border }]}
    >
      <Text style={[styles.texto, { color: tone.text }]}>{meta.label}</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  badge: {
    alignSelf: 'flex-start',
    borderRadius: radius.pill,
    borderWidth: 1,
    paddingHorizontal: spacing.sm,
    paddingVertical: spacing.xs,
  },
  texto: {
    fontSize: 12,
    fontWeight: fontWeight.bold,
  },
});
