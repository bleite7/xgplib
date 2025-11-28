#!/usr/bin/env pwsh

param(
    [switch]$SkipBuild,
    [switch]$NoBrowser
)

$ErrorActionPreference = "Stop"

if (Test-Path ./TestResults) {
    Remove-Item -Recurse -Force ./TestResults
}

if (-not $SkipBuild) {
    dotnet build --configuration Release
    if ($LASTEXITCODE -ne 0) {
        exit 1
    }
}

dotnet test `
    --configuration Release `
    --no-build `
    --collect:"XPlat Code Coverage" `
    --results-directory ./TestResults `
    --logger "console;verbosity=normal"

if ($LASTEXITCODE -ne 0) {
    exit 1
}

$coverageFiles = Get-ChildItem -Path ./TestResults -Filter "coverage.cobertura.xml" -Recurse

if ($coverageFiles.Count -eq 0) {
    exit 1
}

reportgenerator `
    -reports:"./TestResults/**/coverage.cobertura.xml" `
    -targetdir:"./TestResults/CoverageReport" `
    -reporttypes:"Html;Badges;JsonSummary;TextSummary" `
    -verbosity:"Warning"

if ($LASTEXITCODE -ne 0) {
    exit 1
}

if (-not $NoBrowser) {
    Start-Process ./TestResults/CoverageReport/index.html
}
