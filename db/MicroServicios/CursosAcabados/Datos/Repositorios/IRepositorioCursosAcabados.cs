using CursosAcabados.Datos.Modelos;

namespace CursosAcabados.Datos.Repositorios;

public interface IRepositorioCursosAcabados
{
    Task<Usuario?> ObtenerUsuarioPorIdAsync(int userId);
    Task<IEnumerable<Inscripcion>> ObtenerInscripcionesPorUsuarioAsync(int userId);
    Task<Curso?> ObtenerCursoPorIdAsync(int courseId);
    Task<IEnumerable<Leccion>> ObtenerLeccionesPorCursoAsync(int courseId);
    Task<IEnumerable<ProgresoLeccion>> ObtenerProgresosPorUsuarioYLeccionesAsync(int userId, IEnumerable<int> lessonIds);
}
