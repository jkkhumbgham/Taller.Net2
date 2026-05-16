using Notas.Datos.Modelos;

namespace Notas.Datos.Repositorios;

public interface IRepositorioNotas
{
    Task<Usuario?> ObtenerUsuarioPorIdAsync(int userId);
    Task<IEnumerable<IntentoCuestionario>> ObtenerIntentosPorUsuarioAsync(int userId);
    Task<IEnumerable<Cuestionario>> ObtenerCuestionariosPorIdsAsync(IEnumerable<int> quizIds);
    Task<IEnumerable<Inscripcion>> ObtenerEnrollmentsPorUsuarioAsync(int userId);
}
