using Microsoft.EntityFrameworkCore;
using Estadisticas.Datos.Contexto;
using Estadisticas.Datos.Modelos;

namespace Estadisticas.Datos.Repositorios;

public class RepositorioEstadisticas : IRepositorioEstadisticas
{
    private readonly UserDbContext _userContext;
    private readonly ContentDbContext _contentContext;

    public RepositorioEstadisticas(UserDbContext userContext, ContentDbContext contentContext)
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

    public async Task<IEnumerable<ProgresoLeccion>> ObtenerProgresosPorUsuarioAsync(int userId)
    {
        return await _userContext.LessonProgress
            .Where(lp => lp.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProgresoLeccion>> ObtenerProgresosPorUsuarioYLeccionesAsync(int userId, IEnumerable<int> lessonIds)
    {
        return await _userContext.LessonProgress
            .Where(lp => lp.UserId == userId && lessonIds.Contains(lp.LessonId))
            .ToListAsync();
    }

    public async Task<IEnumerable<IntentoCuestionario>> ObtenerIntentosPorUsuarioAsync(int userId)
    {
        return await _userContext.QuizAttempts
            .Where(qa => qa.UserId == userId)
            .ToListAsync();
    }

    public async Task<Curso?> ObtenerCursoPorIdAsync(int courseId)
    {
        return await _contentContext.Courses.FindAsync(courseId);
    }

    public async Task<IEnumerable<Curso>> ObtenerCursosPorIdsAsync(IEnumerable<int> courseIds)
    {
        return await _contentContext.Courses
            .Where(c => courseIds.Contains(c.Id))
            .ToListAsync();
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
}
