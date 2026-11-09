import { useQuery, useQueryClient } from '@tanstack/react-query';
import { useState } from 'react';
import { Pressable, StyleSheet, Text, View } from 'react-native';

import {
  Button,
  Card,
  EmptyState,
  ErrorState,
  LoadingState,
  Screen,
  StatusBadge,
} from '../components';
import { buscarEntregaLocal } from '../db/entregas';
import type { MainStackScreenProps } from '../navigation/types';
import { agendarStatusEntrega } from '../sync/entregas';
import { statusMeta } from '../theme/status';
import { theme } from '../theme/tokens';
import { typography } from '../theme/typography';
import type { EntregaStatus } from '../types/dominio';
import { proximosStatusEntrega } from '../utils/entregas';

/** Alteração de status local-first, enviada depois como PATCH idempotente. */
export function EntregaStatusScreen({ route, navigation }: MainStackScreenProps<'EntregaStatus'>) {
  const { entregaId } = route.params;
  const queryClient = useQueryClient();
  const consulta = useQuery({
    queryKey: ['entrega-local', entregaId],
    queryFn: () => buscarEntregaLocal(entregaId),
  });

  const [selecionado, setSelecionado] = useState<EntregaStatus | null>(null);
  const [salvando, setSalvando] = useState(false);
  const [erro, setErro] = useState<string | null>(null);

  async function confirmar() {
    if (!selecionado) {
      return;
    }

    setSalvando(true);
    setErro(null);

    try {
      await agendarStatusEntrega(entregaId, selecionado);
      await queryClient.invalidateQueries({ queryKey: ['entregas-locais'] });
      await queryClient.invalidateQueries({ queryKey: ['entrega-local', entregaId] });
      navigation.goBack();
    } catch {
      setErro('Não foi possível salvar o status no dispositivo. Tente novamente.');
    } finally {
      setSalvando(false);
    }
  }

  if (consulta.isLoading) {
    return (
      <Screen>
        <LoadingState message="Carregando entrega…" />
      </Screen>
    );
  }

  const entrega = consulta.data;

  if (!entrega) {
    return (
      <Screen>
        <ErrorState
          title="Entrega não encontrada"
          message="A entrega pode ter sido removida na última sincronização."
          onRetry={() => {
            void consulta.refetch();
          }}
        />
      </Screen>
    );
  }

  const opcoes = proximosStatusEntrega(entrega.status);

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

          {erro ? (
            <Text
              style={styles.erro}
              accessibilityRole="alert"
              accessibilityLiveRegion="polite"
            >
              {erro}
            </Text>
          ) : null}

          <Button
            title="Confirmar status"
            disabled={selecionado === null}
            loading={salvando}
            onPress={() => {
              void confirmar();
            }}
            style={styles.acao}
            testID="botao-confirmar-status"
          />
        </>
      )}

      <Text style={styles.aviso}>
        O status é salvo no aparelho e enviado quando houver conexão. Repetir a mesma
        operação não gera efeito colateral no servidor.
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
  erro: {
    ...typography.corpo,
    color: theme.colors.danger,
    marginTop: theme.spacing.md,
  },
  acao: {
    marginTop: theme.spacing.md,
  },
  aviso: {
    ...typography.legenda,
    marginTop: theme.spacing.md,
  },
});
