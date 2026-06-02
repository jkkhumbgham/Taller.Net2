using System.Net;
using System.Text.Json;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Pruebas.Usabilidad;

/// <summary>
/// Punto 2 — Pruebas de Usabilidad (API REST)
///
/// Equivalente a las pruebas de usabilidad del taller JEE,
/// adaptadas al contexto de una API: se verifica que cada endpoint
/// devuelva datos claros, completos, con estructura reconocible
/// y mensajes de error comprensibles.
///
/// Criterios evaluados (tomados del documento JEE):
///   - Claridad      → los campos del JSON son autoexplicativos
///   - Navegación    → los endpoints de detalle son accesibles
///   - Retroalimentación → errores devuelven mensajes legibles (no stack traces)
///   - Consistencia  → todos los endpoints usan el mismo formato de respuesta
/// </summary>
public class UsabilidadTests : IDisposable
{
    private readonly HttpClient _http;
    private readonly ITestOutputHelper _output;

    private const string NotasUrl           = "http://localhost:5010";
    private const string CursosAcabadosUrl  = "http://localhost:5011";
    private const string ClasesMasTomadas   = "http://localhost:5012";
    private const string MejoresEstudiantes = "http://localhost:5013";
    private const int UserId                = 1;

    public UsabilidadTests(ITestOutputHelper output)
    {
        _output = output;
        _http   = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private async Task<JsonElement> ObtenerJson(string url)
    {
        var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"  Body: {body[..Math.Min(200, body.Length)]}...");
        return JsonSerializer.Deserialize<JsonElement>(body);
    }

    private async Task<JsonElement[]> ObtenerJsonArray(string url)
    {
        var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"  Body (primeros 200): {body[..Math.Min(200, body.Length)]}");
        return JsonSerializer.Deserialize<JsonElement[]>(body)!;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // US-01: CLARIDAD — Notas devuelven campos comprensibles
    // ─────────────────────────────────────────────────────────────────────────

