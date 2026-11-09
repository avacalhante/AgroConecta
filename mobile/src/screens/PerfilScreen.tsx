import { Pressable, StyleSheet, Text, View } from 'react-native';

import { Screen } from '../components/Screen';
import { theme } from '../theme/tokens';

type PerfilScreenProps = {
  onSair: () => void;
};

/** Perfil provisório; sessão real e sincronização entram nas tarefas B2 e B6. */
export function PerfilScreen({ onSair }: PerfilScreenProps) {
  return (
    <Screen scrollable>
      <Text style={styles.titulo}>Perfil</Text>

      <View style={styles.cartao}>
        <Text style={styles.nome}>Usuário de desenvolvimento</Text>
        <Text style={styles.detalhe}>Papel: Produtor</Text>
        <Text style={styles.detalhe}>Sessão simulada até a tarefa B2</Text>
      </View>

      <View style={styles.cartao}>
        <Text style={styles.secao}>Sincronização</Text>
        <Text style={styles.detalhe}>
          Fila offline ainda não implementada (tarefa B6). Nenhum item pendente.
        </Text>
      </View>

      <Pressable
        accessibilityRole="button"
        onPress={onSair}
        style={({ pressed }) => [styles.botaoSair, pressed && styles.botaoPressionado]}
        testID="botao-sair"
      >
        <Text style={styles.textoBotaoSair}>Sair</Text>
      </Pressable>
    </Screen>
  );
}

const styles = StyleSheet.create({
  titulo: {
    fontSize: theme.fontSize.headline,
    fontWeight: '700',
    color: theme.colors.text,
    marginBottom: theme.spacing.lg,
  },
  cartao: {
    backgroundColor: theme.colors.surface,
    borderRadius: theme.radius.lg,
    borderWidth: 1,
    borderColor: theme.colors.border,
    padding: theme.spacing.md,
    marginBottom: theme.spacing.md,
  },
  nome: {
    fontSize: theme.fontSize.bodyLarge,
    fontWeight: '600',
    color: theme.colors.text,
  },
  secao: {
    fontSize: theme.fontSize.body,
    fontWeight: '700',
    color: theme.colors.text,
    marginBottom: theme.spacing.xs,
  },
  detalhe: {
    marginTop: theme.spacing.xs,
    fontSize: theme.fontSize.body,
    color: theme.colors.textMuted,
  },
  botaoSair: {
    marginTop: theme.spacing.md,
    minHeight: theme.touchTarget,
    borderRadius: theme.radius.md,
    borderWidth: 1,
    borderColor: theme.colors.danger,
    alignItems: 'center',
    justifyContent: 'center',
  },
  botaoPressionado: {
    backgroundColor: theme.colors.surfaceMuted,
  },
  textoBotaoSair: {
    color: theme.colors.danger,
    fontSize: theme.fontSize.body,
    fontWeight: '700',
  },
});
