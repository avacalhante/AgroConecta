import NetInfo from '@react-native-community/netinfo';

/** Considera online quando há conexão e a internet não foi declarada inalcançável. */
export async function estaConectado(): Promise<boolean> {
  const estado = await NetInfo.fetch();
  if (estado.isConnected !== true) {
    return false;
  }
  return estado.isInternetReachable !== false;
}
