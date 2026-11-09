import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';

import { EntregasScreen } from '../screens/EntregasScreen';
import { InicioScreen } from '../screens/InicioScreen';
import { LotesScreen } from '../screens/LotesScreen';
import { PerfilScreen } from '../screens/PerfilScreen';
import { theme } from '../theme/tokens';
import type { MainTabParamList } from './types';

const Tab = createBottomTabNavigator<MainTabParamList>();

type MainTabsProps = {
  onSair: () => void;
};

/** Navegação principal do aplicativo após o login. */
export function MainTabs({ onSair }: MainTabsProps) {
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
      <Tab.Screen name="Lotes" component={LotesScreen} options={{ title: 'Lotes' }} />
      <Tab.Screen name="Entregas" component={EntregasScreen} options={{ title: 'Entregas' }} />
      <Tab.Screen name="Perfil" options={{ title: 'Perfil' }}>
        {() => <PerfilScreen onSair={onSair} />}
      </Tab.Screen>
    </Tab.Navigator>
  );
}
