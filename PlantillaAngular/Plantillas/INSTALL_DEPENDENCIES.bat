@echo off
REM Script simple para instalar dependencias y ejecutar

setlocal enabledelayedexpansion

echo.
echo ====================================
echo   INSTALANDO DEPENDENCIAS
echo ====================================
echo.

cd API.Client

echo Limpiando instalación anterior...
if exist node_modules rmdir /s /q node_modules
if exist package-lock.json del package-lock.json

echo.
echo Instalando dependencias (esto puede tardar 3-5 minutos)...
echo.

call npm install --legacy-peer-deps

if !errorlevel! equ 0 (
    echo.
    echo ====================================
    echo   INSTALACION COMPLETADA
    echo ====================================
    echo.
    echo Ahora ejecuta en DOS TERMINALES DIFERENTES:
    echo.
    echo Terminal 1:
    echo   cd ApHost\PruebaAngular.Api.Host
    echo   dotnet run
    echo.
    echo Terminal 2:
    echo   cd API.Client
    echo   npm start
    echo.
    echo Luego accede a: http://localhost:4200
    echo.
) else (
    echo.
    echo ERROR: Instalacion fallé
    echo.
)

pause
