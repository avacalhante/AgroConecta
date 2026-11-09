export const colors = {
  primary: '#1F6A3A',
  primaryDark: '#154C29',
  primaryLight: '#D7E9DC',
  accent: '#8A5A12',
  background: '#F5F7F4',
  surface: '#FFFFFF',
  surfaceMuted: '#EEF2EC',
  surfacePressed: '#E3EAE0',
  text: '#1B1F1B',
  textMuted: '#4A524A',
  textInverse: '#FFFFFF',
  border: '#C9D2C7',
  borderStrong: '#9DAA9B',
  borderControl: '#6F7B6D',
  danger: '#B3261E',
  dangerDark: '#8C1D18',
  dangerLight: '#F9DEDC',
  warning: '#8A5A12',
  warningLight: '#F6E7C8',
  success: '#1E6B3A',
  successLight: '#DDF0E3',
  info: '#1B4F8A',
  infoLight: '#DCE9F7',
  focus: '#1B4F8A',
  overlay: 'rgba(27, 31, 27, 0.4)',
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
  small: 14,
  body: 16,
  bodyLarge: 18,
  title: 22,
  headline: 28,
} as const;

export const fontWeight = {
  regular: '400',
  medium: '500',
  semiBold: '600',
  bold: '700',
} as const;

export const lineHeight = {
  caption: 16,
  small: 20,
  body: 24,
  bodyLarge: 26,
  title: 30,
  headline: 36,
} as const;

/** Estados de interação usados pelos componentes. */
export const opacity = {
  disabled: 0.55,
  pressed: 0.85,
} as const;

export const touchTarget = 48;

export const theme = {
  colors,
  spacing,
  radius,
  fontSize,
  fontWeight,
  lineHeight,
  opacity,
  touchTarget,
} as const;

export type Theme = typeof theme;
