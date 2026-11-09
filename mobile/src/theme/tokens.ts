export const colors = {
  primary: '#1F6A3A',
  primaryDark: '#154C29',
  primaryLight: '#D7E9DC',
  accent: '#8A5A12',
  background: '#F5F7F4',
  surface: '#FFFFFF',
  surfaceMuted: '#EEF2EC',
  text: '#1B1F1B',
  textMuted: '#4A524A',
  textInverse: '#FFFFFF',
  border: '#C9D2C7',
  danger: '#B3261E',
  warning: '#8A5A12',
  success: '#1E6B3A',
  info: '#1B4F8A',
} as const;

export const spacing = {
  xs: 4,
  sm: 8,
  md: 16,
  lg: 24,
  xl: 32,
  xxl: 48,
} as const;

export const radius = {
  sm: 6,
  md: 10,
  lg: 16,
  pill: 999,
} as const;

export const fontSize = {
  caption: 12,
  body: 16,
  bodyLarge: 18,
  title: 22,
  headline: 28,
} as const;

export const touchTarget = 48;

export const theme = { colors, spacing, radius, fontSize, touchTarget } as const;

export type Theme = typeof theme;
