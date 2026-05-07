using Microsoft.EntityFrameworkCore;
using MejoresEstudiantes.Datos.Contexto;
using MejoresEstudiantes.Datos.Modelos;

namespace MejoresEstudiantes.Datos.Repositorios;

public class RepositorioMejoresEstudiantes : IRepositorioMejoresEstudiantes
{
    private readonly UserDbContext _userContext;

    public RepositorioMejoresEstudiantes(UserDbContext userContext)
    {
        _userContext = userContext;
    }

    public async Task<IEnumerable<IntentoCuestionario>> ObtenerTodosLosIntentosAsync()
    {
        return await _userContext.QuizAttempts.ToListAsync();
    }

    public async Task<IEnumerable<Usuario>> ObtenerTodosLosUsuariosAsync()
    {
        return await _userContext.Users.ToListAsync();
    }

    public async Task<IEnumerable<Inscripcion>> ObtenerTodasLasInscripcionesAsync()
    {
        return await _userContext.Enrollments.ToListAsync();
    }
}
