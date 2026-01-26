@echo off
REM Script para levantar la aplicación completa

setlocal enabledelayedexpansion

echo.
echo ====================================
echo   PORTFOLIO APPLICATION LAUNCHER
echo ====================================
echo.

REM Verificar si estamos en la carpeta correcta
if not exist "ApHost\PruebaAngular.Api.Host" (
    echo ERROR: Ejecuta este script desde la carpeta raiz (Plantillas)
    exit /b 1
)

REM Instalar dependencias de npm (si node_modules no existe)
if not exist "API.Client\node_modules" (
    echo.
    echo [1/3] Instalando dependencias de npm...
    cd API.Client
    call npm install
    cd ..
    if !errorlevel! neq 0 (
        echo ERROR: npm install fallé
        exit /b 1
    )
)

echo.
echo [2/3] Iniciando Backend API (.NET)...
echo - Accede a Aspire Dashboard en: https://localhost:17198
echo - API en: http://localhost:5000
echo.

start cmd /k "cd ApHost\PruebaAngular.Api.Host && dotnet run"

echo.
echo Esperando 5 segundos para que el backend inicie...
timeout /t 5 /nobreak

echo.
echo [3/3] Iniciando Frontend (Angular)...
echo - Accede a la app en: http://localhost:4200
echo.

start cmd /k "cd API.Client && npm start"

echo.
echo ====================================
echo   APLICACIÓN EN EJECUCIÓN
echo ====================================
echo.
echo Frontend:  http://localhost:4200
echo Backend:   http://localhost:5000
echo Swagger:   http://localhost:5000/swagger
echo Dashboard: https://localhost:17198
echo.
echo Presiona cualquier tecla para cerrar esta ventana...
pause
