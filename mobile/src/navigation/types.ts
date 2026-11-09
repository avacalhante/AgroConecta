import type { BottomTabScreenProps } from '@react-navigation/bottom-tabs';
import type { CompositeScreenProps, NavigatorScreenParams } from '@react-navigation/native';
import type { NativeStackScreenProps } from '@react-navigation/native-stack';

/** Pilha raiz: sessão anônima (login) ou aplicativo autenticado. */
export type RootStackParamList = {
  Login: undefined;
  Main: NavigatorScreenParams<MainStackParamList> | undefined;
};

/** Pilha autenticada: abas principais e telas de detalhe por fluxo. */
export type MainStackParamList = {
  Tabs: NavigatorScreenParams<MainTabParamList> | undefined;
  NovoLote: undefined;
  LoteDetalhe: { codigoRastreio: string };
  EntregaDetalhe: { entregaId: number };
  EntregaStatus: { entregaId: number };
  EntregaEvidencia: { entregaId: number };
};

export type MainTabParamList = {
  Inicio: undefined;
  Lotes: undefined;
  Entregas: undefined;
  Perfil: undefined;
};

export type RootStackScreenProps<T extends keyof RootStackParamList> = NativeStackScreenProps<
  RootStackParamList,
  T
>;

export type MainStackScreenProps<T extends keyof MainStackParamList> = NativeStackScreenProps<
  MainStackParamList,
  T
>;

/**
 * Telas das abas também navegam para a pilha principal
 * (ex.: lista de lotes → detalhe do lote).
 */
export type MainTabScreenProps<T extends keyof MainTabParamList> = CompositeScreenProps<
  BottomTabScreenProps<MainTabParamList, T>,
  NativeStackScreenProps<MainStackParamList>
>;
