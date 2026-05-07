using Clases.Logica.DTOs;

namespace Clases.Logica.Servicios;

public interface IServicioClases
{
    Task<IEnumerable<CursoAcabadoDto>?> ObtenerCursosAcabadosEstudianteAsync(int userId);
    Task<IEnumerable<ClaseMasTomadaDto>> ObtenerMasTomadas();
    Task<IEnumerable<ClaseMasTomadaDto>> ObtenerTopMasTomadas(int n);
}
