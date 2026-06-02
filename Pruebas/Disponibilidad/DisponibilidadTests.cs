using System.Diagnostics;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Pruebas.Disponibilidad;

/// <summary>
/// Punto 1 — Parte A: Pruebas de Disponibilidad
/// 
/// Valida que cada microservicio responda con código HTTP 200
/// y en menos de 2 segundos, equivalente a las pruebas de disponibilidad
/// realizadas en el taller JEE con Postman.
/// </summary>
public class DisponibilidadTests : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly ITestOutputHelper _output;

    // Puertos según docker-compose.yml de MicroServicios
    private const string NotasUrl           = "http://localhost:5010";
    private const string CursosAcabadosUrl  = "http://localhost:5011";
    private const string ClasesMasTomadas   = "http://localhost:5012";
    private const string MejoresEstudiantes = "http://localhost:5013";
    private const int UsuarioIdPrueba       = 1;
    private const int TiempoLimiteMs        = 2000;

    public DisponibilidadTests(ITestOutputHelper output)
    {
        _output = output;
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(5)
        };
    }

    // ─────────────────────────────────────────────────────
    // MICROSERVICIO: Notas  (puerto 5010)
    // ─────────────────────────────────────────────────────

    [Fact(DisplayName = "DIS-01: Notas - endpoint /api/notas/estudiante/{id} responde HTTP 200")]
    public async Task Notas_EndpointEstudiante_DebeResponder200()
    {
        var url = $"{NotasUrl}/api/notas/estudiante/{UsuarioIdPrueba}";
        var sw  = Stopwatch.StartNew();

        var response = await _httpClient.GetAsync(url);
        sw.Stop();

        _output.WriteLine($"[DIS-01] GET {url}");
        _output.WriteLine($"  Status : {(int)response.StatusCode} {response.StatusCode}");
        _output.WriteLine($"  Tiempo : {sw.ElapsedMilliseconds} ms");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK,
            "el microservicio Notas debe estar disponible y devolver HTTP 200");
    }

    [Fact(DisplayName = "DIS-02: Notas - respuesta en menos de 2 segundos")]
    public async Task Notas_EndpointEstudiante_DebeResponderEnMenos2Segundos()
    {
        var url = $"{NotasUrl}/api/notas/estudiante/{UsuarioIdPrueba}";
        var sw  = Stopwatch.StartNew();

        await _httpClient.GetAsync(url);
        sw.Stop();

        _output.WriteLine($"[DIS-02] Tiempo de respuesta Notas: {sw.ElapsedMilliseconds} ms (límite: {TiempoLimiteMs} ms)");

        sw.ElapsedMilliseconds.Should().BeLessThan(TiempoLimiteMs,
            $"la respuesta debe llegar en menos de {TiempoLimiteMs} ms");
    }

    [Fact(DisplayName = "DIS-03: Notas - endpoint /promedio responde HTTP 200")]
    public async Task Notas_EndpointPromedio_DebeResponder200()
    {
        var url = $"{NotasUrl}/api/notas/estudiante/{UsuarioIdPrueba}/promedio";
        var sw  = Stopwatch.StartNew();

        var response = await _httpClient.GetAsync(url);
        sw.Stop();

        _output.WriteLine($"[DIS-03] GET {url} → {(int)response.StatusCode} en {sw.ElapsedMilliseconds} ms");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        sw.ElapsedMilliseconds.Should().BeLessThan(TiempoLimiteMs);
    }

    // ─────────────────────────────────────────────────────
    // MICROSERVICIO: CursosAcabados  (puerto 5011)
    // ─────────────────────────────────────────────────────

    [Fact(DisplayName = "DIS-04: CursosAcabados - endpoint /api/cursos-acabados/estudiante/{id} responde HTTP 200")]
    public async Task CursosAcabados_EndpointEstudiante_DebeResponder200()
    {
        var url = $"{CursosAcabadosUrl}/api/cursos-acabados/estudiante/{UsuarioIdPrueba}";
        var sw  = Stopwatch.StartNew();

        var response = await _httpClient.GetAsync(url);
        sw.Stop();

        _output.WriteLine($"[DIS-04] GET {url} → {(int)response.StatusCode} en {sw.ElapsedMilliseconds} ms");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK,
            "el microservicio CursosAcabados debe estar disponible");
        sw.ElapsedMilliseconds.Should().BeLessThan(TiempoLimiteMs);
    }

    [Fact(DisplayName = "DIS-05: CursosAcabados - endpoint /total responde HTTP 200")]
    public async Task CursosAcabados_EndpointTotal_DebeResponder200()
    {
        var url = $"{CursosAcabadosUrl}/api/cursos-acabados/estudiante/{UsuarioIdPrueba}/total";
        var sw  = Stopwatch.StartNew();

        var response = await _httpClient.GetAsync(url);
        sw.Stop();

        _output.WriteLine($"[DIS-05] GET {url} → {(int)response.StatusCode} en {sw.ElapsedMilliseconds} ms");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        sw.ElapsedMilliseconds.Should().BeLessThan(TiempoLimiteMs);
    }

    // ─────────────────────────────────────────────────────
    // MICROSERVICIO: ClasesMasTomadas  (puerto 5012)
    // ─────────────────────────────────────────────────────

    [Fact(DisplayName = "DIS-06: ClasesMasTomadas - endpoint /api/clases-mas-tomadas responde HTTP 200")]
    public async Task ClasesMasTomadas_EndpointGeneral_DebeResponder200()
    {
        var url = $"{ClasesMasTomadas}/api/clases-mas-tomadas";
        var sw  = Stopwatch.StartNew();

        var response = await _httpClient.GetAsync(url);
        sw.Stop();

        _output.WriteLine($"[DIS-06] GET {url} → {(int)response.StatusCode} en {sw.ElapsedMilliseconds} ms");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK,
            "el microservicio ClasesMasTomadas debe estar disponible");
        sw.ElapsedMilliseconds.Should().BeLessThan(TiempoLimiteMs);
    }

    [Fact(DisplayName = "DIS-07: ClasesMasTomadas - endpoint /top/{n} responde HTTP 200")]
    public async Task ClasesMasTomadas_EndpointTop_DebeResponder200()
    {
        var url = $"{ClasesMasTomadas}/api/clases-mas-tomadas/top/5";
        var sw  = Stopwatch.StartNew();

        var response = await _httpClient.GetAsync(url);
        sw.Stop();

        _output.WriteLine($"[DIS-07] GET {url} → {(int)response.StatusCode} en {sw.ElapsedMilliseconds} ms");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        sw.ElapsedMilliseconds.Should().BeLessThan(TiempoLimiteMs);
    }

    // ─────────────────────────────────────────────────────
    // MICROSERVICIO: MejoresEstudiantes  (puerto 5013)
    // ─────────────────────────────────────────────────────

    [Fact(DisplayName = "DIS-08: MejoresEstudiantes - endpoint /api/mejores-estudiantes responde HTTP 200")]
    public async Task MejoresEstudiantes_EndpointGeneral_DebeResponder200()
    {
        var url = $"{MejoresEstudiantes}/api/mejores-estudiantes";
        var sw  = Stopwatch.StartNew();

        var response = await _httpClient.GetAsync(url);
        sw.Stop();

        _output.WriteLine($"[DIS-08] GET {url} → {(int)response.StatusCode} en {sw.ElapsedMilliseconds} ms");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK,
            "el microservicio MejoresEstudiantes debe estar disponible");
        sw.ElapsedMilliseconds.Should().BeLessThan(TiempoLimiteMs);
    }

    [Fact(DisplayName = "DIS-09: MejoresEstudiantes - endpoint /top/{n} responde HTTP 200")]
    public async Task MejoresEstudiantes_EndpointTop_DebeResponder200()
    {
        var url = $"{MejoresEstudiantes}/api/mejores-estudiantes/top/5";
        var sw  = Stopwatch.StartNew();

        var response = await _httpClient.GetAsync(url);
        sw.Stop();

        _output.WriteLine($"[DIS-09] GET {url} → {(int)response.StatusCode} en {sw.ElapsedMilliseconds} ms");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        sw.ElapsedMilliseconds.Should().BeLessThan(TiempoLimiteMs);
    }

    public void Dispose() => _httpClient.Dispose();
}
