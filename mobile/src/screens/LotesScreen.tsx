import { useFocusEffect } from '@react-navigation/native';
import { useCallback } from 'react';
import { FlatList, StyleSheet, Text, View } from 'react-native';

import {
  Button,
  Card,
  EmptyState,
  ErrorState,
  LoadingState,
  Screen,
  StatusBadge,
} from '../components';
import type { LoteLocal } from '../db/lotes';
import { useLotes } from '../hooks/useLotes';
import type { MainTabScreenProps } from '../navigation/types';
import { statusMeta } from '../theme/status';
import { theme } from '../theme/tokens';
import { typography } from '../theme/typography';
import { formatarData, formatarQuantidade } from '../utils/format';

/** Meus lotes: dados locais, com envio pendente e sincronização sob demanda. */
export function LotesScreen({ navigation }: MainTabScreenProps<'Lotes'>) {
  const { data, error, isLoading, offline, refetch, sincronizar, sincronizando } = useLotes();

  useFocusEffect(
    useCallback(() => {
      void sincronizar();
    }, [sincronizar]),
  );

  if (isLoading) {
    return (
      <Screen>
        <LoadingState message="Carregando lotes…" />
      </Screen>
    );
  }

  if (error) {
    return (
      <Screen>
        <ErrorState
          title="Não foi possível ler os lotes"
          message="O banco local não respondeu. Tente novamente."
          onRetry={() => {
            void refetch();
          }}
        />
      </Screen>
    );
  }

  const lotes = data ?? [];

  return (
    <Screen padded={false}>
      <View style={styles.cabecalho}>
        <Text style={typography.titulo}>Meus lotes</Text>
        <Text style={styles.descricao}>
          Registre a colheita mesmo sem internet; o envio fica pendente até a conexão.
        </Text>
        <Button
          title="Novo lote"
          onPress={() => navigation.navigate('NovoLote')}
          testID="botao-novo-lote"
        />
        {offline ? (
          <Text style={styles.avisoOffline} accessibilityLiveRegion="polite">
            Sem conexão. Os lotes pendentes serão enviados ao reconectar.
          </Text>
        ) : null}
      </View>

      <FlatList
        data={lotes}
        keyExtractor={(item) => item.codigoRastreio}
        contentContainerStyle={styles.lista}
        refreshing={sincronizando}
        onRefresh={() => {
          void sincronizar();
        }}
        ListEmptyComponent={
          <EmptyState
            title="Nenhum lote registrado"
            description="Registre a primeira colheita para começar a rastreabilidade."
            actionLabel="Novo lote"
            onAction={() => navigation.navigate('NovoLote')}
          />
        }
        renderItem={({ item }) => <ItemLote item={item} navigation={navigation} />}
      />
    </Screen>
  );
}

type ItemLoteProps = {
  item: LoteLocal;
  navigation: MainTabScreenProps<'Lotes'>['navigation'];
};

function ItemLote({ item, navigation }: ItemLoteProps) {
  const situacao = item.sincronizado ? 'sincronizado' : 'pendente de envio';

  return (
    <Card
      onPress={() =>
        navigation.navigate('LoteDetalhe', { codigoRastreio: item.codigoRastreio })
      }
      accessibilityLabel={`${item.produtoNome}, ${formatarQuantidade(
        item.quantidade,
        item.produtoUnidade,
      )}, colhido em ${formatarData(item.dataColheita)}, status ${statusMeta[item.status].label}, ${situacao}`}
      testID={`lote-${item.codigoRastreio}`}
    >
      <View style={styles.linhaTitulo}>
        <Text style={[typography.secao, styles.produto]}>{item.produtoNome}</Text>
        <StatusBadge status={item.status} />
      </View>
      <Text style={styles.detalhe}>
        {formatarQuantidade(item.quantidade, item.produtoUnidade)} • colheita{' '}
        {formatarData(item.dataColheita)}
      </Text>
      {item.sincronizado ? null : (
        <Text style={styles.pendente}>Pendente de envio</Text>
      )}
    </Card>
  );
}

const styles = StyleSheet.create({
  cabecalho: {
    paddingHorizontal: theme.spacing.md,
    paddingTop: theme.spacing.md,
    gap: theme.spacing.sm,
  },
  descricao: {
    ...typography.corpoMuted,
  },
  avisoOffline: {
    ...typography.legenda,
    color: theme.colors.warning,
  },
  lista: {
    padding: theme.spacing.md,
    gap: theme.spacing.md,
  },
  linhaTitulo: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    gap: theme.spacing.sm,
  },
  produto: {
    flexShrink: 1,
  },
  detalhe: {
    ...typography.corpoMuted,
  },
  pendente: {
    ...typography.legenda,
    color: theme.colors.warning,
    fontWeight: theme.fontWeight.bold,
  },
});
