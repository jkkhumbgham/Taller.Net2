using Microsoft.EntityFrameworkCore;
using Cuestionarios.Datos.Contexto;
using Cuestionarios.Datos.Modelos;

namespace Cuestionarios.Datos.Repositorios;

public class RepositorioCuestionarios : IRepositorioCuestionarios
{
    private readonly UserDbContext _userContext;
    private readonly ContentDbContext _contentContext;

    public RepositorioCuestionarios(UserDbContext userContext, ContentDbContext contentContext)
    {
        _userContext = userContext;
        _contentContext = contentContext;
    }

    public async Task<Usuario?> ObtenerUsuarioPorIdAsync(int userId)
    {
        return await _userContext.Users.FindAsync(userId);
    }

    public async Task<IEnumerable<Usuario>> ObtenerTodosLosUsuariosAsync()
    {
        return await _userContext.Users.ToListAsync();
    }

    public async Task<IEnumerable<IntentoCuestionario>> ObtenerIntentosPorUsuarioAsync(int userId)
    {
        return await _userContext.QuizAttempts
            .Where(qa => qa.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<IntentoCuestionario>> ObtenerTodosLosIntentosAsync()
    {
        return await _userContext.QuizAttempts.ToListAsync();
    }

    public async Task<IEnumerable<Cuestionario>> ObtenerCuestionariosPorIdsAsync(IEnumerable<int> quizIds)
    {
        return await _contentContext.Quizzes
            .Where(q => quizIds.Contains(q.Id))
            .ToListAsync();
    }
}
