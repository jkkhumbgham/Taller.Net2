using CursosAcabados.Logica.DTOs;

namespace CursosAcabados.Logica.Servicios;

public interface IServicioCursosAcabados
{
    Task<IEnumerable<CursoAcabadoDto>?> ObtenerCursosAcabadosEstudianteAsync(int userId);
    Task<TotalCursosAcabadosDto?> ObtenerTotalCursosAcabadosAsync(int userId);
}
