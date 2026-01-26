# ?? Portfolio Full-Stack Application

Aplicación de portfolio full-stack desarrollada con **.NET 10**, **Angular 17**, **GraphQL** y **PostgreSQL**.

---

## ?? Requisitos Previos

Antes de empezar, asegúrate de tener instalado:

| Requisito | Versión mínima | Verificar instalación |
|-----------|----------------|----------------------|
| **.NET SDK** | 10.0 | `dotnet --version` |
| **Node.js** | 18.x | `node --version` |
| **Docker Desktop** | - | Debe estar **ejecutándose** |

> ?? **IMPORTANTE**: Docker Desktop debe estar abierto y funcionando antes de ejecutar la aplicación.

---

## ?? Cómo Ejecutar el Proyecto

### Paso 1: Clonar el repositorio

```bash
git clone https://github.com/Ronerto201291/Porfolio.git
cd Porfolio/PlantillaAngular/Plantillas
```

### Paso 2: Instalar dependencias del frontend

```bash
cd API.Client
npm install
cd ..
```

### Paso 3: Ejecutar la aplicación

Desde la raíz del proyecto (`PlantillaAngular/Plantillas`):

```bash
dotnet run --project ApHost/PruebaAngular.Api.Host
```

> Esto iniciará automáticamente:
> - ?? PostgreSQL (contenedor Docker)
> - ? API .NET (Backend)
> - ??? Angular (Frontend)

### Paso 4: Inicializar la base de datos

Una vez que todo esté corriendo, abre **PowerShell** y ejecuta:

```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/setup/init" -Method Post
```

> Esto crea las tablas y datos de ejemplo en PostgreSQL.

### Paso 5: Acceder a la aplicación

| Servicio | URL |
|----------|-----|
| **Frontend** | http://localhost:4200 |
| **Swagger (API REST)** | http://localhost:5000/swagger |
| **GraphQL Playground** | http://localhost:5000/graphql/portfolio |
| **Dashboard Aspire** | Se abre automáticamente |

---

## ??? Estructura del Proyecto

```
PlantillaAngular/Plantillas/
??? ApHost/                          # .NET Aspire (orquestador)
?   ??? PruebaAngular.Api.Host/
??? API/                             # Backend .NET
?   ??? PruebaAngular.Api/           # Capa API (Controllers, GraphQL)
?   ??? PruebaAngular.Application/   # Capa Aplicación (Commands, Handlers)
?   ??? PruebaAngular.Domain/        # Capa Dominio (Entidades)
?   ??? PruebaAngular.Infrastructure.Data/  # Capa Infraestructura (EF Core)
??? API.Client/                      # Frontend Angular
    ??? src/app/
        ??? aplicacion/              # Componentes principales (Kanban, Proyectos)
        ??? pages/                   # Páginas (Docs, About, Architecture)
        ??? layout/                  # Navegación y estructura
        ??? services/                # Servicios (GraphQL client)
```

---

## ??? Tecnologías Utilizadas

### Backend
- **.NET 10** - Framework principal
- **ASP.NET Core** - Web API
- **GraphQL (HotChocolate)** - API flexible
- **Entity Framework Core** - ORM
- **PostgreSQL** - Base de datos

### Frontend
- **Angular 17** - Framework SPA
- **TypeScript** - Lenguaje tipado
- **SCSS** - Estilos
- **RxJS** - Programación reactiva

### Infraestructura
- **.NET Aspire** - Orquestación de servicios
- **Docker** - Contenedores
- **PostgreSQL** - Contenedor de BD

### Patrones de Arquitectura
- **Clean Architecture** - Separación de capas
- **CQRS** - Separación de lecturas/escrituras
- **MediatR** - Mediador de comandos

---

## ?? Comandos Útiles

### Limpiar y reconstruir

```bash
# Limpiar volumen de PostgreSQL (resetear BD)
docker volume rm portfolio-postgres-data -f

# Reconstruir solución
dotnet build
```

### Ejecutar solo el frontend (desarrollo)

```bash
cd API.Client
npm start
```

---

## ? Solución de Problemas

### Error: "No se puede conectar a PostgreSQL"

1. Verifica que **Docker Desktop esté ejecutándose**
2. Elimina el volumen y reinicia:
   ```bash
   docker volume rm portfolio-postgres-data -f
   ```
3. Vuelve a ejecutar el proyecto

### Error: "relation does not exist"

Ejecuta la inicialización de la base de datos:
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/setup/init" -Method Post
```

### El frontend no carga

1. Verifica que las dependencias estén instaladas:
   ```bash
   cd API.Client && npm install
   ```
2. Reinicia el proyecto completo

### Puerto ocupado

Si el puerto 5000 o 4200 está ocupado:
```powershell
# Ver qué proceso usa el puerto
netstat -ano | findstr :5000
# Matar el proceso (reemplaza PID)
taskkill /PID <PID> /F
```

---

## ?? Autor

**Roberto Noguera Cuellar**  
.NET Full Stack Developer | Team Leader

- ?? blanco.cuellar.r@gmail.com
- ?? [LinkedIn](https://www.linkedin.com/in/rnogueracuellar)