import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { useWindowDimensions } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';

import { useAuth } from '../auth/AuthContext';
import { EntregasScreen } from '../screens/EntregasScreen';
import { InicioScreen } from '../screens/InicioScreen';
import { LotesScreen } from '../screens/LotesScreen';
import { PerfilScreen } from '../screens/PerfilScreen';
import { theme } from '../theme/tokens';
import type { MainTabParamList } from './types';

const Tab = createBottomTabNavigator<MainTabParamList>();

/** Navegação principal do aplicativo após o login, por papel do usuário. */
export function MainTabs() {
  const { usuario } = useAuth();
  const { fontScale } = useWindowDimensions();
  const insets = useSafeAreaInsets();
  const mostrarLotes = usuario?.papel === 'Produtor' || usuario?.papel === 'Admin';
  const mostrarEntregas = usuario?.papel === 'Motorista' || usuario?.papel === 'Admin';
  // A barra padrão tem altura fixa e corta os rótulos com fonte ampliada.
  const alturaBarra = 64 + insets.bottom + Math.max(0, fontScale - 1) * 24;

  return (
    <Tab.Navigator
      screenOptions={{
        headerShown: false,
        // Sem ícones próprios, o placeholder padrão adiciona um glifo inválido ao rótulo.
        tabBarIcon: () => null,
        tabBarActiveTintColor: theme.colors.primaryDark,
        tabBarInactiveTintColor: theme.colors.textMuted,
        tabBarStyle: {
          height: alturaBarra,
          backgroundColor: theme.colors.surface,
          borderTopColor: theme.colors.border,
        },
        tabBarLabelStyle: {
          fontSize: theme.fontSize.caption,
          fontWeight: '600',
          paddingVertical: 4,
        },
        tabBarItemStyle: {
          minHeight: theme.touchTarget,
          paddingVertical: 4,
        },
      }}
    >
      <Tab.Screen name="Inicio" component={InicioScreen} options={{ title: 'Início' }} />
      {mostrarLotes ? (
        <Tab.Screen name="Lotes" component={LotesScreen} options={{ title: 'Lotes' }} />
      ) : null}
      {mostrarEntregas ? (
        <Tab.Screen
          name="Entregas"
          component={EntregasScreen}
          options={{ title: 'Entregas' }}
        />
      ) : null}
      <Tab.Screen name="Perfil" component={PerfilScreen} options={{ title: 'Perfil' }} />
    </Tab.Navigator>
  );
}
