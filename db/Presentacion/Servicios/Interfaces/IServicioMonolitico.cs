using Presentacion.Modelos.DTOs;

namespace Presentacion.Servicios.Interfaces;

public interface IServicioMonolitico
{
    Task<ResumenEstudianteDto?> ObtenerResumenEstudianteAsync(int userId);
    Task<IEnumerable<EstadisticasCursoDto>> ObtenerEstadisticasCursosAsync(int userId);
    Task<IEnumerable<EstadisticasCuestionarioDto>> ObtenerEstadisticasCuestionariosAsync(int userId);
    Task<IEnumerable<NotaEstudianteDto>> ObtenerNotasEstudianteAsync(int userId);
    Task<IEnumerable<CursoAcabadoDto>> ObtenerCursosAcabadosAsync(int userId);
    Task<IEnumerable<ClaseMasTomadaDto>> ObtenerClasesMasTomadosAsync();
    Task<IEnumerable<MejorEstudianteDto>> ObtenerMejoresEstudiantesAsync();
}
