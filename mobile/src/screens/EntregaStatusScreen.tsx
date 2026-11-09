import { useState } from 'react';
import { Pressable, StyleSheet, Text, View } from 'react-native';

import { Button, Card, EmptyState, ErrorState, Screen, StatusBadge } from '../components';
import { buscarEntrega } from '../mock/dados';
import type { MainStackScreenProps } from '../navigation/types';
import { statusMeta } from '../theme/status';
import { theme } from '../theme/tokens';
import { typography } from '../theme/typography';
import type { EntregaStatus } from '../types/dominio';

/** Transições permitidas em `contracts/enums.md` para `EntregaStatus`. */
const proximosStatusEntrega: Record<EntregaStatus, EntregaStatus[]> = {
  Pendente: ['EmRota', 'Cancelada'],
  EmRota: ['Entregue', 'Falha'],
  Entregue: [],
  Falha: [],
  Cancelada: [],
};

/** Alteração de status mockada; gravação e sincronização entram na tarefa B5. */
export function EntregaStatusScreen({ route, navigation }: MainStackScreenProps<'EntregaStatus'>) {
  const [selecionado, setSelecionado] = useState<EntregaStatus | null>(null);
  const entrega = buscarEntrega(route.params.entregaId);

  if (!entrega) {
    return (
      <Screen scrollable>
        <ErrorState
          title="Entrega não encontrada"
          message="A entrega pode ter sido removida na última sincronização."
          onRetry={() => navigation.goBack()}
          retryLabel="Voltar"
        />
      </Screen>
    );
  }

  const opcoes = proximosStatusEntrega[entrega.status];

  return (
    <Screen scrollable>
      <Text style={[typography.titulo, styles.titulo]}>Atualizar status</Text>

      <Card title="Situação atual">
        <View style={styles.linhaStatus}>
          <StatusBadge status={entrega.status} />
          <Text style={typography.corpoMuted}>Pedido {entrega.pedidoId}</Text>
        </View>
      </Card>

      {opcoes.length === 0 ? (
        <EmptyState
          title="Entrega finalizada"
          description="Não há nova transição de status disponível para esta entrega."
          actionLabel="Voltar ao detalhe"
          onAction={() => navigation.goBack()}
        />
      ) : (
        <>
          <Text style={[typography.secao, styles.secao]}>Escolha o novo status</Text>
          <Card>
            {opcoes.map((status) => {
              const meta = statusMeta[status];
              const marcado = selecionado === status;

              return (
                <Pressable
                  key={status}
                  accessibilityRole="radio"
                  accessibilityState={{ checked: marcado }}
                  accessibilityLabel={meta.label}
                  onPress={() => setSelecionado(status)}
                  testID={`status-${status}`}
                  style={({ pressed }) => [
                    styles.opcao,
                    marcado && styles.opcaoMarcada,
                    pressed && styles.opcaoPressionada,
                  ]}
                >
                  <View style={[styles.marcador, marcado && styles.marcadorAtivo]} />
                  <Text style={typography.corpo}>{meta.label}</Text>
                </Pressable>
              );
            })}
          </Card>

          <Button
            title="Confirmar status"
            disabled={selecionado === null}
            onPress={() => navigation.goBack()}
            style={styles.acao}
            testID="botao-confirmar-status"
          />
        </>
      )}

      <Text style={styles.aviso}>
        Demonstração da B1: o status ainda não é gravado. Gravação local e sincronização entram
        na tarefa B5.
      </Text>
    </Screen>
  );
}

const styles = StyleSheet.create({
  titulo: {
    marginBottom: theme.spacing.md,
  },
  linhaStatus: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    gap: theme.spacing.sm,
  },
  secao: {
    marginTop: theme.spacing.lg,
    marginBottom: theme.spacing.sm,
  },
  opcao: {
    minHeight: theme.touchTarget,
    flexDirection: 'row',
    alignItems: 'center',
    gap: theme.spacing.sm,
    borderWidth: 1,
    borderColor: theme.colors.border,
    borderRadius: theme.radius.md,
    paddingHorizontal: theme.spacing.md,
    paddingVertical: theme.spacing.sm,
  },
  opcaoMarcada: {
    borderColor: theme.colors.primary,
    backgroundColor: theme.colors.primaryLight,
  },
  opcaoPressionada: {
    backgroundColor: theme.colors.surfaceMuted,
  },
  marcador: {
    width: 20,
    height: 20,
    borderRadius: 10,
    borderWidth: 2,
    borderColor: theme.colors.borderStrong,
  },
  marcadorAtivo: {
    borderColor: theme.colors.primary,
    backgroundColor: theme.colors.primary,
  },
  acao: {
    marginTop: theme.spacing.md,
  },
  aviso: {
    ...typography.legenda,
    marginTop: theme.spacing.md,
  },
});
