using System.Net;
using System.Text.Json;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Pruebas.Aceptacion;

/// <summary>
/// Punto 3 — Pruebas de Aceptación
///
/// Verifican que los módulos del sistema .NET cumplen los criterios
/// funcionales esperados por el usuario final, equivalente a las
/// pruebas de aceptación del taller JEE.
///
/// Criterios de aceptación por módulo:
///   - Notas (5010)            → debe devolver calificaciones reales del DB
///   - CursosAcabados (5011)   → debe identificar cursos completados al 100%
///   - ClasesMasTomadas (5012) → debe rankear cursos por inscripciones
///   - MejoresEstudiantes(5013)→ debe rankear estudiantes por puntaje
/// </summary>
public class AceptacionTests : IDisposable
{
    private readonly HttpClient _http;
    private readonly ITestOutputHelper _output;

    private const string NotasUrl           = "http://localhost:5010";
    private const string CursosAcabadosUrl  = "http://localhost:5011";
    private const string ClasesMasTomadas   = "http://localhost:5012";
    private const string MejoresEstudiantes = "http://localhost:5013";
    private const int UserId                = 1;

    public AceptacionTests(ITestOutputHelper output)
    {
        _output = output;
        _http   = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
    }

    private async Task<JsonElement[]> GetArray(string url)
    {
        var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"  Respuesta: {body[..Math.Min(300, body.Length)]}");
        return JsonSerializer.Deserialize<JsonElement[]>(body)!;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // AC-01: Notas — el sistema devuelve calificaciones reales para el usuario 1
    // ─────────────────────────────────────────────────────────────────────────

