using Monolitica.Logica.DTOs;

namespace Monolitica.Logica.Servicios;

public interface IServicioEstadisticas
{
    Task<ResumenEstudianteDto?> ObtenerResumenEstudianteAsync(int userId);
    Task<IEnumerable<EstadisticasCursoDto>?> ObtenerEstadisticasCursosAsync(int userId);
    Task<EstadisticasDetalleCursoDto?> ObtenerEstadisticasDetalleCursoAsync(int userId, int courseId);
    Task<IEnumerable<EstadisticasCuestionarioDto>?> ObtenerEstadisticasCuestionariosAsync(int userId);
    Task<IEnumerable<EstadisticasLeccionDto>?> ObtenerEstadisticasLeccionesAsync(int userId);
    Task<IEnumerable<NotaEstudianteDto>?> ObtenerNotasEstudianteAsync(int userId);
    Task<IEnumerable<CursoAcabadoDto>?> ObtenerCursosAcabadosEstudianteAsync(int userId);
    Task<IEnumerable<ClaseMasTomadaDto>> ObtenerClasesMasTomadas();
    Task<IEnumerable<MejorEstudianteDto>> ObtenerMejoresEstudiantes();
}
