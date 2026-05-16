using Microsoft.AspNetCore.Mvc;
using CursosAcabados.Logica.Servicios;

namespace CursosAcabados.Controladores;

[ApiController]
[Route("api/cursos-acabados")]
public class CursosAcabadosController : ControllerBase
{
    private readonly IServicioCursosAcabados _servicio;

    public CursosAcabadosController(IServicioCursosAcabados servicio)
    {
        _servicio = servicio;
    }

    [HttpGet("estudiante/{userId}")]
    public async Task<IActionResult> ObtenerCursosAcabadosEstudiante(int userId)
    {
        var resultado = await _servicio.ObtenerCursosAcabadosEstudianteAsync(userId);
        if (resultado == null) return NotFound(new { mensaje = $"Estudiante con id {userId} no encontrado." });
        return Ok(resultado);
    }

    [HttpGet("estudiante/{userId}/total")]
    public async Task<IActionResult> ObtenerTotalCursosAcabados(int userId)
    {
        var resultado = await _servicio.ObtenerTotalCursosAcabadosAsync(userId);
        if (resultado == null) return NotFound(new { mensaje = $"Estudiante con id {userId} no encontrado." });
        return Ok(resultado);
    }
}
