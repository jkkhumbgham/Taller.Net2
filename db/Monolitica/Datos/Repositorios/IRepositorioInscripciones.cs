using Monolitica.Datos.Modelos;

namespace Monolitica.Datos.Repositorios;

public interface IRepositorioInscripciones
{
    Task<Usuario?> ObtenerUsuarioPorIdAsync(int userId);
    Task<IEnumerable<Inscripcion>> ObtenerInscripcionesPorUsuarioAsync(int userId);
    Task<Inscripcion?> ObtenerInscripcionPorUsuarioYCursoAsync(int userId, int courseId);
    Task<IEnumerable<Inscripcion>> ObtenerTodasLasInscripcionesAsync();
}
