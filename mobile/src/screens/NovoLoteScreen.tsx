import { useQuery, useQueryClient } from '@tanstack/react-query';
import * as Crypto from 'expo-crypto';
import { useState } from 'react';
import { Pressable, StyleSheet, Text, View } from 'react-native';

import { Button, Card, EmptyState, ErrorState, Field, LoadingState, Screen } from '../components';
import { listarProdutosLocais } from '../db/produtos';
import type { MainStackScreenProps } from '../navigation/types';
import { theme } from '../theme/tokens';
import { typography } from '../theme/typography';
import { salvarLoteOffline } from '../sync/lotes';
import {
  converterQuantidade,
  hojeIsoLocal,
  validarNovoLote,
  type ErrosNovoLote,
} from '../utils/validacaoLote';

/** Registro de lote local-first: salva no SQLite e enfileira o envio. */
export function NovoLoteScreen({ navigation }: MainStackScreenProps<'NovoLote'>) {
  const queryClient = useQueryClient();
  const produtos = useQuery({
    queryKey: ['produtos-locais'],
    queryFn: listarProdutosLocais,
  });

  const [produtoId, setProdutoId] = useState<number | null>(null);
  const [dataColheita, setDataColheita] = useState(hojeIsoLocal());
  const [quantidade, setQuantidade] = useState('');
  const [erros, setErros] = useState<ErrosNovoLote>({});
  const [erroGeral, setErroGeral] = useState<string | null>(null);
  const [salvando, setSalvando] = useState(false);

  async function salvar() {
    const errosValidacao = validarNovoLote({ produtoId, dataColheita, quantidade });
    setErros(errosValidacao);
    if (Object.keys(errosValidacao).length > 0) {
      return;
    }

    const produto = produtos.data?.find((item) => item.id === produtoId);
    if (!produto || produtoId === null) {
      setErros({ produtoId: 'Selecione um produto.' });
      return;
    }

    setErroGeral(null);
    setSalvando(true);

    try {
      await salvarLoteOffline({
        codigoRastreio: Crypto.randomUUID(),
        produtoId,
        produtoNome: produto.nome,
        produtoUnidade: produto.unidade,
        dataColheita,
        quantidade: converterQuantidade(quantidade),
        status: 'Disponivel',
        criadoEm: new Date().toISOString(),
      });

      await queryClient.invalidateQueries({ queryKey: ['lotes-locais'] });
      navigation.goBack();
    } catch {
      setErroGeral('Não foi possível salvar o lote no dispositivo. Tente novamente.');
    } finally {
      setSalvando(false);
    }
  }

  if (produtos.isLoading) {
    return (
      <Screen>
        <LoadingState message="Carregando produtos…" />
      </Screen>
    );
  }

  if (produtos.isError) {
    return (
      <Screen>
        <ErrorState
          title="Produtos indisponíveis"
          message="Não foi possível ler o catálogo local."
          onRetry={() => {
            void produtos.refetch();
          }}
        />
      </Screen>
    );
  }

  const listaProdutos = produtos.data ?? [];

  return (
    <Screen scrollable>
      <Text style={[typography.titulo, styles.titulo]}>Novo lote</Text>
      <Text style={styles.descricao}>
        O lote é salvo no aparelho primeiro e enviado automaticamente quando houver conexão.
      </Text>

      <Card title="Produto">
        {listaProdutos.length === 0 ? (
          <EmptyState
            title="Nenhum produto disponível"
            description="Conecte-se à API para atualizar o catálogo."
          />
        ) : (
          listaProdutos.map((produto) => {
            const selecionado = produtoId === produto.id;
            return (
              <Pressable
                key={produto.id}
                accessibilityRole="radio"
                accessibilityState={{ checked: selecionado }}
                accessibilityLabel={`${produto.nome}, ${produto.categoria}, unidade ${produto.unidade}`}
                onPress={() => {
                  setProdutoId(produto.id);
                  setErros((atuais) => ({ ...atuais, produtoId: undefined }));
                }}
                testID={`produto-${produto.id}`}
                style={({ pressed }) => [
                  styles.opcao,
                  selecionado && styles.opcaoMarcada,
                  pressed && styles.opcaoPressionada,
                ]}
              >
                <View style={[styles.marcador, selecionado && styles.marcadorAtivo]} />
                <View style={styles.textoOpcao}>
                  <Text style={typography.corpo}>{produto.nome}</Text>
                  <Text style={typography.legenda}>
                    {produto.categoria} • {produto.unidade}
                  </Text>
                </View>
              </Pressable>
            );
          })
        )}
        {erros.produtoId ? (
          <Text style={styles.erroCampo} accessibilityLiveRegion="polite">
            {erros.produtoId}
          </Text>
        ) : null}
      </Card>

      <Field
        label="Data da colheita"
        value={dataColheita}
        onChangeText={(texto) => {
          setDataColheita(texto);
          setErros((atuais) => ({ ...atuais, dataColheita: undefined }));
        }}
        placeholder="AAAA-MM-DD"
        maxLength={10}
        autoCapitalize="none"
        autoCorrect={false}
        hint="Use o formato AAAA-MM-DD."
        error={erros.dataColheita}
        testID="campo-data-colheita"
      />

      <Field
        label="Quantidade"
        value={quantidade}
        onChangeText={(texto) => {
          setQuantidade(texto);
          setErros((atuais) => ({ ...atuais, quantidade: undefined }));
        }}
        placeholder="Ex.: 120"
        keyboardType="decimal-pad"
        error={erros.quantidade}
        testID="campo-quantidade"
      />

      {erroGeral ? (
        <Text
          style={styles.erroGeral}
          accessibilityRole="alert"
          accessibilityLiveRegion="polite"
        >
          {erroGeral}
        </Text>
      ) : null}

      <Button
        title="Salvar lote"
        onPress={() => {
          void salvar();
        }}
        loading={salvando}
        disabled={salvando}
        style={styles.botao}
        testID="botao-salvar-lote"
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
  textoOpcao: {
    flexShrink: 1,
    gap: 2,
  },
  erroCampo: {
    ...typography.legenda,
    color: theme.colors.danger,
  },
  erroGeral: {
    ...typography.corpo,
    color: theme.colors.danger,
    marginTop: theme.spacing.sm,
  },
  botao: {
    marginTop: theme.spacing.md,
  },
});
