import { StyleSheet } from 'react-native';

import { colors, fontSize, fontWeight, lineHeight } from './tokens';

/** Estilos de texto compartilhados. Não fixam altura, para permitir fonte ampliada. */
export const typography = StyleSheet.create({
  titulo: {
    fontSize: fontSize.headline,
    lineHeight: lineHeight.headline,
    fontWeight: fontWeight.bold,
    color: colors.text,
  },
  subtitulo: {
    fontSize: fontSize.title,
    lineHeight: lineHeight.title,
    fontWeight: fontWeight.bold,
    color: colors.text,
  },
  secao: {
    fontSize: fontSize.bodyLarge,
    lineHeight: lineHeight.bodyLarge,
    fontWeight: fontWeight.semiBold,
    color: colors.text,
  },
  corpo: {
    fontSize: fontSize.body,
    lineHeight: lineHeight.body,
    color: colors.text,
  },
  corpoMuted: {
    fontSize: fontSize.body,
    lineHeight: lineHeight.body,
    color: colors.textMuted,
  },
  legenda: {
    fontSize: fontSize.caption,
    lineHeight: lineHeight.caption,
    color: colors.textMuted,
  },
  rotulo: {
    fontSize: fontSize.body,
    lineHeight: lineHeight.body,
    fontWeight: fontWeight.semiBold,
    color: colors.text,
  },
  botao: {
    fontSize: fontSize.bodyLarge,
    fontWeight: fontWeight.bold,
  },
});
