using Microsoft.AspNetCore.Mvc;
using Notas.Logica.Servicios;

namespace Notas.Controladores;

[ApiController]
[Route("api/notas")]
public class NotasController : ControllerBase
{
    private readonly IServicioNotas _servicio;

    public NotasController(IServicioNotas servicio)
    {
        _servicio = servicio;
    }

    [HttpGet("estudiante/{userId}")]
    public async Task<IActionResult> ObtenerNotasEstudiante(int userId)
    {
        var resultado = await _servicio.ObtenerNotasEstudianteAsync(userId);
        if (resultado == null) return NotFound(new { mensaje = $"Estudiante con id {userId} no encontrado." });
        return Ok(resultado);
    }

    [HttpGet("estudiante/{userId}/promedio")]
    public async Task<IActionResult> ObtenerPromedioEstudiante(int userId)
    {
        var resultado = await _servicio.ObtenerPromedioEstudianteAsync(userId);
        if (resultado == null) return NotFound(new { mensaje = $"Estudiante con id {userId} no encontrado." });
        return Ok(resultado);
    }
}
