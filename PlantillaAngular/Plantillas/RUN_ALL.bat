@echo off
cls
echo ========================================
echo   PORTFOLIO - EJECUCION COMPLETA
echo ========================================
echo.

REM Verificar carpeta correcta
if not exist "ApHost\PruebaAngular.Api.Host" (
    echo ERROR: Ejecuta desde la carpeta raiz
    pause
    exit /b 1
)

REM Verificar npm
if not exist "API.Client\node_modules" (
    echo Instalando npm...
    cd API.Client
    call npm install --legacy-peer-deps
    cd ..
)

cls
echo ========================================
echo   PORTFOLIO - INICIANDO
echo ========================================
echo.
echo URLS DISPONIBLES:
echo.
echo   Frontend:   http://localhost:4200
echo   Swagger:    http://localhost:5000/swagger
echo   GraphQL:    http://localhost:5000/graphql/portfolio
echo   Dashboard:  https://localhost:17198
echo.
echo ========================================
echo.
echo Espera 30-60 segundos...
echo El frontend tarda en compilar la primera vez.
echo.

cd ApHost\PruebaAngular.Api.Host
dotnet run

