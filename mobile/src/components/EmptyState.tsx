import { StyleSheet, Text, View } from 'react-native';

import { spacing } from '../theme/tokens';
import { typography } from '../theme/typography';
import { Button } from './Button';

export type EmptyStateProps = {
  title: string;
  description?: string;
  actionLabel?: string;
  onAction?: () => void;
};

/** Estado vazio reutilizável, com ação opcional. */
export function EmptyState({ title, description, actionLabel, onAction }: EmptyStateProps) {
  return (
    <View style={styles.container}>
      <Text style={styles.title} accessibilityRole="header">
        {title}
      </Text>
      {description ? <Text style={styles.description}>{description}</Text> : null}
      {actionLabel && onAction ? (
        <Button title={actionLabel} onPress={onAction} variant="secondary" style={styles.action} />
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
  },
  title: {
    ...typography.secao,
    textAlign: 'center',
  },
  description: {
    ...typography.corpoMuted,
    textAlign: 'center',
  },
  action: {
    marginTop: spacing.sm,
    alignSelf: 'stretch',
  },
});
