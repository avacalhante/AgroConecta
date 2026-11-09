import { useState } from 'react';
import { StyleSheet, Text, View } from 'react-native';

import { Button, Field, Screen } from '../components';
import { theme } from '../theme/tokens';
import { typography } from '../theme/typography';

type LoginScreenProps = {
  onEntrar: () => void;
};

/**
 * Tela de login provisória da fundação (B1).
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
        <Text style={typography.corpoMuted}>
          Gestão da cadeia de suprimentos e inclusão digital na agricultura familiar.
        </Text>
      </View>

      <View style={styles.formulario}>
        <Field
          label="E-mail"
          value={email}
          onChangeText={setEmail}
          autoCapitalize="none"
          autoComplete="email"
          keyboardType="email-address"
          placeholder="voce@agroconecta.local"
          testID="campo-email"
        />

        <Field
          label="Senha"
          value={senha}
          onChangeText={setSenha}
          secureTextEntry
          autoComplete="password"
          placeholder="Mínimo de 8 caracteres"
          testID="campo-senha"
        />

        <Button
          title="Entrar"
          onPress={onEntrar}
          disabled={!podeEntrar}
          style={styles.botao}
          testID="botao-entrar"
        />

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
    gap: theme.spacing.sm,
  },
  titulo: {
    ...typography.titulo,
    color: theme.colors.primaryDark,
  },
  formulario: {
    gap: theme.spacing.md,
  },
  botao: {
    marginTop: theme.spacing.sm,
  },
  aviso: {
    ...typography.legenda,
  },
});
