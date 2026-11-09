import { useState } from 'react';
import { StyleSheet, Text, View } from 'react-native';

import { mensagemDeErroLogin } from '../api/erros';
import { useAuth } from '../auth/AuthContext';
import { Button, Field, Screen } from '../components';
import { theme } from '../theme/tokens';
import { typography } from '../theme/typography';

/** Login com e-mail e senha do seed; a sessão é persistida pelo AuthContext. */
export function LoginScreen() {
  const { entrar } = useAuth();
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [erro, setErro] = useState<string | null>(null);
  const [enviando, setEnviando] = useState(false);
  const podeEntrar = email.trim().length > 0 && senha.length >= 8 && !enviando;

  async function submeter() {
    if (!podeEntrar) {
      return;
    }

    setErro(null);
    setEnviando(true);

    try {
      await entrar(email.trim(), senha);
    } catch (falha) {
      setErro(mensagemDeErroLogin(falha));
    } finally {
      setEnviando(false);
    }
  }

  return (
    <Screen scrollable>
      <View style={styles.cabecalho}>
        <Text style={styles.titulo} accessibilityRole="header">
          AgroConecta
        </Text>
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

        {erro ? (
          <Text
            style={styles.erro}
            accessibilityRole="alert"
            accessibilityLiveRegion="polite"
            testID="mensagem-erro"
          >
            {erro}
          </Text>
        ) : null}

        <Button
          title="Entrar"
          onPress={() => {
            void submeter();
          }}
          disabled={!podeEntrar}
          loading={enviando}
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
  erro: {
    ...typography.corpo,
    color: theme.colors.danger,
  },
  aviso: {
    ...typography.legenda,
  },
});
