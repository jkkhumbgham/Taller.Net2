using Microsoft.EntityFrameworkCore;
using Notas.Datos.Contexto;
using Notas.Datos.Modelos;

namespace Notas.Datos.Repositorios;

public class RepositorioNotas : IRepositorioNotas
{
    private readonly UserDbContext _userContext;
    private readonly ContentDbContext _contentContext;

    public RepositorioNotas(UserDbContext userContext, ContentDbContext contentContext)
    {
        _userContext = userContext;
        _contentContext = contentContext;
    }

    public async Task<Usuario?> ObtenerUsuarioPorIdAsync(int userId)
    {
        return await _userContext.Users.FindAsync(userId);
    }

    public async Task<IEnumerable<IntentoCuestionario>> ObtenerIntentosPorUsuarioAsync(int userId)
    {
        return await _userContext.QuizAttempts
            .Where(qa => qa.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Cuestionario>> ObtenerCuestionariosPorIdsAsync(IEnumerable<int> quizIds)
    {
        return await _contentContext.Quizzes
            .Where(q => quizIds.Contains(q.Id))
            .ToListAsync();
    }
}
