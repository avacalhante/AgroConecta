import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';

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
  const mostrarLotes = usuario?.papel === 'Produtor' || usuario?.papel === 'Admin';
  const mostrarEntregas = usuario?.papel === 'Motorista' || usuario?.papel === 'Admin';

  return (
    <Tab.Navigator
      screenOptions={{
        headerShown: false,
        tabBarActiveTintColor: theme.colors.primaryDark,
        tabBarInactiveTintColor: theme.colors.textMuted,
        tabBarStyle: {
          backgroundColor: theme.colors.surface,
          borderTopColor: theme.colors.border,
        },
        tabBarLabelStyle: {
          fontSize: theme.fontSize.caption,
          fontWeight: '600',
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
