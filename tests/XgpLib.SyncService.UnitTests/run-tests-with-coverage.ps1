#!/usr/bin/env pwsh
# Script para executar testes unitários com cobertura de código

param(
    [switch]$SkipBuild,
    [switch]$NoBrowser
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  XgpLib - Testes Unitários & Cobertura" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Limpar resultados anteriores
if (Test-Path ./TestResults) {
    Write-Host "Limpando resultados anteriores..." -ForegroundColor Yellow
    Remove-Item -Recurse -Force ./TestResults
}

# Build (opcional)
if (-not $SkipBuild) {
    Write-Host "Compilando projeto de testes..." -ForegroundColor Green
    dotnet build --configuration Release
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Erro ao compilar o projeto!" -ForegroundColor Red
        exit 1
    }
    Write-Host ""
}

# Executar testes com cobertura
Write-Host "Executando testes unitários..." -ForegroundColor Green
dotnet test `
    --configuration Release `
    --no-build `
    --collect:"XPlat Code Coverage" `
    --results-directory ./TestResults `
    --logger "console;verbosity=normal"

if ($LASTEXITCODE -ne 0) {
    Write-Host "Alguns testes falharam!" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Verificar se há arquivos de cobertura
$coverageFiles = Get-ChildItem -Path ./TestResults -Filter "coverage.cobertura.xml" -Recurse

if ($coverageFiles.Count -eq 0) {
    Write-Host "Nenhum arquivo de cobertura encontrado!" -ForegroundColor Red
    exit 1
}

# Gerar relatório HTML
Write-Host "Gerando relatório de cobertura..." -ForegroundColor Green
reportgenerator `
    -reports:"./TestResults/**/coverage.cobertura.xml" `
    -targetdir:"./TestResults/CoverageReport" `
    -reporttypes:"Html;Badges;JsonSummary;TextSummary" `
    -verbosity:"Warning"

if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro ao gerar relatório de cobertura!" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Exibir resumo da cobertura
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Resumo da Cobertura" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

if (Test-Path ./TestResults/CoverageReport/Summary.txt) {
    Get-Content ./TestResults/CoverageReport/Summary.txt | Write-Host
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Relatório completo: ./TestResults/CoverageReport/index.html" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Abrir relatório no navegador
if (-not $NoBrowser) {
    Write-Host "Abrindo relatório no navegador..." -ForegroundColor Green
    Start-Process ./TestResults/CoverageReport/index.html
}

Write-Host ""
Write-Host "✅ Testes concluídos com sucesso!" -ForegroundColor Green
