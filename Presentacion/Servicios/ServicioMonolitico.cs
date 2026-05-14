using System.Net.Http.Json;
using Presentacion.Modelos.DTOs;
using Presentacion.Servicios.Interfaces;

namespace Presentacion.Servicios;

public class ServicioMonolitico : IServicioMonolitico
{
    private readonly HttpClient _httpClient;

    public ServicioMonolitico(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ResumenEstudianteDto?> ObtenerResumenEstudianteAsync(int userId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ResumenEstudianteDto>($"/api/stats/students/{userId}");
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
            return await _httpClient.GetFromJsonAsync<IEnumerable<EstadisticasCursoDto>>($"/api/stats/students/{userId}/courses")
                   ?? Enumerable.Empty<EstadisticasCursoDto>();
        }
        catch
        {
            return Enumerable.Empty<EstadisticasCursoDto>();
        }
    }

    public async Task<IEnumerable<EstadisticasCuestionarioDto>> ObtenerEstadisticasCuestionariosAsync(int userId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<EstadisticasCuestionarioDto>>($"/api/stats/students/{userId}/quizzes")
                   ?? Enumerable.Empty<EstadisticasCuestionarioDto>();
        }
        catch
        {
            return Enumerable.Empty<EstadisticasCuestionarioDto>();
        }
    }

    public async Task<IEnumerable<NotaEstudianteDto>> ObtenerNotasEstudianteAsync(int userId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<NotaEstudianteDto>>($"/api/stats/notas/estudiante/{userId}")
                   ?? Enumerable.Empty<NotaEstudianteDto>();
        }
        catch
        {
            return Enumerable.Empty<NotaEstudianteDto>();
        }
    }

    public async Task<IEnumerable<CursoAcabadoDto>> ObtenerCursosAcabadosAsync(int userId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<CursoAcabadoDto>>($"/api/stats/cursos-acabados/estudiante/{userId}")
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
            return await _httpClient.GetFromJsonAsync<IEnumerable<ClaseMasTomadaDto>>("/api/stats/clases-mas-tomadas")
                   ?? Enumerable.Empty<ClaseMasTomadaDto>();
        }
        catch
        {
            return Enumerable.Empty<ClaseMasTomadaDto>();
        }
    }

    public async Task<IEnumerable<MejorEstudianteDto>> ObtenerMejoresEstudiantesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<MejorEstudianteDto>>("/api/stats/mejores-estudiantes")
                   ?? Enumerable.Empty<MejorEstudianteDto>();
        }
        catch
        {
            return Enumerable.Empty<MejorEstudianteDto>();
        }
    }
}
