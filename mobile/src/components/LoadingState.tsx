import { ActivityIndicator, StyleSheet, Text, View } from 'react-native';

import { colors, spacing } from '../theme/tokens';
import { typography } from '../theme/typography';

export type LoadingStateProps = {
  message?: string;
};

/** Indicador de carregamento com rótulo para leitores de tela. */
export function LoadingState({ message = 'Carregando…' }: LoadingStateProps) {
  return (
    <View
      accessible
      accessibilityRole="progressbar"
      accessibilityLabel={message}
      style={styles.container}
      testID="estado-carregando"
    >
      <ActivityIndicator
        size="large"
        color={colors.primary}
        accessibilityElementsHidden
        importantForAccessibility="no"
      />
      <Text style={styles.message}>{message}</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: spacing.xl,
    gap: spacing.md,
  },
  message: {
    ...typography.corpoMuted,
    textAlign: 'center',
  },
});
