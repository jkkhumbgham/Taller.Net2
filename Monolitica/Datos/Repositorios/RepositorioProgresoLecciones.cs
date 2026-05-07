using Microsoft.EntityFrameworkCore;
using Monolitica.Datos.Contexto;
using Monolitica.Datos.Modelos;

namespace Monolitica.Datos.Repositorios;

public class RepositorioProgresoLecciones : IRepositorioProgresoLecciones
{
    private readonly UserDbContext _userContext;
    private readonly ContentDbContext _contentContext;

    public RepositorioProgresoLecciones(UserDbContext userContext, ContentDbContext contentContext)
    {
        _userContext = userContext;
        _contentContext = contentContext;
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

    public async Task<IEnumerable<Leccion>> ObtenerLeccionesPorModulosAsync(IEnumerable<int> moduleIds)
    {
        return await _contentContext.Lessons
            .Where(l => moduleIds.Contains(l.ModuleId))
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

    public async Task<IEnumerable<Modulo>> ObtenerModulosPorCursoAsync(int courseId)
    {
        return await _contentContext.Modules
            .Where(m => m.CourseId == courseId)
            .ToListAsync();
    }
}
