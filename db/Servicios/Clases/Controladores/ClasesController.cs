using Microsoft.AspNetCore.Mvc;
using Clases.Logica.Servicios;

namespace Clases.Controladores;

[ApiController]
[Route("api/clases")]
public class ClasesController : ControllerBase
{
    private readonly IServicioClases _servicio;

    public ClasesController(IServicioClases servicio)
    {
        _servicio = servicio;
    }

    [HttpGet("cursos-acabados/estudiante/{userId}")]
    public async Task<IActionResult> ObtenerCursosAcabadosEstudiante(int userId)
    {
        var resultado = await _servicio.ObtenerCursosAcabadosEstudianteAsync(userId);
        if (resultado == null) return NotFound(new { mensaje = $"Estudiante con id {userId} no encontrado." });
        return Ok(resultado);
    }

    [HttpGet("mas-tomadas")]
    public async Task<IActionResult> ObtenerMasTomadas()
    {
        var resultado = await _servicio.ObtenerMasTomadas();
        return Ok(resultado);
    }

    [HttpGet("mas-tomadas/top/{n}")]
    public async Task<IActionResult> ObtenerTopMasTomadas(int n)
    {
        var resultado = await _servicio.ObtenerTopMasTomadas(n);
        return Ok(resultado);
    }
}
