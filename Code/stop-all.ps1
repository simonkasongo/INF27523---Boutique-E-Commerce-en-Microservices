# Arrete tous les processus dotnet qui ecoutent sur les ports 5000-5006

$ports = 5000..5006

foreach ($port in $ports) {
    $conn = Get-NetTCPConnection -LocalPort $port -State Listen -ErrorAction SilentlyContinue
    if ($conn) {
        $pidProc = $conn.OwningProcess
        try {
            Stop-Process -Id $pidProc -Force -ErrorAction Stop
            Write-Host "Arrete : port $port (PID $pidProc)" -ForegroundColor Yellow
        } catch {
            Write-Host "Impossible d'arreter le PID $pidProc sur le port $port" -ForegroundColor Red
        }
    }
}

Write-Host "Termine." -ForegroundColor Green
