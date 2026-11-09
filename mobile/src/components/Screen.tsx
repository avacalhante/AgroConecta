import type { PropsWithChildren } from 'react';
import { ScrollView, StyleSheet, View, type ViewStyle } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';

import { theme } from '../theme/tokens';

type ScreenProps = PropsWithChildren<{
  /** Permite rolagem vertical. Desative em telas que gerenciam a própria lista. */
  scrollable?: boolean;
  /** Aplica preenchimento interno padrão. */
  padded?: boolean;
  style?: ViewStyle;
}>;

/** Contêiner base das telas: área segura, fundo do tema e rolagem opcional. */
export function Screen({ children, scrollable = false, padded = true, style }: ScreenProps) {
  const content = (
    <View style={[padded && styles.padded, style]}>{children}</View>
  );

  return (
    <SafeAreaView style={styles.safeArea} edges={['top', 'left', 'right']}>
      {scrollable ? (
        <ScrollView
          contentContainerStyle={styles.scrollContent}
          keyboardShouldPersistTaps="handled"
        >
          {content}
        </ScrollView>
      ) : (
        content
      )}
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  safeArea: {
    flex: 1,
    backgroundColor: theme.colors.background,
  },
  scrollContent: {
    flexGrow: 1,
  },
  padded: {
    flex: 1,
    padding: theme.spacing.md,
  },
});
