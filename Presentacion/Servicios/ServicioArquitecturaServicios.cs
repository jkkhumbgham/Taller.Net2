using System.Net.Http.Json;
using Presentacion.Modelos.DTOs;
using Presentacion.Servicios.Interfaces;

namespace Presentacion.Servicios;

public class ServicioArquitecturaServicios : IServicioArquitecturaServicios
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ServicioArquitecturaServicios(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ResumenEstudianteDto?> ObtenerResumenEstudianteAsync(int userId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("svc-estadisticas");
            return await client.GetFromJsonAsync<ResumenEstudianteDto>($"/api/estadisticas/estudiante/{userId}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<IEnumerable<EstadisticasCursoDto>> ObtenerEstadisticasCursosAsync(int userId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("svc-estadisticas");
            return await client.GetFromJsonAsync<IEnumerable<EstadisticasCursoDto>>($"/api/estadisticas/cursos/{userId}")
                   ?? Enumerable.Empty<EstadisticasCursoDto>();
        }
        catch
        {
            return Enumerable.Empty<EstadisticasCursoDto>();
        }
    }

    public async Task<IEnumerable<CursoAcabadoDto>> ObtenerCursosAcabadosAsync(int userId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("svc-clases");
            return await client.GetFromJsonAsync<IEnumerable<CursoAcabadoDto>>($"/api/clases/cursos-acabados/estudiante/{userId}")
                   ?? Enumerable.Empty<CursoAcabadoDto>();
        }
        catch
        {
            return Enumerable.Empty<CursoAcabadoDto>();
        }
    }

    public async Task<IEnumerable<ClaseMasTomadaDto>> ObtenerClasesMasTomadosAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("svc-clases");
            return await client.GetFromJsonAsync<IEnumerable<ClaseMasTomadaDto>>("/api/clases/mas-tomadas")
                   ?? Enumerable.Empty<ClaseMasTomadaDto>();
        }
        catch
        {
            return Enumerable.Empty<ClaseMasTomadaDto>();
        }
    }
}
