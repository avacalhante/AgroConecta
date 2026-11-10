const { withAndroidManifest } = require('@expo/config-plugins');

/**
 * A API local de desenvolvimento usa HTTP. Em builds com URL HTTPS, o manifesto
 * permanece com o bloqueio padrão de cleartext do Android.
 */
module.exports = function withCleartextTraffic(config) {
  if (!String(process.env.EXPO_PUBLIC_API_URL ?? '').startsWith('http://')) {
    return config;
  }

  return withAndroidManifest(config, (configComManifesto) => {
    const aplicacao = configComManifesto.modResults.manifest.application?.[0];
    if (aplicacao) {
      aplicacao.$['android:usesCleartextTraffic'] = 'true';
    }
    return configComManifesto;
  });
};
