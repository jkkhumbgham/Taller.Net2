using MejoresEstudiantes.Logica.DTOs;

namespace MejoresEstudiantes.Logica.Servicios;

public interface IServicioMejoresEstudiantes
{
    Task<IEnumerable<MejorEstudianteDto>> ObtenerMejoresEstudiantesAsync();
    Task<IEnumerable<MejorEstudianteDto>> ObtenerTopMejoresEstudiantesAsync(int n);
}
