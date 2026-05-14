using Presentacion.Modelos.VistaModelos;

namespace Presentacion.Datos.Repositorios.Interfaces;

public interface IRepositorioCursos
{
    Task<IEnumerable<CursoVistaModelo>> ObtenerTodosAsync();
    Task<CursoVistaModelo?> ObtenerPorIdAsync(long id);
    Task<long> CrearAsync(CrearCursoModelo modelo);
    Task<bool> ActualizarAsync(CursoVistaModelo modelo);
    Task<bool> EliminarAsync(long id);
}
