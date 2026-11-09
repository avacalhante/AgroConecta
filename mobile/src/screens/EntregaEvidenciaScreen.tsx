import { useQuery, useQueryClient } from '@tanstack/react-query';
import * as FileSystem from 'expo-file-system/legacy';
import { ImageManipulator, SaveFormat } from 'expo-image-manipulator';
import * as ImagePicker from 'expo-image-picker';
import * as Location from 'expo-location';
import { useState } from 'react';
import { Image, Linking, StyleSheet, Text, View } from 'react-native';

import { useAuth } from '../auth/AuthContext';
import { Button, Card, ErrorState, Field, InfoRow, LoadingState, Screen } from '../components';
import { buscarEntregaLocal } from '../db/entregas';
import type { MainStackScreenProps } from '../navigation/types';
import { useSincronizacao } from '../sync/SincronizacaoProvider';
import { agendarEnvioEvidencia, sincronizarEvidencias } from '../sync/evidencias';
import { theme } from '../theme/tokens';
import { typography } from '../theme/typography';

type Foto = {
  uri: string;
  nome: string;
  tipo: string;
};

type Coordenadas = {
  latitude: number;
  longitude: number;
  aproximada: boolean;
};

const LARGURA_MAXIMA = 1280;

/** Registra a evidência com câmera/galeria, GPS e compressão. */
export function EntregaEvidenciaScreen({
  route,
}: MainStackScreenProps<'EntregaEvidencia'>) {
  const { entregaId } = route.params;
  const { executarAutenticado } = useAuth();
  const { sincronizarAgora } = useSincronizacao();
  const queryClient = useQueryClient();
  const [observacao, setObservacao] = useState('');
  const [foto, setFoto] = useState<Foto | null>(null);
  const [coordenadas, setCoordenadas] = useState<Coordenadas | null>(null);
  const [capturando, setCapturando] = useState(false);
  const [localizando, setLocalizando] = useState(false);
  const [enviando, setEnviando] = useState(false);
  const [mensagem, setMensagem] = useState<string | null>(null);
  const [erro, setErro] = useState<string | null>(null);
  const [podeAbrirAjustes, setPodeAbrirAjustes] = useState(false);

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

  const fotoAtual: Foto | null =
    foto ??
    (entrega.fotoUrl?.startsWith('file:')
      ? { uri: entrega.fotoUrl, nome: 'evidencia.jpg', tipo: 'image/jpeg' }
      : null);

  const coordenadasAtuais: Coordenadas | null =
    coordenadas ??
    (entrega.latitude !== null && entrega.longitude !== null
      ? {
          latitude: entrega.latitude,
          longitude: entrega.longitude,
          aproximada: false,
        }
      : null);

  const uriExibivel =
    foto?.uri ??
    (entrega.fotoUrl && /^(file|https?):/i.test(entrega.fotoUrl)
      ? entrega.fotoUrl
      : null);

  const idEntrega = entrega.id;

  async function persistirFoto(uri: string, id: number): Promise<string> {
    const diretorio = `${FileSystem.documentDirectory}evidencias/`;
    const info = await FileSystem.getInfoAsync(diretorio);
    if (!info.exists) {
      await FileSystem.makeDirectoryAsync(diretorio, { intermediates: true });
    }
    const destino = `${diretorio}entrega-${id}-${Date.now()}.jpg`;
    await FileSystem.copyAsync({ from: uri, to: destino });
    return destino;
  }

  async function capturar(origem: 'camera' | 'galeria'): Promise<void> {
    setErro(null);
    setMensagem(null);
    setCapturando(true);

    try {
      if (origem === 'camera') {
        const permissao = await ImagePicker.requestCameraPermissionsAsync();
        if (!permissao.granted) {
          setErro(
            'Permissão de câmera negada. Você pode escolher uma foto da galeria.',
          );
          return;
        }
      } else {
        const permissao = await ImagePicker.requestMediaLibraryPermissionsAsync();
        if (!permissao.granted) {
          setErro('Permissão para acessar as fotos negada. Libere nos ajustes do aparelho.');
          return;
        }
      }

      const resultado =
        origem === 'camera'
          ? await ImagePicker.launchCameraAsync({
              mediaTypes: ['images'],
              quality: 0.9,
            })
          : await ImagePicker.launchImageLibraryAsync({
              mediaTypes: ['images'],
              quality: 0.9,
            });

      const asset = resultado.canceled ? undefined : resultado.assets[0];
      if (!asset) {
        return;
      }

      const contexto = ImageManipulator.manipulate(asset.uri, {
        maxWidth: 1600,
        maxHeight: 1600,
      });
      if ((asset.width ?? 0) > LARGURA_MAXIMA) {
        contexto.resize({ width: LARGURA_MAXIMA });
      }
      const imagem = await contexto.renderAsync();
      const comprimida = await imagem.saveAsync({
        compress: 0.7,
        format: SaveFormat.JPEG,
      });
      const uri = await persistirFoto(comprimida.uri, idEntrega);

      setFoto({
        uri,
        nome: `evidencia-${idEntrega}.jpg`,
        tipo: 'image/jpeg',
      });
      setMensagem('Foto capturada e comprimida.');
    } catch {
      setErro('Não foi possível capturar a foto. Tente novamente ou use a galeria.');
    } finally {
      setCapturando(false);
    }
  }

  async function obterLocalizacao(): Promise<void> {
    setErro(null);
    setMensagem(null);
    setPodeAbrirAjustes(false);
    setLocalizando(true);

    try {
      const permissao = await Location.requestForegroundPermissionsAsync();
      if (!permissao.granted) {
        setPodeAbrirAjustes(!permissao.canAskAgain);
        setErro(
          permissao.canAskAgain
            ? 'Permissão de localização negada. Toque em “Obter localização” para tentar de novo.'
            : 'Permissão de localização negada. Libere o acesso nos ajustes do aparelho.',
        );
        return;
      }

      let posicao: Location.LocationObject;
      try {
        posicao = await Location.getCurrentPositionAsync({
          accuracy: Location.Accuracy.Balanced,
        });
      } catch {
        const ultima = await Location.getLastKnownPositionAsync();
        if (!ultima) {
          throw new Error('Sem posição disponível.');
        }
        setCoordenadas({
          latitude: ultima.coords.latitude,
          longitude: ultima.coords.longitude,
          aproximada: true,
        });
        setMensagem('GPS indisponível no momento. Usando a última localização conhecida.');
        return;
      }

      setCoordenadas({
        latitude: posicao.coords.latitude,
        longitude: posicao.coords.longitude,
        aproximada: false,
      });
      setMensagem('Localização atual obtida.');
    } catch {
      setErro(
        'Não foi possível obter a localização. Verifique se o GPS está ligado e tente novamente.',
      );
    } finally {
      setLocalizando(false);
    }
  }

  async function enviar(): Promise<void> {
    if (!fotoAtual || !coordenadasAtuais) {
      setErro('Capture a foto e obtenha a localização antes de enviar.');
      return;
    }

    setEnviando(true);
    setErro(null);
    setMensagem(null);

    try {
      await agendarEnvioEvidencia({
        entregaId: idEntrega,
        latitude: coordenadasAtuais.latitude,
        longitude: coordenadasAtuais.longitude,
        ...(observacao.trim() ? { observacao: observacao.trim() } : {}),
        fotoUri: fotoAtual.uri,
        fotoNome: fotoAtual.nome,
        fotoTipo: fotoAtual.tipo,
      });
      await queryClient.invalidateQueries({ queryKey: ['entrega-local', entregaId] });

      const resultado = await sincronizarEvidencias(executarAutenticado);
      await queryClient.invalidateQueries({ queryKey: ['entrega-local', entregaId] });

      if (resultado.enviados > 0) {
        setFoto(null);
        setMensagem('Evidência enviada ao servidor.');
      } else if (resultado.offline) {
        setMensagem(
          'Sem conexão. A evidência ficou salva no aparelho e será enviada ao reconectar.',
        );
      } else {
        const atualizada = await buscarEntregaLocal(idEntrega);
        setErro(
          atualizada?.evidenciaErro
            ? `A evidência ficou salva, mas o servidor recusou o envio: ${atualizada.evidenciaErro}`
            : 'A evidência ficou salva, mas o envio não foi concluído. Tente novamente.',
        );
      }

      void sincronizarAgora();
    } catch {
      setErro('Não foi possível salvar a evidência no aparelho. Tente novamente.');
    } finally {
      setEnviando(false);
    }
  }

  const prontoParaEnviar = Boolean(fotoAtual) && Boolean(coordenadasAtuais) && !enviando;

  return (
    <Screen scrollable>
      <Text style={[typography.titulo, styles.titulo]}>Evidência da entrega</Text>
      <Text style={styles.descricao}>Pedido {entrega.pedidoId}</Text>

      {entrega.evidenciaPendente ? (
        <Text style={styles.pendente} accessibilityLiveRegion="polite">
          Evidência salva no aparelho. Aguardando conexão para enviar.
        </Text>
      ) : null}

      {entrega.evidenciaErro ? (
        <Text style={styles.erro} accessibilityRole="alert">
          Falha na evidência: {entrega.evidenciaErro}
        </Text>
      ) : null}

      <Card title="Foto">
        {uriExibivel ? (
          <Image
            source={{ uri: uriExibivel }}
            style={styles.imagem}
            accessibilityLabel="Prévia da foto da entrega"
          />
        ) : (
          <View style={styles.foto}>
            <Text style={typography.corpoMuted}>
              {entrega.fotoUrl ? 'Foto registrada no servidor' : 'Nenhuma foto capturada'}
            </Text>
          </View>
        )}

        <View style={styles.linhaBotoes}>
          <Button
            title="Tirar foto"
            variant="primary"
            onPress={() => {
              void capturar('camera');
            }}
            loading={capturando}
            disabled={capturando}
            style={styles.botaoFoto}
            testID="botao-camera"
          />
          <Button
            title="Escolher da galeria"
            variant="secondary"
            onPress={() => {
              void capturar('galeria');
            }}
            disabled={capturando}
            style={styles.botaoFoto}
            testID="botao-galeria"
          />
        </View>

        <Text style={styles.detalhe}>
          A foto é reduzida para até {LARGURA_MAXIMA} px e comprimida em JPEG antes do envio.
        </Text>
      </Card>

      <Card title="Localização">
        <InfoRow
          label="Latitude"
          value={
            coordenadasAtuais ? coordenadasAtuais.latitude.toFixed(6) : '—'
          }
        />
        <InfoRow
          label="Longitude"
          value={
            coordenadasAtuais ? coordenadasAtuais.longitude.toFixed(6) : '—'
          }
        />
        {coordenadasAtuais?.aproximada ? (
          <Text style={styles.detalhe}>Localização aproximada (última conhecida).</Text>
        ) : null}

        <Button
          title="Obter localização"
          variant="secondary"
          onPress={() => {
            void obterLocalizacao();
          }}
          loading={localizando}
          disabled={localizando}
          style={styles.acao}
          testID="botao-localizacao"
        />

        {podeAbrirAjustes ? (
          <Button
            title="Abrir ajustes"
            variant="secondary"
            onPress={() => {
              void Linking.openSettings();
            }}
            style={styles.acao}
            testID="botao-ajustes"
          />
        ) : null}
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

      {erro ? (
        <Text style={styles.erro} accessibilityRole="alert">
          {erro}
        </Text>
      ) : null}

      {mensagem ? (
        <Text style={styles.mensagem} accessibilityLiveRegion="polite">
          {mensagem}
        </Text>
      ) : null}

      {!prontoParaEnviar && !enviando ? (
        <Text style={styles.detalhe}>
          Capture a foto e obtenha a localização para habilitar o envio.
        </Text>
      ) : null}

      <Button
        title="Enviar evidência"
        disabled={!prontoParaEnviar}
        loading={enviando}
        onPress={() => {
          void enviar();
        }}
        style={styles.acao}
        testID="botao-enviar-evidencia"
      />
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
  imagem: {
    width: '100%',
    height: 200,
    borderRadius: theme.radius.md,
    backgroundColor: theme.colors.surfaceMuted,
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
  linhaBotoes: {
    flexDirection: 'row',
    gap: theme.spacing.sm,
    marginTop: theme.spacing.md,
  },
  botaoFoto: {
    flex: 1,
  },
  detalhe: {
    ...typography.legenda,
    marginTop: theme.spacing.sm,
  },
  acao: {
    marginTop: theme.spacing.md,
  },
  pendente: {
    ...typography.legenda,
    color: theme.colors.warning,
    marginBottom: theme.spacing.sm,
  },
  erro: {
    ...typography.corpo,
    color: theme.colors.danger,
    marginTop: theme.spacing.sm,
  },
  mensagem: {
    ...typography.corpo,
    color: theme.colors.success,
    marginTop: theme.spacing.sm,
  },
});
