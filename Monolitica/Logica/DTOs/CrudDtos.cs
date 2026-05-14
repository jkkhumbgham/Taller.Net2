namespace Monolitica.Logica.DTOs;

public record UsuarioDto(long Id, string Nombre, string Email, string Password, DateTime CreadoEn);
public record CrearUsuarioRequest(string Nombre, string Email, string Password);
public record ActualizarUsuarioRequest(string Nombre, string Email, string? Password);

public record CursoDto(long Id, string Titulo, string? Descripcion, string? Nivel, string? Idioma, bool EsPublicado, DateTime CreadoEn, DateTime? ActualizadoEn);
public record CrearCursoRequest(string Titulo, string? Descripcion, string? Nivel, string? Idioma, bool EsPublicado);
public record ActualizarCursoRequest(string Titulo, string? Descripcion, string? Nivel, string? Idioma, bool EsPublicado);

public record InscripcionDto(long Id, long UsuarioId, long CursoId, DateTime InscritoEn, double Progreso);
public record CrearInscripcionRequest(long UsuarioId, long CursoId);
