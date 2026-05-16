using Clases.Datos.Repositorios;
using Clases.Logica.DTOs;

namespace Clases.Logica.Servicios;

public class ServicioClases : IServicioClases
{
    private readonly IRepositorioClases _repo;

    public ServicioClases(IRepositorioClases repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<CursoAcabadoDto>?> ObtenerCursosAcabadosEstudianteAsync(int userId)
    {
        var usuario = await _repo.ObtenerUsuarioPorIdAsync(userId);
        if (usuario == null) return null;

        var inscripciones = await _repo.ObtenerInscripcionesPorUsuarioAsync(userId);
        var completadas = inscripciones.Where(i => i.Progress >= 100).ToList();

        var resultado = new List<CursoAcabadoDto>();
        foreach (var inscripcion in completadas)
        {
            var curso = await _repo.ObtenerCursoPorIdAsync(inscripcion.CourseId);
            var lecciones = await _repo.ObtenerLeccionesPorCursoAsync(inscripcion.CourseId);
            var leccionesList = lecciones.ToList();
            var lessonIds = leccionesList.Select(l => l.Id).ToList();
            var progresos = await _repo.ObtenerProgresosPorUsuarioYLeccionesAsync(userId, lessonIds);
            var progresosLista = progresos.ToList();

            var fechaCompletado = progresosLista
                .Where(p => p.CompletedAt.HasValue)
                .Select(p => p.CompletedAt)
                .Max();

            var duracionTotal = leccionesList.Sum(l => l.Duration ?? 0);

            resultado.Add(new CursoAcabadoDto
            {
                IdCurso = inscripcion.CourseId,
                TituloCurso = curso?.Title ?? $"Curso {inscripcion.CourseId}",
                FechaInscripcion = inscripcion.EnrolledAt,
                FechaCompletado = fechaCompletado,
                TotalLecciones = leccionesList.Count,
                DuracionTotalSegundos = duracionTotal
            });
        }

        return resultado;
    }

    public async Task<IEnumerable<ClaseMasTomadaDto>> ObtenerMasTomadas()
    {
        var todasInscripciones = await _repo.ObtenerTodasLasInscripcionesAsync();
        var inscripcionesList = todasInscripciones.ToList();

        var courseIds = inscripcionesList.Select(i => i.CourseId).Distinct().ToList();
        var cursos = await _repo.ObtenerCursosPorIdsAsync(courseIds);
        var cursosDict = cursos.ToDictionary(c => c.Id);

        return inscripcionesList
            .GroupBy(i => i.CourseId)
            .Select(g =>
            {
                cursosDict.TryGetValue(g.Key, out var curso);
                var total = g.Count();
                var completados = g.Count(i => i.Progress >= 100);
                var porcentaje = total > 0 ? Math.Round((double)completados / total * 100, 2) : 0;
                return new ClaseMasTomadaDto
                {
                    IdCurso = g.Key,
                    TituloCurso = curso?.Title ?? $"Curso {g.Key}",
                    TotalInscritos = total,
                    PorcentajeCompletados = porcentaje
                };
            })
            .OrderByDescending(c => c.TotalInscritos)
            .ToList();
    }

    public async Task<IEnumerable<ClaseMasTomadaDto>> ObtenerTopMasTomadas(int n)
    {
        var todas = await ObtenerMasTomadas();
        return todas.Take(n).ToList();
    }
}
