using Notas.Logica.DTOs;

namespace Notas.Logica.Servicios;

public interface IServicioNotas
{
    Task<IEnumerable<NotaEstudianteDto>?> ObtenerNotasEstudianteAsync(int userId);
    Task<PromedioEstudianteDto?> ObtenerPromedioEstudianteAsync(int userId);
    Task<ResumenEstudianteDto?> ObtenerResumenEstudianteAsync(int userId);
}
