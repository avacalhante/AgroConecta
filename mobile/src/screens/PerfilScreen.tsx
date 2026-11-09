import { StyleSheet, Text } from 'react-native';

import { Button, Card, Screen } from '../components';
import { theme } from '../theme/tokens';
import { typography } from '../theme/typography';

type PerfilScreenProps = {
  onSair: () => void;
};

/** Perfil provisório; sessão real e sincronização entram nas tarefas B2 e B6. */
export function PerfilScreen({ onSair }: PerfilScreenProps) {
  return (
    <Screen scrollable>
      <Text style={[typography.titulo, styles.titulo]}>Perfil</Text>

      <Card title="Usuário">
        <Text style={typography.secao}>Usuário de desenvolvimento</Text>
        <Text style={styles.detalhe}>Papel: Produtor</Text>
        <Text style={styles.detalhe}>Sessão simulada até a tarefa B2</Text>
      </Card>

      <Card title="Sincronização">
        <Text style={styles.detalhe}>
          Fila offline ainda não implementada (tarefa B6). Nenhum item pendente.
        </Text>
      </Card>

      <Button
        title="Sair"
        variant="danger"
        onPress={onSair}
        style={styles.botaoSair}
        testID="botao-sair"
      />
    </Screen>
  );
}

const styles = StyleSheet.create({
  titulo: {
    marginBottom: theme.spacing.lg,
  },
  detalhe: {
    ...typography.corpoMuted,
  },
  botaoSair: {
    marginTop: theme.spacing.md,
  },
});
