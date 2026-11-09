import { useState } from 'react';
import { Pressable, StyleSheet, Text, TextInput, View } from 'react-native';

import { Screen } from '../components/Screen';
import { theme } from '../theme/tokens';

type LoginScreenProps = {
  onEntrar: () => void;
};

/**
 * Tela de login provisória da fundação (B0).
 * A autenticação real, com tokens e armazenamento seguro, entra na tarefa B2.
 */
export function LoginScreen({ onEntrar }: LoginScreenProps) {
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const podeEntrar = email.trim().length > 0 && senha.length >= 8;

  return (
    <Screen scrollable>
      <View style={styles.cabecalho}>
        <Text style={styles.titulo}>AgroConecta</Text>
        <Text style={styles.subtitulo}>
          Gestão da cadeia de suprimentos e inclusão digital na agricultura familiar.
        </Text>
      </View>

      <View style={styles.formulario}>
        <Text style={styles.rotulo}>E-mail</Text>
        <TextInput
          style={styles.campo}
          value={email}
          onChangeText={setEmail}
          autoCapitalize="none"
          autoComplete="email"
          keyboardType="email-address"
          placeholder="voce@agroconecta.local"
          accessibilityLabel="E-mail"
          testID="campo-email"
        />

        <Text style={styles.rotulo}>Senha</Text>
        <TextInput
          style={styles.campo}
          value={senha}
          onChangeText={setSenha}
          secureTextEntry
          autoComplete="password"
          placeholder="Mínimo de 8 caracteres"
          accessibilityLabel="Senha"
          testID="campo-senha"
        />

        <Pressable
          accessibilityRole="button"
          accessibilityState={{ disabled: !podeEntrar }}
          disabled={!podeEntrar}
          onPress={onEntrar}
          style={({ pressed }) => [
            styles.botao,
            !podeEntrar && styles.botaoDesabilitado,
            pressed && podeEntrar && styles.botaoPressionado,
          ]}
          testID="botao-entrar"
        >
          <Text style={styles.textoBotao}>Entrar</Text>
        </Pressable>

        <Text style={styles.aviso}>
          Ambiente de desenvolvimento. Use um usuário do seed em `contracts/seed.md`.
        </Text>
      </View>
    </Screen>
  );
}

const styles = StyleSheet.create({
  cabecalho: {
    marginTop: theme.spacing.xl,
    marginBottom: theme.spacing.lg,
  },
  titulo: {
    fontSize: theme.fontSize.headline,
    fontWeight: '700',
    color: theme.colors.primaryDark,
  },
  subtitulo: {
    marginTop: theme.spacing.sm,
    fontSize: theme.fontSize.body,
    color: theme.colors.textMuted,
  },
  formulario: {
    gap: theme.spacing.sm,
  },
  rotulo: {
    marginTop: theme.spacing.sm,
    fontSize: theme.fontSize.body,
    fontWeight: '600',
    color: theme.colors.text,
  },
  campo: {
    minHeight: theme.touchTarget,
    borderWidth: 1,
    borderColor: theme.colors.border,
    borderRadius: theme.radius.md,
    paddingHorizontal: theme.spacing.md,
    fontSize: theme.fontSize.body,
    backgroundColor: theme.colors.surface,
    color: theme.colors.text,
  },
  botao: {
    marginTop: theme.spacing.lg,
    minHeight: theme.touchTarget,
    borderRadius: theme.radius.md,
    backgroundColor: theme.colors.primary,
    alignItems: 'center',
    justifyContent: 'center',
  },
  botaoDesabilitado: {
    backgroundColor: theme.colors.border,
  },
  botaoPressionado: {
    backgroundColor: theme.colors.primaryDark,
  },
  textoBotao: {
    color: theme.colors.textInverse,
    fontSize: theme.fontSize.bodyLarge,
    fontWeight: '700',
  },
  aviso: {
    marginTop: theme.spacing.md,
    fontSize: theme.fontSize.caption,
    color: theme.colors.textMuted,
  },
});
