// Reemplazar la URL de la API con una variable de entorno para mayor seguridad
export const environment = {
  ApiUrl: process.env['API_URL'] || 'https://localhost:50598/',
  production: false,
};




