// Reemplazar la URL de la API con una variable de entorno para mayor seguridad
export const environment = {
  ApiUrl: process.env['API_URL'] || 'https://localhost:50598/',
  production: false,
};


// el proxy Elk usa el token de canales online, no funciona con el token de Azure
//export const ElkConfiguration: IELKEnvironment = {
//  production: false,
//  appName: 'Net6-Arquitectura',
//  endpoints: {
//  },
//  ApmElkConfig: {
//    INDEX_NAME: 'MGA-Smaple-Angular',
//    APP_FIELD: 'MGA-Smaple-Angular',
//    ENV_FIELD: 'Produccion',
//    VERSION_FIELD: 'Version',
//    USER_NAME_FIELD: 'UserName',
//    ELAPSED_MS_FIELD: 'ElapsedMilliseconds',
//    REQUEST_PATH_FIELD: 'RequestPath',
//    URL_FIELD: 'Url',
//    APP_STATE_FIELD: 'AppState',
//  },
//  ProxyTokenHeaderName: "proxyElkToken",
//  env: "local"
//};


