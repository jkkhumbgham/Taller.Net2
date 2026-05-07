using Estadisticas.Datos.Repositorios;
using Estadisticas.Logica.DTOs;

namespace Estadisticas.Logica.Servicios;

public class ServicioEstadisticas : IServicioEstadisticas
{
    private readonly IRepositorioEstadisticas _repo;

    public ServicioEstadisticas(IRepositorioEstadisticas repo)
    {
        _repo = repo;
    }

    public async Task<ResumenEstudianteDto?> ObtenerResumenEstudianteAsync(int userId)
    {
        var usuario = await _repo.ObtenerUsuarioPorIdAsync(userId);
        if (usuario == null) return null;

        var inscripciones = await _repo.ObtenerInscripcionesPorUsuarioAsync(userId);
        var inscripcionesList = inscripciones.ToList();

        var progresos = await _repo.ObtenerProgresosPorUsuarioAsync(userId);
        var progresosLista = progresos.ToList();

        var intentos = await _repo.ObtenerIntentosPorUsuarioAsync(userId);
        var intentosList = intentos.ToList();

        var cursosActivos = inscripcionesList.Count(i => i.Progress > 0 && i.Progress < 100);
        var cursosCompletados = inscripcionesList.Count(i => i.Progress >= 100);
        var leccionesCompletadas = progresosLista.Count(lp => lp.Status == "COMPLETED");
        var tiempoTotal = progresosLista.Sum(lp => lp.TimeSpent);

        double promedioCalificacion = 0;
        if (intentosList.Any())
        {
            promedioCalificacion = intentosList
                .Where(i => i.MaxScore > 0)
                .Select(i => (i.Score / i.MaxScore) * 100)
                .DefaultIfEmpty(0)
                .Average();
        }

        return new ResumenEstudianteDto
        {
            IdEstudiante = usuario.Id,
            NombreEstudiante = usuario.Name,
            EmailEstudiante = usuario.Email,
            TotalCursosInscritos = inscripcionesList.Count,
            CursosActivos = cursosActivos,
            CursosCompletados = cursosCompletados,
            TotalTiempoInvertido = tiempoTotal,
            TotalLeccionesCompletadas = leccionesCompletadas,
            PromedioCalificacionCuestionarios = Math.Round(promedioCalificacion, 2)
        };
    }

    public async Task<IEnumerable<EstadisticasCursoDto>?> ObtenerEstadisticasCursosAsync(int userId)
    {
        var usuario = await _repo.ObtenerUsuarioPorIdAsync(userId);
        if (usuario == null) return null;

        var inscripciones = await _repo.ObtenerInscripcionesPorUsuarioAsync(userId);
        var inscripcionesList = inscripciones.ToList();

        var courseIds = inscripcionesList.Select(i => i.CourseId).ToList();
        var cursos = await _repo.ObtenerCursosPorIdsAsync(courseIds);
        var cursosDict = cursos.ToDictionary(c => c.Id);

        var resultado = new List<EstadisticasCursoDto>();

        foreach (var inscripcion in inscripcionesList)
        {
            var lecciones = await _repo.ObtenerLeccionesPorCursoAsync(inscripcion.CourseId);
            var leccionesList = lecciones.ToList();
            var lessonIds = leccionesList.Select(l => l.Id).ToList();

            var progresos = await _repo.ObtenerProgresosPorUsuarioYLeccionesAsync(userId, lessonIds);
            var progresosLista = progresos.ToList();

            var leccionesCompletadas = progresosLista.Count(lp => lp.Status == "COMPLETED");
            var tiempoInvertido = progresosLista.Sum(lp => lp.TimeSpent);

            cursosDict.TryGetValue(inscripcion.CourseId, out var curso);

            resultado.Add(new EstadisticasCursoDto
            {
                IdCurso = inscripcion.CourseId,
                TituloCurso = curso?.Title ?? $"Curso {inscripcion.CourseId}",
                PorcentajeProgreso = inscripcion.Progress,
                LeccionesCompletadas = leccionesCompletadas,
                TotalLecciones = leccionesList.Count,
                TiempoInvertido = tiempoInvertido,
                FechaInscripcion = inscripcion.EnrolledAt,
                EstadoCurso = inscripcion.Progress >= 100 ? "COMPLETADO" : inscripcion.Progress > 0 ? "EN_PROGRESO" : "NO_INICIADO"
            });
        }

        return resultado;
    }

    public async Task<IEnumerable<EstadisticasLeccionDto>?> ObtenerEstadisticasLeccionesAsync(int userId)
    {
        var usuario = await _repo.ObtenerUsuarioPorIdAsync(userId);
        if (usuario == null) return null;

        var progresos = await _repo.ObtenerProgresosPorUsuarioAsync(userId);
        var progresosLista = progresos.ToList();

        var inscripciones = await _repo.ObtenerInscripcionesPorUsuarioAsync(userId);
        var courseIds = inscripciones.Select(i => i.CourseId).ToList();

        var todasLecciones = new List<Estadisticas.Datos.Modelos.Leccion>();
        foreach (var courseId in courseIds)
        {
            var lecciones = await _repo.ObtenerLeccionesPorCursoAsync(courseId);
            todasLecciones.AddRange(lecciones);
        }

        var leccionesDict = todasLecciones
            .GroupBy(l => l.Id)
            .ToDictionary(g => g.Key, g => g.First());

        return progresosLista.Select(p =>
        {
            leccionesDict.TryGetValue(p.LessonId, out var leccion);
            return new EstadisticasLeccionDto
            {
                IdLeccion = p.LessonId,
                TituloLeccion = leccion?.Title ?? $"Leccion {p.LessonId}",
                Estado = p.Status,
                TiempoInvertido = p.TimeSpent,
                FechaCompletado = p.CompletedAt,
                PorcentajeProgreso = p.ProgressPercent
            };
        }).ToList();
    }
}