    [Fact(DisplayName = "AC-01: Notas — el sistema devuelve calificaciones cargadas en la BD")]
    public async Task Notas_DebeRetornarCalificacionesReales()
    {
        var url = $"{NotasUrl}/api/notas/estudiante/{UserId}";
        _output.WriteLine($"[AC-01] GET {url}");

        var items = await GetArray(url);

        items.Should().NotBeNull("el sistema debe responder");
        items.Length.Should().BeGreaterThan(0,
            "el estudiante 1 debe tener calificaciones cargadas en la BD de datos de prueba");

        _output.WriteLine($"  → {items.Length} calificacion(es) encontrada(s). ✅ Aceptado");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // AC-02: Notas/Top — el top N limita correctamente el resultado
    // ─────────────────────────────────────────────────────────────────────────

    [Theory(DisplayName = "AC-02: ClasesMasTomadas/Top — respeta el límite N solicitado")]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public async Task ClasesMasTomadas_Top_RespetaLimiteN(int n)
    {
        var url = $"{ClasesMasTomadas}/api/clases-mas-tomadas/top/{n}";
        _output.WriteLine($"[AC-02] GET {url} (top {n})");

        var items = await GetArray(url);

        items.Length.Should().BeLessThanOrEqualTo(n,
            $"el endpoint /top/{n} no debe devolver más de {n} resultados");

        _output.WriteLine($"  → Devolvió {items.Length} elemento(s) para top/{n}. ✅ Aceptado");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // AC-03: MejoresEstudiantes — el ranking no está vacío
    // ─────────────────────────────────────────────────────────────────────────

    [Fact(DisplayName = "AC-03: MejoresEstudiantes — el ranking contiene estudiantes de la BD")]
    public async Task MejoresEstudiantes_RankingNoEstaVacio()
    {
        var url = $"{MejoresEstudiantes}/api/mejores-estudiantes";
        _output.WriteLine($"[AC-03] GET {url}");

        var items = await GetArray(url);

        items.Should().NotBeNull();
        items.Length.Should().BeGreaterThan(0,
            "el ranking debe contener al menos un estudiante (la BD de prueba tiene datos)");

        _output.WriteLine($"  → {items.Length} estudiante(s) en el ranking. ✅ Aceptado");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // AC-04: MejoresEstudiantes/Top5 — devuelve máximo 5
    // ─────────────────────────────────────────────────────────────────────────

    [Fact(DisplayName = "AC-04: MejoresEstudiantes/top/5 — devuelve entre 1 y 5 estudiantes")]
    public async Task MejoresEstudiantes_Top5_DevuelveHasta5()
    {
        var url = $"{MejoresEstudiantes}/api/mejores-estudiantes/top/5";
        _output.WriteLine($"[AC-04] GET {url}");

        var items = await GetArray(url);

        items.Length.Should().BeInRange(1, 5,
            "el top 5 debe devolver entre 1 y 5 estudiantes");

        _output.WriteLine($"  → {items.Length} estudiante(s). ✅ Aceptado");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // AC-05: CursosAcabados — responde para el estudiante 1
    // ─────────────────────────────────────────────────────────────────────────

    [Fact(DisplayName = "AC-05: CursosAcabados — el sistema procesa la consulta del estudiante 1")]
    public async Task CursosAcabados_ProcesaConsultaEstudiante1()
    {
        var url = $"{CursosAcabadosUrl}/api/cursos-acabados/estudiante/{UserId}";
        _output.WriteLine($"[AC-05] GET {url}");

        var response = await _http.GetAsync(url);
        var body     = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"  Status : {(int)response.StatusCode}");
        _output.WriteLine($"  Cuerpo : {body[..Math.Min(200, body.Length)]}");

        response.StatusCode.Should().Be(HttpStatusCode.OK,
            "el sistema debe procesar la consulta del estudiante 1 sin errores");

        body.Should().NotBeNullOrWhiteSpace(
            "la respuesta no debe estar vacía");

        _output.WriteLine("  ✅ Aceptado");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // AC-06: CursosAcabados/Total — devuelve conteo numérico
    // ─────────────────────────────────────────────────────────────────────────

    [Fact(DisplayName = "AC-06: CursosAcabados/total — devuelve un conteo numérico para el estudiante 1")]
    public async Task CursosAcabados_TotalEsNumerico()
    {
        var url = $"{CursosAcabadosUrl}/api/cursos-acabados/estudiante/{UserId}/total";
        _output.WriteLine($"[AC-06] GET {url}");

        var response = await _http.GetAsync(url);
        var body     = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"  Body: {body}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().MatchRegex(@"\d",
            "la respuesta de /total debe contener al menos un valor numérico");

        _output.WriteLine("  ✅ Aceptado");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // AC-07: ClasesMasTomadas — devuelve datos de todas las clases
    // ─────────────────────────────────────────────────────────────────────────

    [Fact(DisplayName = "AC-07: ClasesMasTomadas — devuelve el ranking completo de cursos")]
    public async Task ClasesMasTomadas_DevuelveRankingCompleto()
    {
        var url = $"{ClasesMasTomadas}/api/clases-mas-tomadas";
        _output.WriteLine($"[AC-07] GET {url}");

        var items = await GetArray(url);

        items.Should().NotBeNull();
        items.Length.Should().BeGreaterThan(0,
            "debe haber cursos registrados en la BD de prueba");

        _output.WriteLine($"  → {items.Length} curso(s) en el ranking. ✅ Aceptado");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // AC-08: Todos los módulos responden sin errores 5xx
    // ─────────────────────────────────────────────────────────────────────────

    [Theory(DisplayName = "AC-08: Todos los módulos responden sin errores de servidor (5xx)")]
    [InlineData("http://localhost:5010/api/notas/estudiante/1",             "Notas")]
    [InlineData("http://localhost:5011/api/cursos-acabados/estudiante/1",   "CursosAcabados")]
    [InlineData("http://localhost:5012/api/clases-mas-tomadas",             "ClasesMasTomadas")]
    [InlineData("http://localhost:5013/api/mejores-estudiantes",            "MejoresEstudiantes")]
    public async Task TodosLosModulos_SinErrores5xx(string url, string nombre)
    {
        _output.WriteLine($"[AC-08] {nombre}: GET {url}");

        var response = await _http.GetAsync(url);
        _output.WriteLine($"  Status: {(int)response.StatusCode}");

        ((int)response.StatusCode).Should().BeLessThan(500,
            $"el módulo {nombre} no debe devolver errores de servidor (5xx)");

        _output.WriteLine("  ✅ Aceptado");
    }

    public void Dispose() => _http.Dispose();
}
