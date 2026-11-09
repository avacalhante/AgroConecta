import { StyleSheet, Text } from 'react-native';

import { useAuth } from '../auth/AuthContext';
import { Button, Card, InfoRow, Screen } from '../components';
import { useSincronizacao } from '../sync/SincronizacaoProvider';
import { theme } from '../theme/tokens';
import { typography } from '../theme/typography';
import type { Papel } from '../types/dominio';
import { formatarDataHora } from '../utils/format';

const ROTULOS_PAPEL: Record<Papel, string> = {
  Admin: 'Administrador',
  Produtor: 'Produtor',
  Comprador: 'Comprador',
  Motorista: 'Motorista',
};

/** Perfil do usuário autenticado e painel de sincronização offline. */
export function PerfilScreen() {
  const { sair, saindo, usuario } = useAuth();
  const {
    conflitos,
    online,
    pendentes,
    sincronizando,
    ultimaSincronizacao,
    ultimoResultado,
    sincronizarAgora,
  } = useSincronizacao();

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
        <InfoRow
          label="Conexão"
          value={online === null ? 'Verificando…' : online ? 'Online' : 'Offline'}
        />
        <InfoRow label="Operações na fila" value={String(pendentes)} />
        <InfoRow label="Conflitos" value={String(conflitos)} />
        <InfoRow
          label="Última sincronização"
          value={
            ultimaSincronizacao
              ? formatarDataHora(ultimaSincronizacao.toISOString())
              : '—'
          }
        />

        {ultimoResultado && ultimoResultado.falhas > 0 ? (
          <Text style={styles.aviso} accessibilityLiveRegion="polite">
            A última sincronização teve {ultimoResultado.falhas} falha(s). Verifique os
            lotes e entregas com aviso.
          </Text>
        ) : null}

        <Button
          title="Sincronizar agora"
          variant="secondary"
          onPress={() => {
            void sincronizarAgora();
          }}
          loading={sincronizando}
          disabled={sincronizando}
          style={styles.botaoSincronizar}
          testID="botao-sincronizar"
        />
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
  aviso: {
    ...typography.legenda,
    color: theme.colors.warning,
    marginTop: theme.spacing.sm,
  },
  botaoSincronizar: {
    marginTop: theme.spacing.md,
  },
  botaoSair: {
    marginTop: theme.spacing.md,
  },
});
