
export const environment  = {
  ApiUrl: 'https://$(azure_site_name).azurewebsites.net/',
  production: true,
  appName: 'Net8-Arquitectura',
  
};

//export const ElkConfiguration: IELKEnvironment = {
//    production: true,
//    appName: 'MGA-Angular',
//    endpoints: {
//        elasticSearchEndpoint: 'http://[ip publica ELK]:[puerto]/_bulk'
//    },
//    ApmElkConfig: {
//      INDEX_NAME: 'MGA-Smaple-Angular',
//      APP_FIELD: 'MGA-Smaple-Angular',
//        ENV_FIELD: 'Produccion',
//        VERSION_FIELD: 'Version',
//        USER_NAME_FIELD: 'UserName',
//        ELAPSED_MS_FIELD: 'ElapsedMilliseconds',
//        REQUEST_PATH_FIELD: 'RequestPath',
//        URL_FIELD: 'Url',
//        APP_STATE_FIELD: 'AppState',
//    },
//    ProxyTokenHeaderName: "proxyElkToken",
//    env: "pro"
//};

