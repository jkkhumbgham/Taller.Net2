<#
.SYNOPSIS
    Punto 1 — Parte B: Prueba de Escalabilidad (.NET)
    Equivalente al script PowerShell del Taller JEE.

.DESCRIPTION
    Lanza 500 peticiones concurrentes contra los 4 microservicios .NET
    y reporta: total, exitosas, % éxito, tiempo promedio y P95.

.EXAMPLE
    .\EscalabilidadTest.ps1
    .\EscalabilidadTest.ps1 -Concurrencia 100
#>

param(
    [int]$Concurrencia = 500
)

# ── Configuración de endpoints ────────────────────────────────────────────────
$Endpoints = @{
    "Notas"              = "http://localhost:5010/api/notas/estudiante/1"
    "CursosAcabados"     = "http://localhost:5011/api/cursos-acabados/estudiante/1"
    "ClasesMasTomadas"   = "http://localhost:5012/api/clases-mas-tomadas/top/10"
    "MejoresEstudiantes" = "http://localhost:5013/api/mejores-estudiantes/top/5"
}

# ── Función: ejecutar N peticiones concurrentes a una URL ─────────────────────
function Invoke-CargaConcurrente {
    param(
        [string]$NombreServicio,
        [string]$Url,
        [int]$Total
    )

    Write-Host "`n🔄  Probando $NombreServicio ($Total peticiones concurrentes)..."
    Write-Host "    URL: $Url"

    $tiempos    = [System.Collections.Concurrent.ConcurrentBag[double]]::new()
    $exitosas   = [System.Threading.Interlocked]::new()
    $exitosRef  = [ref] 0
    $failRef    = [ref] 0

    $jobs = 1..$Total | ForEach-Object {
        [System.Threading.Tasks.Task]::Run({
            $sw = [System.Diagnostics.Stopwatch]::StartNew()
            try {
                $resp = Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 10 -ErrorAction Stop
                $sw.Stop()
                if ($resp.StatusCode -in 200, 201, 204) {
                    [System.Threading.Interlocked]::Increment($exitosRef) | Out-Null
                } else {
                    [System.Threading.Interlocked]::Increment($failRef) | Out-Null
                }
            } catch {
                $sw.Stop()
                [System.Threading.Interlocked]::Increment($failRef) | Out-Null
            }
            $tiempos.Add($sw.Elapsed.TotalMilliseconds)
        }.GetAwaiter())
    }

    [System.Threading.Tasks.Task]::WaitAll($jobs)

    $listaMs  = $tiempos | Sort-Object
    $exitCount = $exitosRef.Value
    $promedio  = if ($listaMs.Count -gt 0) { [Math]::Round(($listaMs | Measure-Object -Average).Average, 2) } else { 0 }
    $p95Index  = [Math]::Ceiling($listaMs.Count * 0.95) - 1
    $p95       = if ($p95Index -ge 0 -and $listaMs.Count -gt 0) { [Math]::Round($listaMs[$p95Index], 2) } else { 0 }
    $pct       = [Math]::Round(($exitCount / $Total) * 100, 1)
    $aprobado  = if ($pct -ge 95) { "✅ Aprobado" } else { "❌ No aprobado" }

    return [PSCustomObject]@{
        Servicio    = $NombreServicio
        Peticiones  = $Total
        Exitosas    = $exitCount
        PctExito    = "$pct%"
        Promedio_ms = $promedio
        P95_ms      = $p95
        Resultado   = $aprobado
    }
}

# ── Ejecutar prueba para cada servicio ───────────────────────────────────────
Write-Host "═══════════════════════════════════════════════════════════"
Write-Host "   PRUEBA DE ESCALABILIDAD — $Concurrencia peticiones concurrentes"
Write-Host "   Taller No. 4 — .NET MicroServicios"
Write-Host "═══════════════════════════════════════════════════════════"

$resultados = @()
foreach ($entry in $Endpoints.GetEnumerator()) {
    $resultados += Invoke-CargaConcurrente -NombreServicio $entry.Key -Url $entry.Value -Total $Concurrencia
}

# ── Tabla de resultados ───────────────────────────────────────────────────────
Write-Host "`n`n════════════ RESULTADOS ════════════"
$resultados | Format-Table -AutoSize

# ── Guardar CSV ───────────────────────────────────────────────────────────────
$csvPath = ".\resultados_escalabilidad_$(Get-Date -Format 'yyyyMMdd_HHmmss').csv"
$resultados | Export-Csv -Path $csvPath -NoTypeInformation -Encoding UTF8
Write-Host "📄 Resultados guardados en: $csvPath"
