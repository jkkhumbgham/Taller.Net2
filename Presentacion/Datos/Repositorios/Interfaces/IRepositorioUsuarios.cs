using Presentacion.Modelos.VistaModelos;

namespace Presentacion.Datos.Repositorios.Interfaces;

public interface IRepositorioUsuarios
{
    Task<IEnumerable<UsuarioVistaModelo>> ObtenerTodosAsync();
    Task<UsuarioVistaModelo?> ObtenerPorIdAsync(long id);
    Task<long> CrearAsync(CrearUsuarioModelo modelo);
    Task<bool> ActualizarAsync(UsuarioVistaModelo modelo);
    Task<bool> EliminarAsync(long id);
}
