using Notas.Datos.Repositorios;
using Notas.Logica.DTOs;

namespace Notas.Logica.Servicios;

public class ServicioNotas : IServicioNotas
{
    private readonly IRepositorioNotas _repo;

    public ServicioNotas(IRepositorioNotas repo)
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
                NombreCuestionario = cuestionario?.Title ?? $"Cuestionario {i.QuizId}",
                Calificacion = i.Score,
                CalificacionMaxima = i.MaxScore,
                PorcentajeCalificacion = porcentaje,
                NumeroIntento = i.AttemptNumber,
                FechaIntento = i.AttemptedAt
            };
        }).ToList();
    }

    public async Task<PromedioEstudianteDto?> ObtenerPromedioEstudianteAsync(int userId)
    {
        var usuario = await _repo.ObtenerUsuarioPorIdAsync(userId);
        if (usuario == null) return null;

        var intentos = await _repo.ObtenerIntentosPorUsuarioAsync(userId);
        var intentosList = intentos.ToList();

        var promedio = intentosList.Where(i => i.MaxScore > 0)
            .Select(i => (i.Score / i.MaxScore) * 100)
            .DefaultIfEmpty(0)
            .Average();

        return new PromedioEstudianteDto
        {
            IdEstudiante = usuario.Id,
            Nombre = usuario.Name,
            PromedioCalificacion = Math.Round(promedio, 2),
            TotalIntentos = intentosList.Count
        };
    }

    public async Task<ResumenEstudianteDto?> ObtenerResumenEstudianteAsync(int userId)
    {
        var usuario = await _repo.ObtenerUsuarioPorIdAsync(userId);
        if (usuario == null) return null;

        var enrollments = (await _repo.ObtenerEnrollmentsPorUsuarioAsync(userId)).ToList();
        var intentos = (await _repo.ObtenerIntentosPorUsuarioAsync(userId)).ToList();

        var promedio = intentos.Where(i => i.MaxScore > 0)
            .Select(i => (i.Score / i.MaxScore) * 100)
            .DefaultIfEmpty(0)
            .Average();

        return new ResumenEstudianteDto
        {
            IdEstudiante = usuario.Id,
            NombreEstudiante = usuario.Name,
            EmailEstudiante = usuario.Email,
            TotalCursosInscritos = enrollments.Count,
            CursosActivos = enrollments.Count(e => e.Progress < 1.0),
            CursosCompletados = enrollments.Count(e => e.Progress >= 1.0),
            TotalTiempoInvertido = 0,
            TotalLeccionesCompletadas = 0,
            PromedioCalificacionCuestionarios = Math.Round(promedio, 2)
        };
    }
}
