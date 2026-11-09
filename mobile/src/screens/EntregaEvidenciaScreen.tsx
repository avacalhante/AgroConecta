import { useQuery } from '@tanstack/react-query';
import { useState } from 'react';
import { StyleSheet, Text, View } from 'react-native';

import { Button, Card, ErrorState, Field, InfoRow, LoadingState, Screen } from '../components';
import { buscarEntregaLocal } from '../db/entregas';
import type { MainStackScreenProps } from '../navigation/types';
import { theme } from '../theme/tokens';
import { typography } from '../theme/typography';

/** Estrutura da evidência; câmera, localização e upload entram na tarefa B7. */
export function EntregaEvidenciaScreen({
  route,
}: MainStackScreenProps<'EntregaEvidencia'>) {
  const { entregaId } = route.params;
  const [observacao, setObservacao] = useState('');
  const consulta = useQuery({
    queryKey: ['entrega-local', entregaId],
    queryFn: () => buscarEntregaLocal(entregaId),
  });

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

  return (
    <Screen scrollable>
      <Text style={[typography.titulo, styles.titulo]}>Evidência da entrega</Text>
      <Text style={styles.descricao}>Pedido {entrega.pedidoId}</Text>

      <Card title="Foto">
        <View style={styles.foto}>
          <Text style={typography.corpoMuted}>Nenhuma foto capturada</Text>
        </View>
        <Text style={styles.detalhe}>
          A captura pela câmera, com permissão e compressão, será habilitada na tarefa B7.
        </Text>
      </Card>

      <Card title="Localização">
        <InfoRow label="Latitude" value="—" />
        <InfoRow label="Longitude" value="—" />
        <Text style={styles.detalhe}>
          As coordenadas serão obtidas com `expo-location` na tarefa B7.
        </Text>
      </Card>

      <Field
        label="Observação"
        value={observacao}
        onChangeText={setObservacao}
        multiline
        maxLength={500}
        placeholder="Informações sobre a entrega (opcional)"
        hint="Até 500 caracteres."
        testID="campo-observacao"
      />

      <Button
        title="Enviar evidência"
        disabled
        onPress={() => undefined}
        style={styles.acao}
        testID="botao-enviar-evidencia"
      />

      <Text style={styles.aviso}>
        A captura, o GPS e o upload multipart entram na tarefa B7. O rascunho não é enviado
        nesta etapa.
      </Text>
    </Screen>
  );
}

const styles = StyleSheet.create({
  titulo: {
    marginBottom: theme.spacing.xs,
  },
  descricao: {
    ...typography.corpoMuted,
    marginBottom: theme.spacing.md,
  },
  foto: {
    minHeight: 120,
    alignItems: 'center',
    justifyContent: 'center',
    borderRadius: theme.radius.md,
    borderWidth: 1,
    borderStyle: 'dashed',
    borderColor: theme.colors.borderStrong,
    backgroundColor: theme.colors.surfaceMuted,
    padding: theme.spacing.md,
  },
  detalhe: {
    ...typography.legenda,
  },
  acao: {
    marginTop: theme.spacing.md,
  },
  aviso: {
    ...typography.legenda,
    marginTop: theme.spacing.md,
  },
});
