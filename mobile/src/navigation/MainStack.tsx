import { createNativeStackNavigator } from '@react-navigation/native-stack';

import { EntregaDetalheScreen } from '../screens/EntregaDetalheScreen';
import { EntregaEvidenciaScreen } from '../screens/EntregaEvidenciaScreen';
import { EntregaStatusScreen } from '../screens/EntregaStatusScreen';
import { LoteDetalheScreen } from '../screens/LoteDetalheScreen';
import { theme } from '../theme/tokens';
import { MainTabs } from './MainTabs';
import type { MainStackParamList } from './types';

const Stack = createNativeStackNavigator<MainStackParamList>();

type MainStackProps = {
  onSair: () => void;
};

/** Fluxos autenticados: abas principais e detalhes de lote e entrega. */
export function MainStack({ onSair }: MainStackProps) {
  return (
    <Stack.Navigator
      screenOptions={{
        headerStyle: { backgroundColor: theme.colors.surface },
        headerTintColor: theme.colors.primaryDark,
        headerTitleStyle: {
          color: theme.colors.text,
          fontWeight: theme.fontWeight.bold,
        },
        headerBackTitle: 'Voltar',
        contentStyle: { backgroundColor: theme.colors.background },
      }}
    >
      <Stack.Screen name="Tabs" options={{ headerShown: false }}>
        {() => <MainTabs onSair={onSair} />}
      </Stack.Screen>
      <Stack.Screen
        name="LoteDetalhe"
        component={LoteDetalheScreen}
        options={{ title: 'Detalhe do lote' }}
      />
      <Stack.Screen
        name="EntregaDetalhe"
        component={EntregaDetalheScreen}
        options={{ title: 'Detalhe da entrega' }}
      />
      <Stack.Screen
        name="EntregaStatus"
        component={EntregaStatusScreen}
        options={{ title: 'Alterar status' }}
      />
      <Stack.Screen
        name="EntregaEvidencia"
        component={EntregaEvidenciaScreen}
        options={{ title: 'Registrar evidência' }}
      />
    </Stack.Navigator>
  );
}
