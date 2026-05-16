using Microsoft.EntityFrameworkCore;
using Monolitica.Datos.Contexto;
using Monolitica.Datos.Modelos;

namespace Monolitica.Datos.Repositorios;

public class RepositorioIntentosCuestionarios : IRepositorioIntentosCuestionarios
{
    private readonly UserDbContext _userContext;
    private readonly ContentDbContext _contentContext;

    public RepositorioIntentosCuestionarios(UserDbContext userContext, ContentDbContext contentContext)
    {
        _userContext = userContext;
        _contentContext = contentContext;
    }

    public async Task<IEnumerable<IntentoCuestionario>> ObtenerIntentosPorUsuarioAsync(int userId)
    {
        return await _userContext.QuizAttempts
            .Where(qa => qa.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<IntentoCuestionario>> ObtenerIntentosPorUsuarioYCuestionariosAsync(int userId, IEnumerable<int> quizIds)
    {
        return await _userContext.QuizAttempts
            .Where(qa => qa.UserId == userId && quizIds.Contains(qa.QuizId))
            .ToListAsync();
    }

    public async Task<IEnumerable<Cuestionario>> ObtenerCuestionariosPorIdsAsync(IEnumerable<int> quizIds)
    {
        return await _contentContext.Quizzes
            .Where(q => quizIds.Contains(q.Id))
            .ToListAsync();
    }

    public async Task<IEnumerable<IntentoCuestionario>> ObtenerTodosLosIntentosAsync()
    {
        return await _userContext.QuizAttempts.ToListAsync();
    }

    public async Task<IEnumerable<Usuario>> ObtenerTodosLosUsuariosAsync()
    {
        return await _userContext.Users.ToListAsync();
    }
}
