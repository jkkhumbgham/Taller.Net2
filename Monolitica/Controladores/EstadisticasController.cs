using Microsoft.AspNetCore.Mvc;
using Monolitica.Logica.Servicios;

namespace Monolitica.Controladores;

[ApiController]
[Route("api/stats")]
public class EstadisticasController : ControllerBase
{
    private readonly IServicioEstadisticas _servicioEstadisticas;

    public EstadisticasController(IServicioEstadisticas servicioEstadisticas)
    {
        _servicioEstadisticas = servicioEstadisticas;
    }

    [HttpGet("students/{userId}")]
    public async Task<IActionResult> ObtenerResumenEstudiante(int userId)
    {
        var resultado = await _servicioEstadisticas.ObtenerResumenEstudianteAsync(userId);
        if (resultado == null) return NotFound(new { mensaje = $"Estudiante con id {userId} no encontrado." });
        return Ok(resultado);
    }

    [HttpGet("students/{userId}/courses")]
    public async Task<IActionResult> ObtenerEstadisticasCursos(int userId)
    {
        var resultado = await _servicioEstadisticas.ObtenerEstadisticasCursosAsync(userId);
        if (resultado == null) return NotFound(new { mensaje = $"Estudiante con id {userId} no encontrado." });
        return Ok(resultado);
    }

    [HttpGet("students/{userId}/courses/{courseId}")]
    public async Task<IActionResult> ObtenerEstadisticasDetalleCurso(int userId, int courseId)
    {
        var resultado = await _servicioEstadisticas.ObtenerEstadisticasDetalleCursoAsync(userId, courseId);
        if (resultado == null) return NotFound(new { mensaje = $"Estudiante {userId} o curso {courseId} no encontrado." });
        return Ok(resultado);
    }

    [HttpGet("students/{userId}/quizzes")]
    public async Task<IActionResult> ObtenerEstadisticasCuestionarios(int userId)
    {
        var resultado = await _servicioEstadisticas.ObtenerEstadisticasCuestionariosAsync(userId);
        if (resultado == null) return NotFound(new { mensaje = $"Estudiante con id {userId} no encontrado." });
        return Ok(resultado);
    }

    [HttpGet("students/{userId}/lessons")]
    public async Task<IActionResult> ObtenerEstadisticasLecciones(int userId)
    {
        var resultado = await _servicioEstadisticas.ObtenerEstadisticasLeccionesAsync(userId);
        if (resultado == null) return NotFound(new { mensaje = $"Estudiante con id {userId} no encontrado." });
        return Ok(resultado);
    }

    [HttpGet("notas/estudiante/{userId}")]
    public async Task<IActionResult> ObtenerNotasEstudiante(int userId)
    {
        var resultado = await _servicioEstadisticas.ObtenerNotasEstudianteAsync(userId);
        if (resultado == null) return NotFound(new { mensaje = $"Estudiante con id {userId} no encontrado." });
        return Ok(resultado);
    }

    [HttpGet("cursos-acabados/estudiante/{userId}")]
    public async Task<IActionResult> ObtenerCursosAcabadosEstudiante(int userId)
    {
        var resultado = await _servicioEstadisticas.ObtenerCursosAcabadosEstudianteAsync(userId);
        if (resultado == null) return NotFound(new { mensaje = $"Estudiante con id {userId} no encontrado." });
        return Ok(resultado);
    }

    [HttpGet("clases-mas-tomadas")]
    public async Task<IActionResult> ObtenerClasesMasTomadas()
    {
        var resultado = await _servicioEstadisticas.ObtenerClasesMasTomadas();
        return Ok(resultado);
    }

    [HttpGet("mejores-estudiantes")]
    public async Task<IActionResult> ObtenerMejoresEstudiantes()
    {
        var resultado = await _servicioEstadisticas.ObtenerMejoresEstudiantes();
        return Ok(resultado);
    }
}
