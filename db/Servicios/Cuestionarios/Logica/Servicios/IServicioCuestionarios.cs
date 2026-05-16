using Cuestionarios.Logica.DTOs;

namespace Cuestionarios.Logica.Servicios;

public interface IServicioCuestionarios
{
    Task<IEnumerable<NotaEstudianteDto>?> ObtenerNotasEstudianteAsync(int userId);
    Task<MejorEstudianteDto?> ObtenerPromedioEstudianteAsync(int userId);
    Task<IEnumerable<MejorEstudianteDto>> ObtenerMejoresEstudiantesAsync();
    Task<IEnumerable<MejorEstudianteDto>> ObtenerTopMejoresEstudiantesAsync(int n);
}
