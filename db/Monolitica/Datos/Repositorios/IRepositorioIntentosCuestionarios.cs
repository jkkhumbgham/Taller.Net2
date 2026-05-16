using Monolitica.Datos.Modelos;

namespace Monolitica.Datos.Repositorios;

public interface IRepositorioIntentosCuestionarios
{
    Task<IEnumerable<IntentoCuestionario>> ObtenerIntentosPorUsuarioAsync(int userId);
    Task<IEnumerable<IntentoCuestionario>> ObtenerIntentosPorUsuarioYCuestionariosAsync(int userId, IEnumerable<int> quizIds);
    Task<IEnumerable<Cuestionario>> ObtenerCuestionariosPorIdsAsync(IEnumerable<int> quizIds);
    Task<IEnumerable<IntentoCuestionario>> ObtenerTodosLosIntentosAsync();
    Task<IEnumerable<Usuario>> ObtenerTodosLosUsuariosAsync();
}
