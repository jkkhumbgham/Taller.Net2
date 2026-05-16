using Microsoft.AspNetCore.Mvc;
using Estadisticas.Logica.Servicios;

namespace Estadisticas.Controladores;

[ApiController]
[Route("api/estadisticas")]
public class EstadisticasController : ControllerBase
{
    private readonly IServicioEstadisticas _servicio;

    public EstadisticasController(IServicioEstadisticas servicio)
    {
        _servicio = servicio;
    }

    [HttpGet("estudiante/{userId}")]
    public async Task<IActionResult> ObtenerResumenEstudiante(int userId)
    {
        var resultado = await _servicio.ObtenerResumenEstudianteAsync(userId);
        if (resultado == null) return NotFound(new { mensaje = $"Estudiante con id {userId} no encontrado." });
        return Ok(resultado);
    }

    [HttpGet("estudiante/{userId}/cursos")]
    public async Task<IActionResult> ObtenerEstadisticasCursos(int userId)
    {
        var resultado = await _servicio.ObtenerEstadisticasCursosAsync(userId);
        if (resultado == null) return NotFound(new { mensaje = $"Estudiante con id {userId} no encontrado." });
        return Ok(resultado);
    }

    [HttpGet("estudiante/{userId}/lecciones")]
    public async Task<IActionResult> ObtenerEstadisticasLecciones(int userId)
    {
        var resultado = await _servicio.ObtenerEstadisticasLeccionesAsync(userId);
        if (resultado == null) return NotFound(new { mensaje = $"Estudiante con id {userId} no encontrado." });
        return Ok(resultado);
    }
}
