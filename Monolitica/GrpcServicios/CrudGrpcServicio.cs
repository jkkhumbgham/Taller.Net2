using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Monolitica.Datos.Contexto;
using Monolitica.Datos.Modelos;
using Monolitica.Grpc;

namespace Monolitica.GrpcServicios;

public class CrudGrpcServicio : CrudGrpc.CrudGrpcBase
{
    private readonly UserDbContext _userDb;
    private readonly ContentDbContext _contentDb;

    public CrudGrpcServicio(UserDbContext userDb, ContentDbContext contentDb)
    {
        _userDb = userDb;
        _contentDb = contentDb;
    }

    // ===== Usuarios =====

    public override async Task<RespuestaUsuarios> ListarUsuarios(Empty request, ServerCallContext context)
    {
        var lista = await _userDb.Users.OrderBy(u => u.Id).ToListAsync();
        var respuesta = new RespuestaUsuarios();
        respuesta.Usuarios.AddRange(lista.Select(MapearUsuario));
        return respuesta;
    }

    public override async Task<RespuestaUsuario> ObtenerUsuario(SolicitudId request, ServerCallContext context)
    {
        var u = await _userDb.Users.FindAsync((int)request.Id);
        if (u == null) return new RespuestaUsuario { Encontrado = false };
        return new RespuestaUsuario { Encontrado = true, Usuario = MapearUsuario(u) };
    }

    public override async Task<RespuestaUsuario> CrearUsuario(SolicitudCrearUsuario request, ServerCallContext context)
    {
        var u = new Usuario
        {
            Name = request.Nombre,
            Email = request.Email,
            Password = request.Password,
            CreatedAt = DateTime.UtcNow
        };
        _userDb.Users.Add(u);
        await _userDb.SaveChangesAsync();
        return new RespuestaUsuario { Encontrado = true, Usuario = MapearUsuario(u) };
    }

    public override async Task<RespuestaExito> ActualizarUsuario(SolicitudActualizarUsuario request, ServerCallContext context)
    {
        var u = await _userDb.Users.FindAsync((int)request.Id);
        if (u == null) return new RespuestaExito { Exito = false };
        u.Name = request.Nombre;
        u.Email = request.Email;
        if (!string.IsNullOrWhiteSpace(request.Password))
            u.Password = request.Password;
        await _userDb.SaveChangesAsync();
        return new RespuestaExito { Exito = true };
    }

    public override async Task<RespuestaExito> EliminarUsuario(SolicitudId request, ServerCallContext context)
    {
        var u = await _userDb.Users.FindAsync((int)request.Id);
        if (u == null) return new RespuestaExito { Exito = false };
        _userDb.Users.Remove(u);
        await _userDb.SaveChangesAsync();
        return new RespuestaExito { Exito = true };
    }

    public override async Task<RespuestaConteo> ContarUsuarios(Empty request, ServerCallContext context)
    {
        return new RespuestaConteo { Total = await _userDb.Users.CountAsync() };
    }

    // ===== Cursos =====

    public override async Task<RespuestaCursos> ListarCursos(Empty request, ServerCallContext context)
    {
        var lista = await _contentDb.Courses.OrderBy(c => c.Id).ToListAsync();
        var respuesta = new RespuestaCursos();
        respuesta.Cursos.AddRange(lista.Select(MapearCurso));
        return respuesta;
    }

    public override async Task<RespuestaCurso> ObtenerCurso(SolicitudId request, ServerCallContext context)
    {
        var c = await _contentDb.Courses.FindAsync((int)request.Id);
        if (c == null) return new RespuestaCurso { Encontrado = false };
        return new RespuestaCurso { Encontrado = true, Curso = MapearCurso(c) };
    }

