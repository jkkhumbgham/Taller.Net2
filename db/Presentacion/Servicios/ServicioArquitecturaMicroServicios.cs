using System.Net.Http.Json;
using Presentacion.Modelos.DTOs;
using Presentacion.Servicios.Interfaces;

namespace Presentacion.Servicios;

public class ServicioArquitecturaMicroServicios : IServicioArquitecturaMicroServicios
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ServicioArquitecturaMicroServicios(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ResumenEstudianteDto?> ObtenerResumenEstudianteAsync(int userId)
    {
        var client = _httpClientFactory.CreateClient("micro-notas");
        return await client.GetFromJsonAsync<ResumenEstudianteDto>($"/api/notas/estudiante/{userId}/resumen");
    }

    public async Task<IEnumerable<NotaEstudianteDto>> ObtenerNotasEstudianteAsync(int userId)
    {
        var client = _httpClientFactory.CreateClient("micro-notas");
        return await client.GetFromJsonAsync<IEnumerable<NotaEstudianteDto>>($"/api/notas/estudiante/{userId}")
               ?? Enumerable.Empty<NotaEstudianteDto>();
    }

    public async Task<PromedioEstudianteDto?> ObtenerPromedioEstudianteAsync(int userId)
    {
        var client = _httpClientFactory.CreateClient("micro-notas");
        return await client.GetFromJsonAsync<PromedioEstudianteDto>($"/api/notas/estudiante/{userId}/promedio");
    }

    public async Task<IEnumerable<CursoAcabadoDto>> ObtenerCursosAcabadosAsync(int userId)
    {
        var client = _httpClientFactory.CreateClient("micro-cursos-acabados");
        return await client.GetFromJsonAsync<IEnumerable<CursoAcabadoDto>>($"/api/cursos-acabados/estudiante/{userId}")
               ?? Enumerable.Empty<CursoAcabadoDto>();
    }

    public async Task<IEnumerable<ClaseMasTomadaDto>> ObtenerClasesMasTomadosAsync()
    {
        var client = _httpClientFactory.CreateClient("micro-clases-mas-tomadas");
        return await client.GetFromJsonAsync<IEnumerable<ClaseMasTomadaDto>>("/api/clases/mas-tomadas")
               ?? Enumerable.Empty<ClaseMasTomadaDto>();
    }

    public async Task<IEnumerable<MejorEstudianteDto>> ObtenerMejoresEstudiantesAsync()
    {
        var client = _httpClientFactory.CreateClient("micro-mejores-estudiantes");
        return await client.GetFromJsonAsync<IEnumerable<MejorEstudianteDto>>("/api/mejores-estudiantes")
               ?? Enumerable.Empty<MejorEstudianteDto>();
    }
}
