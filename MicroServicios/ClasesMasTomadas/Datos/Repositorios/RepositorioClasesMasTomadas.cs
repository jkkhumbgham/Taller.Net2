using Microsoft.EntityFrameworkCore;
using ClasesMasTomadas.Datos.Contexto;
using ClasesMasTomadas.Datos.Modelos;

namespace ClasesMasTomadas.Datos.Repositorios;

public class RepositorioClasesMasTomadas : IRepositorioClasesMasTomadas
{
    private readonly UserDbContext _userContext;
    private readonly ContentDbContext _contentContext;

    public RepositorioClasesMasTomadas(UserDbContext userContext, ContentDbContext contentContext)
    {
        _userContext = userContext;
        _contentContext = contentContext;
    }

    public async Task<IEnumerable<Inscripcion>> ObtenerTodasLasInscripcionesAsync()
    {
        return await _userContext.Enrollments.ToListAsync();
    }

    public async Task<IEnumerable<Curso>> ObtenerCursosPorIdsAsync(IEnumerable<int> courseIds)
    {
        return await _contentContext.Courses
            .Where(c => courseIds.Contains(c.Id))
            .ToListAsync();
    }
}
