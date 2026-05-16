using Monolitica.Datos.Modelos;

namespace Monolitica.Datos.Repositorios;

public interface IRepositorioProgresoLecciones
{
    Task<IEnumerable<ProgresoLeccion>> ObtenerProgresosPorUsuarioAsync(int userId);
    Task<IEnumerable<ProgresoLeccion>> ObtenerProgresosPorUsuarioYLeccionesAsync(int userId, IEnumerable<int> lessonIds);
    Task<IEnumerable<Leccion>> ObtenerLeccionesPorModulosAsync(IEnumerable<int> moduleIds);
    Task<IEnumerable<Leccion>> ObtenerLeccionesPorCursoAsync(int courseId);
    Task<Curso?> ObtenerCursoPorIdAsync(int courseId);
    Task<IEnumerable<Curso>> ObtenerCursosPorIdsAsync(IEnumerable<int> courseIds);
    Task<IEnumerable<Modulo>> ObtenerModulosPorCursoAsync(int courseId);
}
