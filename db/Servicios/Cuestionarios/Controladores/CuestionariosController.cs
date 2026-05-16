using Microsoft.AspNetCore.Mvc;
using Cuestionarios.Logica.Servicios;

namespace Cuestionarios.Controladores;

[ApiController]
[Route("api/cuestionarios")]
public class CuestionariosController : ControllerBase
{
    private readonly IServicioCuestionarios _servicio;

    public CuestionariosController(IServicioCuestionarios servicio)
    {
        _servicio = servicio;
    }

    [HttpGet("notas/estudiante/{userId}")]
    public async Task<IActionResult> ObtenerNotasEstudiante(int userId)
    {
        var resultado = await _servicio.ObtenerNotasEstudianteAsync(userId);
        if (resultado == null) return NotFound(new { mensaje = $"Estudiante con id {userId} no encontrado." });
        return Ok(resultado);
    }

    [HttpGet("notas/promedio/estudiante/{userId}")]
    public async Task<IActionResult> ObtenerPromedioEstudiante(int userId)
    {
        var resultado = await _servicio.ObtenerPromedioEstudianteAsync(userId);
        if (resultado == null) return NotFound(new { mensaje = $"Estudiante con id {userId} no encontrado." });
        return Ok(resultado);
    }

    [HttpGet("mejores-estudiantes")]
    public async Task<IActionResult> ObtenerMejoresEstudiantes()
    {
        var resultado = await _servicio.ObtenerMejoresEstudiantesAsync();
        return Ok(resultado);
    }

    [HttpGet("mejores-estudiantes/top/{n}")]
    public async Task<IActionResult> ObtenerTopMejoresEstudiantes(int n)
    {
        var resultado = await _servicio.ObtenerTopMejoresEstudiantesAsync(n);
        return Ok(resultado);
    }
}
