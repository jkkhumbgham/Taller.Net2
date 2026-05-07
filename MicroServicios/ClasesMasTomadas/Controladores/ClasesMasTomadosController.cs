using Microsoft.AspNetCore.Mvc;
using ClasesMasTomadas.Logica.Servicios;

namespace ClasesMasTomadas.Controladores;

[ApiController]
[Route("api/clases-mas-tomadas")]
public class ClasesMasTomadosController : ControllerBase
{
    private readonly IServicioClasesMasTomadas _servicio;

    public ClasesMasTomadosController(IServicioClasesMasTomadas servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerClasesMasTomadas()
    {
        var resultado = await _servicio.ObtenerClasesMasTomadas();
        return Ok(resultado);
    }

    [HttpGet("top/{n}")]
    public async Task<IActionResult> ObtenerTopClasesMasTomadas(int n)
    {
        var resultado = await _servicio.ObtenerTopClasesMasTomadas(n);
        return Ok(resultado);
    }
}
