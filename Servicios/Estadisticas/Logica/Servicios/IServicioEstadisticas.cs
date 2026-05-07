using Estadisticas.Logica.DTOs;

namespace Estadisticas.Logica.Servicios;

public interface IServicioEstadisticas
{
    Task<ResumenEstudianteDto?> ObtenerResumenEstudianteAsync(int userId);
    Task<IEnumerable<EstadisticasCursoDto>?> ObtenerEstadisticasCursosAsync(int userId);
    Task<IEnumerable<EstadisticasLeccionDto>?> ObtenerEstadisticasLeccionesAsync(int userId);
}
