using MejoresEstudiantes.Datos.Modelos;

namespace MejoresEstudiantes.Datos.Repositorios;

public interface IRepositorioMejoresEstudiantes
{
    Task<IEnumerable<IntentoCuestionario>> ObtenerTodosLosIntentosAsync();
    Task<IEnumerable<Usuario>> ObtenerTodosLosUsuariosAsync();
    Task<IEnumerable<Inscripcion>> ObtenerTodasLasInscripcionesAsync();
}
