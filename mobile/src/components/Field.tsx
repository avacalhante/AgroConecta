import { StyleSheet, Text, TextInput, View, type TextInputProps } from 'react-native';

import { colors, fontSize, fontWeight, radius, spacing, touchTarget } from '../theme/tokens';
import { typography } from '../theme/typography';

export type FieldProps = TextInputProps & {
  label: string;
  error?: string;
  hint?: string;
  required?: boolean;
};

/**
 * Campo de texto com rótulo visível e acessível.
 * O rótulo é lido pelo leitor de tela mesmo sem o texto de apoio.
 */
export function Field({
  label,
  error,
  hint,
  required = false,
  style,
  accessibilityLabel,
  ...props
}: FieldProps) {
  const apoio = error ?? hint;

  return (
    <View style={styles.container}>
      <Text style={styles.label}>
        {label}
        {required ? ' *' : ''}
      </Text>
      <TextInput
        {...props}
        accessibilityLabel={accessibilityLabel ?? label}
        accessibilityHint={apoio}
        placeholderTextColor={colors.textMuted}
        style={[styles.input, Boolean(error) && styles.inputError, style]}
      />
      {apoio ? (
        <Text
          style={[styles.support, Boolean(error) && styles.error]}
          accessibilityLiveRegion="polite"
        >
          {apoio}
        </Text>
      ) : null}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    gap: spacing.xs,
  },
  label: {
    fontSize: fontSize.body,
    fontWeight: fontWeight.semiBold,
    color: colors.text,
  },
  input: {
    minHeight: touchTarget,
    borderWidth: 1,
    borderColor: colors.border,
    borderRadius: radius.md,
    paddingHorizontal: spacing.md,
    paddingVertical: spacing.sm,
    fontSize: fontSize.body,
    backgroundColor: colors.surface,
    color: colors.text,
  },
  inputError: {
    borderColor: colors.danger,
  },
  support: {
    ...typography.legenda,
  },
  error: {
    color: colors.danger,
  },
});
