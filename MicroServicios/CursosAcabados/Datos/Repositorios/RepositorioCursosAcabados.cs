using Microsoft.EntityFrameworkCore;
using CursosAcabados.Datos.Contexto;
using CursosAcabados.Datos.Modelos;

namespace CursosAcabados.Datos.Repositorios;

public class RepositorioCursosAcabados : IRepositorioCursosAcabados
{
    private readonly UserDbContext _userContext;
    private readonly ContentDbContext _contentContext;

    public RepositorioCursosAcabados(UserDbContext userContext, ContentDbContext contentContext)
    {
        _userContext = userContext;
        _contentContext = contentContext;
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

    public async Task<Curso?> ObtenerCursoPorIdAsync(int courseId)
    {
        return await _contentContext.Courses.FindAsync(courseId);
    }

    public async Task<IEnumerable<Leccion>> ObtenerLeccionesPorCursoAsync(int courseId)
    {
        var moduleIds = await _contentContext.Modules
            .Where(m => m.CourseId == courseId)
            .Select(m => m.Id)
            .ToListAsync();

        return await _contentContext.Lessons
            .Where(l => moduleIds.Contains(l.ModuleId))
            .ToListAsync();
    }

    public async Task<IEnumerable<ProgresoLeccion>> ObtenerProgresosPorUsuarioYLeccionesAsync(int userId, IEnumerable<int> lessonIds)
    {
        return await _userContext.LessonProgress
            .Where(lp => lp.UserId == userId && lessonIds.Contains(lp.LessonId))
            .ToListAsync();
    }
}
