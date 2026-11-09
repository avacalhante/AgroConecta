import { NavigationContainer, DefaultTheme } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';

import { useAuth } from '../auth/AuthContext';
import { LoadingState, Screen } from '../components';
import { LoginScreen } from '../screens/LoginScreen';
import { theme } from '../theme/tokens';
import { MainStack } from './MainStack';
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

/** Alterna entre o fluxo anônimo (login) e o aplicativo autenticado. */
export function RootNavigator() {
  const { restaurando, usuario } = useAuth();

  if (restaurando) {
    return (
      <Screen>
        <LoadingState message="Restaurando sessão…" />
      </Screen>
    );
  }

  return (
    <NavigationContainer theme={navigationTheme}>
      <Stack.Navigator screenOptions={{ headerShown: false }}>
        {usuario ? (
          <Stack.Screen name="Main" component={MainStack} />
        ) : (
          <Stack.Screen name="Login" component={LoginScreen} />
        )}
      </Stack.Navigator>
    </NavigationContainer>
  );
}
