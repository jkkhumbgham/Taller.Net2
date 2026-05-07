using Estadisticas.Datos.Modelos;

namespace Estadisticas.Datos.Repositorios;

public interface IRepositorioEstadisticas
{
    Task<Usuario?> ObtenerUsuarioPorIdAsync(int userId);
    Task<IEnumerable<Inscripcion>> ObtenerInscripcionesPorUsuarioAsync(int userId);
    Task<IEnumerable<ProgresoLeccion>> ObtenerProgresosPorUsuarioAsync(int userId);
    Task<IEnumerable<ProgresoLeccion>> ObtenerProgresosPorUsuarioYLeccionesAsync(int userId, IEnumerable<int> lessonIds);
    Task<IEnumerable<IntentoCuestionario>> ObtenerIntentosPorUsuarioAsync(int userId);
    Task<Curso?> ObtenerCursoPorIdAsync(int courseId);
    Task<IEnumerable<Curso>> ObtenerCursosPorIdsAsync(IEnumerable<int> courseIds);
    Task<IEnumerable<Leccion>> ObtenerLeccionesPorCursoAsync(int courseId);
}
