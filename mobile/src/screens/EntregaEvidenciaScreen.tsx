import { useState } from 'react';
import { StyleSheet, Text, View } from 'react-native';

import { Button, Card, ErrorState, Field, InfoRow, Screen } from '../components';
import { buscarEntrega } from '../mock/dados';
import type { MainStackScreenProps } from '../navigation/types';
import { theme } from '../theme/tokens';
import { typography } from '../theme/typography';

/** Evidência mockada; câmera, localização e upload entram na tarefa B7. */
export function EntregaEvidenciaScreen({
  route,
  navigation,
}: MainStackScreenProps<'EntregaEvidencia'>) {
  const [observacao, setObservacao] = useState('');
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

  return (
    <Screen scrollable>
      <Text style={[typography.titulo, styles.titulo]}>Evidência da entrega</Text>
      <Text style={styles.descricao}>
        Pedido {entrega.pedidoId} • {entrega.destino}
      </Text>

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
        Demonstração da B1: a evidência ainda não é enviada. Upload multipart e fallback de
        permissão entram na tarefa B7.
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
