using Presentacion.Modelos.VistaModelos;

namespace Presentacion.Servicios.Interfaces;

public interface IServicioCrud
{
    Task<IEnumerable<UsuarioVistaModelo>> ObtenerTodosUsuariosAsync();
    Task<UsuarioVistaModelo?> ObtenerUsuarioPorIdAsync(long id);
    Task<long> CrearUsuarioAsync(CrearUsuarioModelo modelo);
    Task<bool> ActualizarUsuarioAsync(UsuarioVistaModelo modelo);
    Task<bool> EliminarUsuarioAsync(long id);

    Task<IEnumerable<CursoVistaModelo>> ObtenerTodosCursosAsync();
    Task<CursoVistaModelo?> ObtenerCursoPorIdAsync(long id);
    Task<long> CrearCursoAsync(CrearCursoModelo modelo);
    Task<bool> ActualizarCursoAsync(CursoVistaModelo modelo);
    Task<bool> EliminarCursoAsync(long id);

    Task<IEnumerable<InscripcionVistaModelo>> ObtenerTodasInscripcionesAsync();
    Task<long> CrearInscripcionAsync(long usuarioId, long cursoId);
    Task<bool> EliminarInscripcionAsync(long id);

    Task<int> ContarUsuariosAsync();
    Task<int> ContarCursosAsync();
    Task<int> ContarInscripcionesAsync();
}
