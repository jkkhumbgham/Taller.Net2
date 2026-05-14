using Google.Protobuf.WellKnownTypes;
using Monolitica.Grpc;
using Presentacion.Modelos.VistaModelos;
using Presentacion.Servicios.Interfaces;

namespace Presentacion.Servicios;

public class ServicioCrud : IServicioCrud
{
    private readonly CrudGrpc.CrudGrpcClient _cliente;

    public ServicioCrud(CrudGrpc.CrudGrpcClient cliente)
    {
        _cliente = cliente;
    }

    // ===== Usuarios =====

    public async Task<IEnumerable<UsuarioVistaModelo>> ObtenerTodosUsuariosAsync()
    {
        try
        {
            var respuesta = await _cliente.ListarUsuariosAsync(new Empty());
            return respuesta.Usuarios.Select(MapearUsuario).ToList();
        }
        catch { return Enumerable.Empty<UsuarioVistaModelo>(); }
    }

    public async Task<UsuarioVistaModelo?> ObtenerUsuarioPorIdAsync(long id)
    {
        try
        {
            var respuesta = await _cliente.ObtenerUsuarioAsync(new SolicitudId { Id = id });
            return respuesta.Encontrado ? MapearUsuario(respuesta.Usuario) : null;
        }
        catch { return null; }
    }

    public async Task<long> CrearUsuarioAsync(CrearUsuarioModelo modelo)
    {
        var respuesta = await _cliente.CrearUsuarioAsync(new SolicitudCrearUsuario
        {
            Nombre = modelo.Nombre,
            Email = modelo.Email,
            Password = modelo.Password
        });
        return respuesta.Encontrado ? respuesta.Usuario.Id : 0;
    }

    public async Task<bool> ActualizarUsuarioAsync(UsuarioVistaModelo modelo)
    {
        try
        {
            var respuesta = await _cliente.ActualizarUsuarioAsync(new SolicitudActualizarUsuario
            {
                Id = modelo.Id,
                Nombre = modelo.Nombre,
                Email = modelo.Email,
                Password = modelo.Password
            });
            return respuesta.Exito;
        }
        catch { return false; }
    }

    public async Task<bool> EliminarUsuarioAsync(long id)
    {
        try
        {
            var respuesta = await _cliente.EliminarUsuarioAsync(new SolicitudId { Id = id });
            return respuesta.Exito;
        }
        catch { return false; }
    }

    public async Task<int> ContarUsuariosAsync()
    {
        try
        {
            var respuesta = await _cliente.ContarUsuariosAsync(new Empty());
            return respuesta.Total;
        }
        catch { return 0; }
    }

    // ===== Cursos =====

    public async Task<IEnumerable<CursoVistaModelo>> ObtenerTodosCursosAsync()
    {
        try
        {
            var respuesta = await _cliente.ListarCursosAsync(new Empty());
            return respuesta.Cursos.Select(MapearCurso).ToList();
        }
        catch { return Enumerable.Empty<CursoVistaModelo>(); }
    }

    public async Task<CursoVistaModelo?> ObtenerCursoPorIdAsync(long id)
    {
        try
        {
            var respuesta = await _cliente.ObtenerCursoAsync(new SolicitudId { Id = id });
            return respuesta.Encontrado ? MapearCurso(respuesta.Curso) : null;
        }
        catch { return null; }
    }

    public async Task<long> CrearCursoAsync(CrearCursoModelo modelo)
    {
        var respuesta = await _cliente.CrearCursoAsync(new SolicitudCrearCurso
        {
            Titulo = modelo.Titulo,
            Descripcion = modelo.Descripcion,
            Nivel = modelo.Nivel,
            Idioma = modelo.Idioma,
            EsPublicado = modelo.EsPublicado
        });
        return respuesta.Encontrado ? respuesta.Curso.Id : 0;
    }

    public async Task<bool> ActualizarCursoAsync(CursoVistaModelo modelo)
    {
        try
        {
            var respuesta = await _cliente.ActualizarCursoAsync(new SolicitudActualizarCurso
            {
                Id = modelo.Id,
                Titulo = modelo.Titulo,
                Descripcion = modelo.Descripcion,
                Nivel = modelo.Nivel,
                Idioma = modelo.Idioma,
                EsPublicado = modelo.EsPublicado
            });
            return respuesta.Exito;
        }
        catch { return false; }
    }

    public async Task<bool> EliminarCursoAsync(long id)
    {
        try
        {
            var respuesta = await _cliente.EliminarCursoAsync(new SolicitudId { Id = id });
            return respuesta.Exito;
        }
        catch { return false; }
    }

    public async Task<int> ContarCursosAsync()
    {
        try
        {
            var respuesta = await _cliente.ContarCursosAsync(new Empty());
            return respuesta.Total;
        }
        catch { return 0; }
    }

    // ===== Inscripciones =====

    public async Task<IEnumerable<InscripcionVistaModelo>> ObtenerTodasInscripcionesAsync()
    {
        try
        {
            var respuesta = await _cliente.ListarInscripcionesAsync(new Empty());
            return respuesta.Inscripciones.Select(MapearInscripcion).ToList();
        }
        catch { return Enumerable.Empty<InscripcionVistaModelo>(); }
    }

    public async Task<long> CrearInscripcionAsync(long usuarioId, long cursoId)
    {
        var respuesta = await _cliente.CrearInscripcionAsync(new SolicitudCrearInscripcion
        {
            UsuarioId = usuarioId,
            CursoId = cursoId
        });
        return respuesta.Inscripcion?.Id ?? 0;
    }

    public async Task<bool> EliminarInscripcionAsync(long id)
    {
        try
        {
            var respuesta = await _cliente.EliminarInscripcionAsync(new SolicitudId { Id = id });
            return respuesta.Exito;
        }
        catch { return false; }
    }

    public async Task<int> ContarInscripcionesAsync()
    {
        try
        {
            var respuesta = await _cliente.ContarInscripcionesAsync(new Empty());
            return respuesta.Total;
        }
        catch { return 0; }
    }

    // ===== Mappers =====

    private static UsuarioVistaModelo MapearUsuario(MensajeUsuario u) => new()
    {
        Id = u.Id,
        Nombre = u.Nombre,
        Email = u.Email,
        Password = u.Password,
        CreadoEn = DateTime.Parse(u.CreadoEn)
    };

    private static CursoVistaModelo MapearCurso(MensajeCurso c) => new()
    {
        Id = c.Id,
        Titulo = c.Titulo,
        Descripcion = c.Descripcion,
        Nivel = c.Nivel,
        Idioma = c.Idioma,
        EsPublicado = c.EsPublicado,
        CreadoEn = DateTime.Parse(c.CreadoEn),
        ActualizadoEn = DateTime.Parse(c.ActualizadoEn)
    };

    private static InscripcionVistaModelo MapearInscripcion(MensajeInscripcion i) => new()
    {
        Id = i.Id,
        UsuarioId = i.UsuarioId,
        CursoId = i.CursoId,
        InscritoEn = DateTime.Parse(i.InscritoEn),
        Progreso = i.Progreso
    };
}
