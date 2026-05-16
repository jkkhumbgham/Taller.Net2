using ClasesMasTomadas.Datos.Modelos;

namespace ClasesMasTomadas.Datos.Repositorios;

public interface IRepositorioClasesMasTomadas
{
    Task<IEnumerable<Inscripcion>> ObtenerTodasLasInscripcionesAsync();
    Task<IEnumerable<Curso>> ObtenerCursosPorIdsAsync(IEnumerable<int> courseIds);
}
