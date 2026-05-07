using Microsoft.EntityFrameworkCore;
using Monolitica.Datos.Contexto;
using Monolitica.Datos.Modelos;

namespace Monolitica.Datos.Repositorios;

public class RepositorioInscripciones : IRepositorioInscripciones
{
    private readonly UserDbContext _userContext;

    public RepositorioInscripciones(UserDbContext userContext)
    {
        _userContext = userContext;
    }

    public async Task<Usuario?> ObtenerUsuarioPorIdAsync(int userId)
    {
        return await _userContext.Users.FindAsync(userId);
    }

    public async Task<IEnumerable<Inscripcion>> ObtenerInscripcionesPorUsuarioAsync(int userId)
    {
        return await _userContext.Enrollments
            .Where(e => e.UserId == userId)
            .ToListAsync();
    }

    public async Task<Inscripcion?> ObtenerInscripcionPorUsuarioYCursoAsync(int userId, int courseId)
    {
        return await _userContext.Enrollments
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);
    }

    public async Task<IEnumerable<Inscripcion>> ObtenerTodasLasInscripcionesAsync()
    {
        return await _userContext.Enrollments.ToListAsync();
    }
}
