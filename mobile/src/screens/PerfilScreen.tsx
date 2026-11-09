import { StyleSheet, Text } from 'react-native';

import { useAuth } from '../auth/AuthContext';
import { Button, Card, InfoRow, Screen } from '../components';
import { theme } from '../theme/tokens';
import { typography } from '../theme/typography';
import type { Papel } from '../types/dominio';

const ROTULOS_PAPEL: Record<Papel, string> = {
  Admin: 'Administrador',
  Produtor: 'Produtor',
  Comprador: 'Comprador',
  Motorista: 'Motorista',
};

/** Perfil do usuário autenticado; a fila offline entra na tarefa B6. */
export function PerfilScreen() {
  const { sair, saindo, usuario } = useAuth();

  if (!usuario) {
    return null;
  }

  return (
    <Screen scrollable>
      <Text style={[typography.titulo, styles.titulo]}>Perfil</Text>

      <Card title="Usuário">
        <Text style={typography.secao}>{usuario.nome}</Text>
        <InfoRow label="E-mail" value={usuario.email} />
        <InfoRow label="Papel" value={ROTULOS_PAPEL[usuario.papel]} />
      </Card>

      <Card title="Sincronização">
        <Text style={styles.detalhe}>
          Fila offline ainda não implementada (tarefa B6). Nenhum item pendente.
        </Text>
      </Card>

      <Button
        title={saindo ? 'Saindo…' : 'Sair'}
        variant="danger"
        onPress={() => {
          void sair();
        }}
        loading={saindo}
        disabled={saindo}
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
