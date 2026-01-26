# Script para ejecutar la aplicación completa (PowerShell)
# Ejecutar como: powershell -ExecutionPolicy Bypass -File START_APP.ps1

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  PORTFOLIO APPLICATION LAUNCHER" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Verificar que estamos en la carpeta correcta
if (-not (Test-Path "ApHost\PruebaAngular.Api.Host")) {
    Write-Host "ERROR: Ejecuta este script desde la carpeta raiz (Plantillas)" -ForegroundColor Red
    exit 1
}

# Paso 1: Instalar dependencias
Write-Host "[1/3] Verificando dependencias de npm..." -ForegroundColor Yellow

if (-not (Test-Path "API.Client\node_modules")) {
    Write-Host "Instalando dependencias (esto puede tardar varios minutos)..." -ForegroundColor Yellow
    Set-Location "API.Client"
    
    try {
        npm install --legacy-peer-deps
        if ($LASTEXITCODE -ne 0) {
            Write-Host "ERROR: npm install falló" -ForegroundColor Red
            exit 1
        }
    }
    catch {
        Write-Host "ERROR: No se puede instalar dependencias" -ForegroundColor Red
        exit 1
    }
    
    Set-Location ".."
} else {
    Write-Host "Dependencias ya instaladas ?" -ForegroundColor Green
}

# Verificar que npm start funciona
Write-Host "Verificando configuración de npm..." -ForegroundColor Yellow

$packageContent = Get-Content "API.Client\package.json" -Raw | ConvertFrom-Json
if ($packageContent.scripts.start -notmatch "ng serve") {
    Write-Host "ERROR: npm start no está configurado correctamente" -ForegroundColor Red
    exit 1
}

Write-Host "Configuración verificada ?" -ForegroundColor Green

# Paso 2: Verificar puertos
Write-Host ""
Write-Host "[2/3] Verificando puertos..." -ForegroundColor Yellow

$port4200 = netstat -ano | findstr ":4200"
$port5000 = netstat -ano | findstr ":5000"

if ($port4200) {
    Write-Host "ADVERTENCIA: Puerto 4200 ya está en uso" -ForegroundColor Yellow
    Write-Host "  (Puede ser un proceso anterior. Se reiniciará)" -ForegroundColor Yellow
}

if ($port5000) {
    Write-Host "ADVERTENCIA: Puerto 5000 ya está en uso" -ForegroundColor Yellow
}

# Paso 3: Iniciar servicios
Write-Host ""
Write-Host "[3/3] Iniciando servicios..." -ForegroundColor Yellow
Write-Host ""

# Mostrar información de acceso
Write-Host "=====================================" -ForegroundColor Green
Write-Host "  APLICACIÓN EN EJECUCIÓN" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green
Write-Host ""
Write-Host "Frontend:  http://localhost:4200" -ForegroundColor Cyan
Write-Host "Backend:   http://localhost:5000" -ForegroundColor Cyan
Write-Host "Swagger:   http://localhost:5000/swagger" -ForegroundColor Cyan
Write-Host "Dashboard: https://localhost:17198" -ForegroundColor Cyan
Write-Host ""

# Iniciar Backend
Write-Host "Iniciando Backend API (.NET)..." -ForegroundColor Yellow
Write-Host "  (Se abrirá en una nueva ventana)" -ForegroundColor Gray
Write-Host ""

Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\ApHost\PruebaAngular.Api.Host'; dotnet run"

# Esperar a que el backend inicie
Write-Host "Esperando 8 segundos para que el backend inicie..." -ForegroundColor Gray
Start-Sleep -Seconds 8

# Iniciar Frontend
Write-Host "Iniciando Frontend (Angular)..." -ForegroundColor Yellow
Write-Host "  (Se abrirá en una nueva ventana)" -ForegroundColor Gray
Write-Host ""

Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\API.Client'; npm start"

Write-Host ""
Write-Host "=====================================" -ForegroundColor Green
Write-Host "  ? SERVICIOS INICIADOS" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green
Write-Host ""
Write-Host "La aplicación se abrirá automáticamente en:" -ForegroundColor Gray
Write-Host "  http://localhost:4200" -ForegroundColor Cyan
Write-Host ""
Write-Host "Si algo falla, revisa TROUBLESHOOTING.md" -ForegroundColor Yellow
Write-Host ""
