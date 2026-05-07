using Clases.Datos.Modelos;

namespace Clases.Datos.Repositorios;

public interface IRepositorioClases
{
    Task<Usuario?> ObtenerUsuarioPorIdAsync(int userId);
    Task<IEnumerable<Inscripcion>> ObtenerInscripcionesPorUsuarioAsync(int userId);
    Task<IEnumerable<Inscripcion>> ObtenerTodasLasInscripcionesAsync();
    Task<Curso?> ObtenerCursoPorIdAsync(int courseId);
    Task<IEnumerable<Curso>> ObtenerCursosPorIdsAsync(IEnumerable<int> courseIds);
    Task<IEnumerable<Leccion>> ObtenerLeccionesPorCursoAsync(int courseId);
    Task<IEnumerable<ProgresoLeccion>> ObtenerProgresosPorUsuarioYLeccionesAsync(int userId, IEnumerable<int> lessonIds);
}
