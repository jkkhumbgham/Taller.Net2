using Cuestionarios.Datos.Repositorios;
using Cuestionarios.Logica.DTOs;

namespace Cuestionarios.Logica.Servicios;

public class ServicioCuestionarios : IServicioCuestionarios
{
    private readonly IRepositorioCuestionarios _repo;

    public ServicioCuestionarios(IRepositorioCuestionarios repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<NotaEstudianteDto>?> ObtenerNotasEstudianteAsync(int userId)
    {
        var usuario = await _repo.ObtenerUsuarioPorIdAsync(userId);
        if (usuario == null) return null;

        var intentos = await _repo.ObtenerIntentosPorUsuarioAsync(userId);
        var intentosList = intentos.ToList();

        var quizIds = intentosList.Select(i => i.QuizId).Distinct().ToList();
        var cuestionarios = await _repo.ObtenerCuestionariosPorIdsAsync(quizIds);
        var cuestionariosDict = cuestionarios.ToDictionary(q => q.Id);

        return intentosList.Select(i =>
        {
            cuestionariosDict.TryGetValue(i.QuizId, out var cuestionario);
            var porcentaje = i.MaxScore > 0 ? Math.Round((i.Score / i.MaxScore) * 100, 2) : 0;
            return new NotaEstudianteDto
            {
                IdIntento = i.Id,
                IdCuestionario = i.QuizId,
                TituloCuestionario = cuestionario?.Title ?? $"Cuestionario {i.QuizId}",
                Calificacion = i.Score,
                CalificacionMaxima = i.MaxScore,
                PorcentajeCalificacion = porcentaje,
                NumeroIntento = i.AttemptNumber,
                FechaIntento = i.AttemptedAt
            };
        }).ToList();
    }

    public async Task<MejorEstudianteDto?> ObtenerPromedioEstudianteAsync(int userId)
    {
        var usuario = await _repo.ObtenerUsuarioPorIdAsync(userId);
        if (usuario == null) return null;

        var intentos = await _repo.ObtenerIntentosPorUsuarioAsync(userId);
        var intentosList = intentos.ToList();

        var promedio = intentosList.Where(i => i.MaxScore > 0)
            .Select(i => (i.Score / i.MaxScore) * 100)
            .DefaultIfEmpty(0)
            .Average();

        return new MejorEstudianteDto
        {
            IdEstudiante = usuario.Id,
            NombreEstudiante = usuario.Name,
            PromedioCalificacion = Math.Round(promedio, 2),
            TotalIntentos = intentosList.Count
        };
    }

    public async Task<IEnumerable<MejorEstudianteDto>> ObtenerMejoresEstudiantesAsync()
    {
        var todosIntentos = await _repo.ObtenerTodosLosIntentosAsync();
        var intentosList = todosIntentos.ToList();

        var todosUsuarios = await _repo.ObtenerTodosLosUsuariosAsync();
        var usuariosDict = todosUsuarios.ToDictionary(u => u.Id);

        return intentosList
            .GroupBy(i => i.UserId)
            .Select(g =>
            {
                usuariosDict.TryGetValue(g.Key, out var usuario);
                var intentosGrupo = g.ToList();
                var promedio = intentosGrupo.Where(i => i.MaxScore > 0)
                    .Select(i => (i.Score / i.MaxScore) * 100)
                    .DefaultIfEmpty(0)
                    .Average();
                return new MejorEstudianteDto
                {
                    IdEstudiante = g.Key,
                    NombreEstudiante = usuario?.Name ?? $"Estudiante {g.Key}",
                    PromedioCalificacion = Math.Round(promedio, 2),
                    TotalIntentos = intentosGrupo.Count
                };
            })
            .OrderByDescending(e => e.PromedioCalificacion)
            .ToList();
    }

    public async Task<IEnumerable<MejorEstudianteDto>> ObtenerTopMejoresEstudiantesAsync(int n)
    {
        var todos = await ObtenerMejoresEstudiantesAsync();
        return todos.Take(n).ToList();
    }
}
