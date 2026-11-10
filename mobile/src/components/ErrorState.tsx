import { StyleSheet, Text, View } from 'react-native';

import { colors, radius, spacing } from '../theme/tokens';
import { typography } from '../theme/typography';
import { Button } from './Button';

export type ErrorStateProps = {
  message: string;
  title?: string;
  onRetry?: () => void;
  retryLabel?: string;
};

/**
 * Estado de erro com mensagem em pt-BR e ação opcional.
 * O título usa `alert` para ser anunciado quando surgir na tela.
 */
export function ErrorState({
  message,
  title = 'Não foi possível carregar',
  onRetry,
  retryLabel = 'Tentar novamente',
}: ErrorStateProps) {
  return (
    <View style={styles.container} testID="estado-erro">
      <Text style={styles.title} accessibilityRole="alert" accessibilityLiveRegion="polite">
        {title}
      </Text>
      <Text style={styles.message}>{message}</Text>
      {onRetry ? (
        <Button
          title={retryLabel}
          onPress={onRetry}
          variant="secondary"
          style={styles.action}
          testID="botao-tentar-novamente"
        />
      ) : null}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: spacing.xl,
    paddingHorizontal: spacing.md,
    gap: spacing.sm,
    borderWidth: 1,
    borderColor: colors.dangerLight,
    borderRadius: radius.lg,
    backgroundColor: colors.surface,
  },
  title: {
    ...typography.secao,
    color: colors.danger,
    textAlign: 'center',
  },
  message: {
    ...typography.corpoMuted,
    textAlign: 'center',
  },
  action: {
    marginTop: spacing.sm,
    alignSelf: 'stretch',
  },
});
