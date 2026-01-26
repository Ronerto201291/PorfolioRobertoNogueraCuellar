# Usar nginx para servir archivos estáticos
FROM nginx:alpine

# Remover configuración por defecto
RUN rm /etc/nginx/conf.d/default.conf

# Copiar archivos de la aplicación Angular
COPY src /usr/share/nginx/html/src
COPY dist /usr/share/nginx/html/dist

# Copiar index.html en la raíz
COPY src/index.html /usr/share/nginx/html/

# Copiar assets
COPY src/assets /usr/share/nginx/html/assets 2>/dev/null || true

# Copiar configuración de nginx
COPY nginx.conf /etc/nginx/nginx.conf

# Exponer puerto
EXPOSE 4200

# Iniciar nginx
CMD ["nginx", "-g", "daemon off;"]





