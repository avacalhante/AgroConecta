import { NavigationContainer, DefaultTheme } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { useState } from 'react';

import { LoginScreen } from '../screens/LoginScreen';
import { theme } from '../theme/tokens';
import { MainTabs } from './MainTabs';
import type { RootStackParamList } from './types';

const Stack = createNativeStackNavigator<RootStackParamList>();

const navigationTheme = {
  ...DefaultTheme,
  colors: {
    ...DefaultTheme.colors,
    primary: theme.colors.primary,
    background: theme.colors.background,
    card: theme.colors.surface,
    text: theme.colors.text,
    border: theme.colors.border,
  },
};

/**
 * Navegação raiz provisória.
 * A tarefa B2 substitui o estado local por um contexto de autenticação real.
 */
export function RootNavigator() {
  const [autenticado, setAutenticado] = useState(false);

  return (
    <NavigationContainer theme={navigationTheme}>
      <Stack.Navigator screenOptions={{ headerShown: false }}>
        {autenticado ? (
          <Stack.Screen name="Main">
            {() => <MainTabs onSair={() => setAutenticado(false)} />}
          </Stack.Screen>
        ) : (
          <Stack.Screen name="Login">
            {() => <LoginScreen onEntrar={() => setAutenticado(true)} />}
          </Stack.Screen>
        )}
      </Stack.Navigator>
    </NavigationContainer>
  );
}
