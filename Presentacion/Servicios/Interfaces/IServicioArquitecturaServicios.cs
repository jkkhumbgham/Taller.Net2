using Presentacion.Modelos.DTOs;

namespace Presentacion.Servicios.Interfaces;

public interface IServicioArquitecturaServicios
{
    Task<ResumenEstudianteDto?> ObtenerResumenEstudianteAsync(int userId);
    Task<IEnumerable<EstadisticasCursoDto>> ObtenerEstadisticasCursosAsync(int userId);
    Task<IEnumerable<CursoAcabadoDto>> ObtenerCursosAcabadosAsync(int userId);
    Task<IEnumerable<ClaseMasTomadaDto>> ObtenerClasesMasTomadosAsync();
}