    public override async Task<RespuestaCurso> CrearCurso(SolicitudCrearCurso request, ServerCallContext context)
    {
        var c = new Curso
        {
            Title = request.Titulo,
            Description = request.Descripcion,
            Level = request.Nivel,
            Language = request.Idioma,
            IsPublished = request.EsPublicado,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _contentDb.Courses.Add(c);
        await _contentDb.SaveChangesAsync();
        return new RespuestaCurso { Encontrado = true, Curso = MapearCurso(c) };
    }

    public override async Task<RespuestaExito> ActualizarCurso(SolicitudActualizarCurso request, ServerCallContext context)
    {
        var c = await _contentDb.Courses.FindAsync((int)request.Id);
        if (c == null) return new RespuestaExito { Exito = false };
        c.Title = request.Titulo;
        c.Description = request.Descripcion;
        c.Level = request.Nivel;
        c.Language = request.Idioma;
        c.IsPublished = request.EsPublicado;
        c.UpdatedAt = DateTime.UtcNow;
        await _contentDb.SaveChangesAsync();
        return new RespuestaExito { Exito = true };
    }

    public override async Task<RespuestaExito> EliminarCurso(SolicitudId request, ServerCallContext context)
    {
        var c = await _contentDb.Courses.FindAsync((int)request.Id);
        if (c == null) return new RespuestaExito { Exito = false };
        _contentDb.Courses.Remove(c);
        await _contentDb.SaveChangesAsync();
        return new RespuestaExito { Exito = true };
    }

    public override async Task<RespuestaConteo> ContarCursos(Empty request, ServerCallContext context)
    {
        return new RespuestaConteo { Total = await _contentDb.Courses.CountAsync() };
    }

    // ===== Inscripciones =====

    public override async Task<RespuestaInscripciones> ListarInscripciones(Empty request, ServerCallContext context)
    {
        var lista = await _userDb.Enrollments.OrderBy(i => i.Id).ToListAsync();
        var respuesta = new RespuestaInscripciones();
        respuesta.Inscripciones.AddRange(lista.Select(MapearInscripcion));
        return respuesta;
    }

    public override async Task<RespuestaInscripcion> CrearInscripcion(SolicitudCrearInscripcion request, ServerCallContext context)
    {
        var inscripcion = new Inscripcion
        {
            UserId = (int)request.UsuarioId,
            CourseId = (int)request.CursoId,
            EnrolledAt = DateTime.UtcNow,
            Progress = 0
        };
        _userDb.Enrollments.Add(inscripcion);
        await _userDb.SaveChangesAsync();
        return new RespuestaInscripcion { Inscripcion = MapearInscripcion(inscripcion) };
    }

    public override async Task<RespuestaExito> EliminarInscripcion(SolicitudId request, ServerCallContext context)
    {
        var i = await _userDb.Enrollments.FindAsync((int)request.Id);
        if (i == null) return new RespuestaExito { Exito = false };
        _userDb.Enrollments.Remove(i);
        await _userDb.SaveChangesAsync();
        return new RespuestaExito { Exito = true };
    }

    public override async Task<RespuestaConteo> ContarInscripciones(Empty request, ServerCallContext context)
    {
        return new RespuestaConteo { Total = await _userDb.Enrollments.CountAsync() };
    }

    // ===== Mappers =====

    private static MensajeUsuario MapearUsuario(Usuario u) => new()
    {
        Id = u.Id,
        Nombre = u.Name,
        Email = u.Email,
        Password = u.Password,
        CreadoEn = u.CreatedAt.ToString("O")
    };

    private static MensajeCurso MapearCurso(Curso c) => new()
    {
        Id = c.Id,
        Titulo = c.Title,
        Descripcion = c.Description ?? string.Empty,
        Nivel = c.Level ?? string.Empty,
        Idioma = c.Language ?? string.Empty,
        EsPublicado = c.IsPublished,
        CreadoEn = c.CreatedAt.ToString("O"),
        ActualizadoEn = (c.UpdatedAt ?? c.CreatedAt).ToString("O")
    };

    private static MensajeInscripcion MapearInscripcion(Inscripcion i) => new()
    {
        Id = i.Id,
        UsuarioId = i.UserId,
        CursoId = i.CourseId,
        InscritoEn = i.EnrolledAt.ToString("O"),
        Progreso = i.Progress
    };
}
