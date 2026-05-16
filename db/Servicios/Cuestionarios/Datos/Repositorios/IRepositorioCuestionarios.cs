using Cuestionarios.Datos.Modelos;

namespace Cuestionarios.Datos.Repositorios;

public interface IRepositorioCuestionarios
{
    Task<Usuario?> ObtenerUsuarioPorIdAsync(int userId);
    Task<IEnumerable<Usuario>> ObtenerTodosLosUsuariosAsync();
    Task<IEnumerable<IntentoCuestionario>> ObtenerIntentosPorUsuarioAsync(int userId);
    Task<IEnumerable<IntentoCuestionario>> ObtenerTodosLosIntentosAsync();
    Task<IEnumerable<Cuestionario>> ObtenerCuestionariosPorIdsAsync(IEnumerable<int> quizIds);
}
