# Lance les 7 microservices dans des fenêtres PowerShell séparées
# Le Gateway est lancé en dernier (avec un délai) afin que les services en aval
# soient prêts au moment où Swagger unifié essaie d'agréger leurs définitions.
# ASPNETCORE_ENVIRONMENT=Development : avec --no-launch-profile, charger appsettings.Development (ex. Stripe).

$ErrorActionPreference = 'Stop'
$base = Split-Path -Parent $MyInvocation.MyCommand.Path

$services = @(
    @{ Nom = 'EC_AuthService'        ; Port = 5001 },
    @{ Nom = 'EC_ProductService'     ; Port = 5002 },
    @{ Nom = 'EC_CartService'        ; Port = 5003 },
    @{ Nom = 'EC_OrderService'       ; Port = 5004 },
    @{ Nom = 'EC_PaymentService'     ; Port = 5005 },
    @{ Nom = 'EC_NotificationService'; Port = 5006 }
)

Write-Host "Demarrage des 6 microservices..." -ForegroundColor Cyan

foreach ($svc in $services) {
    $dossier = Join-Path $base $svc.Nom
    $titre   = "$($svc.Nom) (port $($svc.Port))"
    Write-Host "  -> $titre"
    Start-Process powershell -ArgumentList @(
        '-NoExit',
        '-Command',
        "`$env:ASPNETCORE_ENVIRONMENT = 'Development'; `$Host.UI.RawUI.WindowTitle = '$titre'; Set-Location '$dossier'; dotnet run --no-launch-profile --urls http://localhost:$($svc.Port)"
    ) | Out-Null
}

Write-Host ""
Write-Host "Attente de 8 secondes pour laisser les services demarrer..." -ForegroundColor Yellow
Start-Sleep -Seconds 8

$gatewayDir = Join-Path $base 'EC_Gateway'
Write-Host "Demarrage de la passerelle EC_Gateway (port 5000)..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList @(
    '-NoExit',
    '-Command',
    "`$env:ASPNETCORE_ENVIRONMENT = 'Development'; `$Host.UI.RawUI.WindowTitle = 'EC_Gateway (port 5000)'; Set-Location '$gatewayDir'; dotnet run --no-launch-profile --urls http://localhost:5000"
) | Out-Null

Write-Host ""
Write-Host "Tous les services sont en cours de demarrage." -ForegroundColor Green
Write-Host "Ouvrir le Swagger unifie : http://localhost:5000/swagger" -ForegroundColor Green
