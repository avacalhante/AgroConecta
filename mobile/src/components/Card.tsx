import type { PropsWithChildren } from 'react';
import {
  Pressable,
  StyleSheet,
  Text,
  View,
  type StyleProp,
  type ViewStyle,
} from 'react-native';

import { colors, radius, spacing } from '../theme/tokens';
import { typography } from '../theme/typography';

export type CardProps = PropsWithChildren<{
  title?: string;
  onPress?: () => void;
  accessibilityLabel?: string;
  style?: StyleProp<ViewStyle>;
  testID?: string;
}>;

/** Cartão de conteúdo. Com `onPress`, vira um item de lista acessível. */
export function Card({
  children,
  title,
  onPress,
  accessibilityLabel,
  style,
  testID,
}: CardProps) {
  const conteudo = (
    <>
      {title ? (
        <Text style={styles.title} accessibilityRole="header">
          {title}
        </Text>
      ) : null}
      {children}
    </>
  );

  if (onPress) {
    return (
      <Pressable
        accessibilityRole="button"
        accessibilityLabel={accessibilityLabel}
        onPress={onPress}
        testID={testID}
        style={({ pressed }) => [styles.card, pressed && styles.pressed, style]}
      >
        {conteudo}
      </Pressable>
    );
  }

  return (
    <View
      style={[styles.card, style]}
      testID={testID}
      accessible={Boolean(accessibilityLabel)}
      accessibilityLabel={accessibilityLabel}
    >
      {conteudo}
    </View>
  );
}

const styles = StyleSheet.create({
  card: {
    backgroundColor: colors.surface,
    borderRadius: radius.lg,
    borderWidth: 1,
    borderColor: colors.border,
    padding: spacing.md,
    gap: spacing.sm,
  },
  pressed: {
    backgroundColor: colors.surfacePressed,
  },
  title: {
    ...typography.secao,
  },
});
