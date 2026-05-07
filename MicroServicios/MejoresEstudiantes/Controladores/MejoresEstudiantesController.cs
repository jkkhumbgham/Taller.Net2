using Microsoft.AspNetCore.Mvc;
using MejoresEstudiantes.Logica.Servicios;

namespace MejoresEstudiantes.Controladores;

[ApiController]
[Route("api/mejores-estudiantes")]
public class MejoresEstudiantesController : ControllerBase
{
    private readonly IServicioMejoresEstudiantes _servicio;

    public MejoresEstudiantesController(IServicioMejoresEstudiantes servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerMejoresEstudiantes()
    {
        var resultado = await _servicio.ObtenerMejoresEstudiantesAsync();
        return Ok(resultado);
    }

    [HttpGet("top/{n}")]
    public async Task<IActionResult> ObtenerTopMejoresEstudiantes(int n)
    {
        var resultado = await _servicio.ObtenerTopMejoresEstudiantesAsync(n);
        return Ok(resultado);
    }
}
