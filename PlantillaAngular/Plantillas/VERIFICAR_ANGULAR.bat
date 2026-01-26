@echo off
cls
echo ========================================
echo   PORTFOLIO - DIAGNOSTICO
echo ========================================
echo.

cd API.Client

echo [1/4] Verificando node_modules...
if not exist "node_modules" (
    echo ERROR: node_modules no existe
    echo Ejecutando npm install...
    call npm install --legacy-peer-deps
) else (
    echo OK: node_modules existe
)

echo.
echo [2/4] Verificando package.json...
findstr /C:"ng serve" package.json >nul
if %errorlevel% equ 0 (
    echo OK: npm start configurado
) else (
    echo ERROR: npm start no configurado correctamente
)

echo.
echo [3/4] Limpiando cache de Angular...
if exist ".angular" (
    rmdir /s /q .angular
    echo OK: Cache limpiado
)

echo.
echo [4/4] Compilando Angular manualmente...
echo Esto puede tardar 2-5 minutos...
echo.

call npm run build

echo.
if exist "dist\PruebaAngularFront" (
    echo ========================================
    echo   COMPILACION EXITOSA
    echo ========================================
    echo.
    echo Ahora ejecuta Aspire:
    echo   cd ..\ApHost\PruebaAngular.Api.Host
    echo   dotnet run
    echo.
) else (
    echo ========================================
    echo   ERROR: COMPILACION FALLO
    echo ========================================
    echo.
    echo Revisa los errores arriba.
)

pause
