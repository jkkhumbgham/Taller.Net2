using Presentacion.Modelos.DTOs;

namespace Presentacion.Servicios.Interfaces;

public interface IServicioArquitecturaMicroServicios
{
    Task<ResumenEstudianteDto?> ObtenerResumenEstudianteAsync(int userId);
    Task<IEnumerable<NotaEstudianteDto>> ObtenerNotasEstudianteAsync(int userId);
    Task<PromedioEstudianteDto?> ObtenerPromedioEstudianteAsync(int userId);
    Task<IEnumerable<CursoAcabadoDto>> ObtenerCursosAcabadosAsync(int userId);
    Task<IEnumerable<ClaseMasTomadaDto>> ObtenerClasesMasTomadosAsync();
    Task<IEnumerable<MejorEstudianteDto>> ObtenerMejoresEstudiantesAsync();
}
