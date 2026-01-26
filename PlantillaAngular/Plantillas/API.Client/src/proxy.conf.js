/* eslint-disable @typescript-eslint/no-var-requires */
/* eslint-disable no-undef */
const { env } = require('process');

// Usa la variable de entorno proporcionada por Aspire
const target = env.services__pruebaangularapi__http__0 || env.services__pruebaangularapi__https__0 || 'http://localhost:5000';

const PROXY_CONFIG = [
  {
    context: [
      "/api",
      "/graphql"
    ],
    target,
    secure: false,
    changeOrigin: true,
    logLevel: 'debug'
  }
]

module.exports = PROXY_CONFIG;