    [Fact(DisplayName = "US-01: Notas — la respuesta tiene campos claros (nombreCurso, nota, etc.)")]
    public async Task Notas_RespuestaContieneCamposLegibles()
    {
        var url  = $"{NotasUrl}/api/notas/estudiante/{UserId}";
        _output.WriteLine($"[US-01] GET {url}");

        var items = await ObtenerJsonArray(url);

        items.Should().NotBeNull("la respuesta no debe ser nula");
        items.Length.Should().BeGreaterThan(0, "debe devolver al menos una nota");

        var primer = items[0];
        // Verifica que el JSON tenga propiedades con nombres reconocibles
        var propiedades = primer.EnumerateObject().Select(p => p.Name.ToLower()).ToList();
        _output.WriteLine($"  Propiedades encontradas: {string.Join(", ", propiedades)}");

        // Al menos debe haber alguna propiedad con "nota", "score", "calificacion" o "puntaje"
        propiedades.Should().Contain(p =>
            p.Contains("nota") || p.Contains("score") || p.Contains("calificacion") || p.Contains("puntaje"),
            "debe haber un campo que identifique la calificación");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // US-02: CLARIDAD — Promedio devuelve valor numérico identificable
    // ─────────────────────────────────────────────────────────────────────────

    [Fact(DisplayName = "US-02: Notas/Promedio — devuelve un objeto con campo numérico de promedio")]
    public async Task Notas_PromedioContieneCampoNumerico()
    {
        var url = $"{NotasUrl}/api/notas/estudiante/{UserId}/promedio";
        _output.WriteLine($"[US-02] GET {url}");

        var response = await _http.GetAsync(url);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"  Body: {body}");

        // El body puede ser un objeto con un campo de promedio
        body.Should().NotBeNullOrWhiteSpace("la respuesta no debe estar vacía");
        // Debe contener al menos un dígito (el valor numérico del promedio)
        body.Should().MatchRegex(@"\d",
            "la respuesta debe contener un valor numérico identificable");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // US-03: NAVEGACIÓN — CursosAcabados accesible por ID de estudiante
    // ─────────────────────────────────────────────────────────────────────────

    [Fact(DisplayName = "US-03: CursosAcabados — se puede consultar por ID de estudiante")]
    public async Task CursosAcabados_AccesiblePorEstudianteId()
    {
        var url = $"{CursosAcabadosUrl}/api/cursos-acabados/estudiante/{UserId}";
        _output.WriteLine($"[US-03] GET {url}");

        var response = await _http.GetAsync(url);
        _output.WriteLine($"  Status: {(int)response.StatusCode}");

        response.StatusCode.Should().Be(HttpStatusCode.OK,
            "el endpoint de navegación por estudiante debe responder correctamente");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // US-04: RETROALIMENTACIÓN — Estudiante inexistente devuelve 404 con mensaje
    // ─────────────────────────────────────────────────────────────────────────

    [Fact(DisplayName = "US-04: CursosAcabados — estudiante inexistente devuelve 404 con mensaje legible")]
    public async Task CursosAcabados_EstudianteInexistente_Devuelve404ConMensaje()
    {
        var url = $"{CursosAcabadosUrl}/api/cursos-acabados/estudiante/99999";
        _output.WriteLine($"[US-04] GET {url}");

        var response = await _http.GetAsync(url);
        var body     = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"  Status: {(int)response.StatusCode}");
        _output.WriteLine($"  Body  : {body}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound,
            "un estudiante inexistente debe devolver 404, no 500");

        body.Should().NotBeNullOrWhiteSpace(
            "el mensaje de error no debe estar vacío");

        body.ToLower().Should().Contain("mensaje",
            "el cuerpo del error debe contener un campo 'mensaje' comprensible para el usuario");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // US-05: RETROALIMENTACIÓN — Notas, estudiante inexistente devuelve 404
    // ─────────────────────────────────────────────────────────────────────────

    [Fact(DisplayName = "US-05: Notas — estudiante inexistente devuelve 404 con mensaje legible")]
    public async Task Notas_EstudianteInexistente_Devuelve404ConMensaje()
    {
        var url  = $"{NotasUrl}/api/notas/estudiante/99999";
        _output.WriteLine($"[US-05] GET {url}");

        var response = await _http.GetAsync(url);
        var body     = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"  Status: {(int)response.StatusCode}");
        _output.WriteLine($"  Body  : {body}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        body.Should().NotBeNullOrWhiteSpace();
        body.ToLower().Should().Contain("mensaje");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // US-06: CONSISTENCIA — Todos los microservicios devuelven JSON
    // ─────────────────────────────────────────────────────────────────────────

    [Theory(DisplayName = "US-06: Todos los endpoints devuelven Content-Type application/json")]
    [InlineData("http://localhost:5010/api/notas/estudiante/1",             "Notas")]
    [InlineData("http://localhost:5011/api/cursos-acabados/estudiante/1",   "CursosAcabados")]
    [InlineData("http://localhost:5012/api/clases-mas-tomadas",             "ClasesMasTomadas")]
    [InlineData("http://localhost:5013/api/mejores-estudiantes",            "MejoresEstudiantes")]
    public async Task TodosLosEndpoints_DebenDevolverContentTypeJson(string url, string nombre)
    {
        _output.WriteLine($"[US-06] {nombre}: GET {url}");

        var response    = await _http.GetAsync(url);
        var contentType = response.Content.Headers.ContentType?.MediaType;
        _output.WriteLine($"  Content-Type: {contentType}");

        contentType.Should().Contain("application/json",
            $"el microservicio {nombre} debe devolver JSON para mantener consistencia");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // US-07: CONSISTENCIA — ClasesMasTomadas y MejoresEstudiantes devuelven arrays
    // ─────────────────────────────────────────────────────────────────────────

    [Fact(DisplayName = "US-07: ClasesMasTomadas — devuelve una lista de clases con datos claros")]
    public async Task ClasesMasTomadas_DevuelveListaConCamposReconocibles()
    {
        var url = $"{ClasesMasTomadas}/api/clases-mas-tomadas";
        _output.WriteLine($"[US-07] GET {url}");

        var items = await ObtenerJsonArray(url);
        items.Should().NotBeEmpty("debe devolver al menos una clase");

        var primer     = items[0];
        var propiedades = primer.EnumerateObject().Select(p => p.Name.ToLower()).ToList();
        _output.WriteLine($"  Propiedades: {string.Join(", ", propiedades)}");

        propiedades.Should().Contain(p =>
            p.Contains("curso") || p.Contains("clase") || p.Contains("titulo") || p.Contains("nombre"),
            "debe haber un campo que identifique el nombre del curso");
    }

    [Fact(DisplayName = "US-08: MejoresEstudiantes — devuelve lista con identificación de estudiante")]
    public async Task MejoresEstudiantes_DevuelveListaConEstudiante()
    {
        var url = $"{MejoresEstudiantes}/api/mejores-estudiantes";
        _output.WriteLine($"[US-08] GET {url}");

        var items = await ObtenerJsonArray(url);
        items.Should().NotBeEmpty("debe devolver al menos un estudiante");

        var primer     = items[0];
        var propiedades = primer.EnumerateObject().Select(p => p.Name.ToLower()).ToList();
        _output.WriteLine($"  Propiedades: {string.Join(", ", propiedades)}");

        propiedades.Should().Contain(p =>
            p.Contains("nombre") || p.Contains("estudiante") || p.Contains("usuario") || p.Contains("name"),
            "debe haber un campo que identifique al estudiante");
    }

    public void Dispose() => _http.Dispose();
}
