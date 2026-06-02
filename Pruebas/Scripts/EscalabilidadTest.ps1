<#
.SYNOPSIS
    Prueba de Escalabilidad para Microservicios .NET

.DESCRIPTION
    Ejecuta múltiples peticiones concurrentes a cada microservicio
    y calcula:
    - Total peticiones
    - Exitosas
    - % éxito
    - Tiempo promedio
    - Percentil 95 (P95)

.EXAMPLE
    pwsh EscalabilidadTest.ps1

.EXAMPLE
    pwsh EscalabilidadTest.ps1 -Concurrencia 100
#>

param(
    [int]$Concurrencia = 500
)

$Endpoints = @{
    "Notas"              = "http://localhost:5010/api/notas/estudiante/1"
    "CursosAcabados"     = "http://localhost:5011/api/cursos-acabados/estudiante/1"
    "ClasesMasTomadas"   = "http://localhost:5012/api/clases-mas-tomadas/top/10"
    "MejoresEstudiantes" = "http://localhost:5013/api/mejores-estudiantes/top/5"
}

function Invoke-CargaConcurrente {
    param(
        [string]$NombreServicio,
        [string]$Url,
        [int]$Total
    )

    Write-Host ""
    Write-Host "════════════════════════════════════════════"
    Write-Host "Probando: $NombreServicio"
    Write-Host "URL: $Url"
    Write-Host "Peticiones concurrentes: $Total"
    Write-Host "════════════════════════════════════════════"

    $resultados = 1..$Total |
        ForEach-Object -Parallel {

            $endpoint = $using:Url

            $sw = [System.Diagnostics.Stopwatch]::StartNew()

            try {

                $resp = Invoke-WebRequest `
                    -Uri $endpoint `
                    -TimeoutSec 10 `
                    -ErrorAction Stop

                $sw.Stop()

                [PSCustomObject]@{
                    Exito   = ($resp.StatusCode -eq 200)
                    Tiempo  = $sw.Elapsed.TotalMilliseconds
                }
            }
            catch {

                $sw.Stop()

                [PSCustomObject]@{
                    Exito   = $false
                    Tiempo  = $sw.Elapsed.TotalMilliseconds
                }
            }

        } -ThrottleLimit 100

    $tiempos = $resultados.Tiempo | Sort-Object

    $exitosas = ($resultados | Where-Object { $_.Exito }).Count

    $promedio = if ($tiempos.Count -gt 0) {
        [Math]::Round(($tiempos | Measure-Object -Average).Average, 2)
    }
    else {
        0
    }

    $p95Index = [Math]::Ceiling($tiempos.Count * 0.95) - 1

    $p95 = if ($tiempos.Count -gt 0) {
        [Math]::Round($tiempos[$p95Index], 2)
    }
    else {
        0
    }

    $porcentajeExito = [Math]::Round(($exitosas / $Total) * 100, 2)

    $resultadoFinal = if ($porcentajeExito -ge 95) {
        "✅ Aprobado"
    }
    else {
        "❌ No aprobado"
    }

    return [PSCustomObject]@{
        Servicio    = $NombreServicio
        Peticiones  = $Total
        Exitosas    = $exitosas
        PctExito    = "$porcentajeExito%"
        Promedio_ms = $promedio
        P95_ms      = $p95
        Resultado   = $resultadoFinal
    }
}

Write-Host ""
Write-Host "═══════════════════════════════════════════════════════════════"
Write-Host "     PRUEBA DE ESCALABILIDAD - MICROSERVICIOS .NET"
Write-Host "═══════════════════════════════════════════════════════════════"
Write-Host "Concurrencia: $Concurrencia"
Write-Host ""

$resultadosFinales = @()

foreach ($endpoint in $Endpoints.GetEnumerator()) {

    $resultado = Invoke-CargaConcurrente `
        -NombreServicio $endpoint.Key `
        -Url $endpoint.Value `
        -Total $Concurrencia

    $resultadosFinales += $resultado
}

Write-Host ""
Write-Host "════════════ RESULTADOS FINALES ════════════"
$resultadosFinales | Format-Table -AutoSize

$csvPath = "./resultados_escalabilidad_$(Get-Date -Format 'yyyyMMdd_HHmmss').csv"

$resultadosFinales | Export-Csv `
    -Path $csvPath `
    -NoTypeInformation `
    -Encoding UTF8

Write-Host ""
Write-Host "Resultados guardados en:"
Write-Host $csvPath