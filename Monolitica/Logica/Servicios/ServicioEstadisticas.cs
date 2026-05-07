using Monolitica.Datos.Repositorios;
using Monolitica.Logica.DTOs;

namespace Monolitica.Logica.Servicios;

public class ServicioEstadisticas : IServicioEstadisticas
{
    private readonly IRepositorioInscripciones _repoInscripciones;
    private readonly IRepositorioProgresoLecciones _repoProgreso;
    private readonly IRepositorioIntentosCuestionarios _repoIntentos;

    public ServicioEstadisticas(
        IRepositorioInscripciones repoInscripciones,
        IRepositorioProgresoLecciones repoProgreso,
        IRepositorioIntentosCuestionarios repoIntentos)
    {
        _repoInscripciones = repoInscripciones;
        _repoProgreso = repoProgreso;
        _repoIntentos = repoIntentos;
    }

    public async Task<ResumenEstudianteDto?> ObtenerResumenEstudianteAsync(int userId)
    {
        var usuario = await _repoInscripciones.ObtenerUsuarioPorIdAsync(userId);
        if (usuario == null) return null;

        var inscripciones = await _repoInscripciones.ObtenerInscripcionesPorUsuarioAsync(userId);
        var inscripcionesList = inscripciones.ToList();

        var progresosLecciones = await _repoProgreso.ObtenerProgresosPorUsuarioAsync(userId);
        var progresosLista = progresosLecciones.ToList();

        var intentosCuestionarios = await _repoIntentos.ObtenerIntentosPorUsuarioAsync(userId);
        var intentosLista = intentosCuestionarios.ToList();

        var cursosActivos = inscripcionesList.Count(i => i.Progress > 0 && i.Progress < 100);
        var cursosCompletados = inscripcionesList.Count(i => i.Progress >= 100);
        var leccionesCompletadas = progresosLista.Count(lp => lp.Status == "COMPLETED");
        var tiempoTotal = progresosLista.Sum(lp => lp.TimeSpent);

        double promedioCalificacion = 0;
        if (intentosLista.Any())
        {
            promedioCalificacion = intentosLista
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
        var usuario = await _repoInscripciones.ObtenerUsuarioPorIdAsync(userId);
        if (usuario == null) return null;

        var inscripciones = await _repoInscripciones.ObtenerInscripcionesPorUsuarioAsync(userId);
        var inscripcionesList = inscripciones.ToList();

        var courseIds = inscripcionesList.Select(i => i.CourseId).ToList();
        var cursos = await _repoProgreso.ObtenerCursosPorIdsAsync(courseIds);
        var cursosDict = cursos.ToDictionary(c => c.Id);

        var resultado = new List<EstadisticasCursoDto>();

        foreach (var inscripcion in inscripcionesList)
        {
            var lecciones = await _repoProgreso.ObtenerLeccionesPorCursoAsync(inscripcion.CourseId);
            var leccionesList = lecciones.ToList();
            var lessonIds = leccionesList.Select(l => l.Id).ToList();

            var progresos = await _repoProgreso.ObtenerProgresosPorUsuarioYLeccionesAsync(userId, lessonIds);
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

    public async Task<EstadisticasDetalleCursoDto?> ObtenerEstadisticasDetalleCursoAsync(int userId, int courseId)
    {
        var usuario = await _repoInscripciones.ObtenerUsuarioPorIdAsync(userId);
        if (usuario == null) return null;

        var inscripcion = await _repoInscripciones.ObtenerInscripcionPorUsuarioYCursoAsync(userId, courseId);
        if (inscripcion == null) return null;

        var curso = await _repoProgreso.ObtenerCursoPorIdAsync(courseId);
        var lecciones = await _repoProgreso.ObtenerLeccionesPorCursoAsync(courseId);
        var leccionesList = lecciones.ToList();
        var lessonIds = leccionesList.Select(l => l.Id).ToList();

        var progresos = await _repoProgreso.ObtenerProgresosPorUsuarioYLeccionesAsync(userId, lessonIds);
        var progresosDict = progresos.ToDictionary(p => p.LessonId);

        var estadisticasLecciones = leccionesList.Select(l =>
        {
            progresosDict.TryGetValue(l.Id, out var progreso);
            return new EstadisticasLeccionDto
            {
                IdLeccion = l.Id,
                TituloLeccion = l.Title,
                Estado = progreso?.Status ?? "NOT_STARTED",
                TiempoInvertido = progreso?.TimeSpent ?? 0,
                FechaCompletado = progreso?.CompletedAt,
                PorcentajeProgreso = progreso?.ProgressPercent ?? 0
            };
        }).ToList();

        var tiempoInvertido = progresos.Sum(p => p.TimeSpent);
        var leccionesCompletadas = estadisticasLecciones.Count(l => l.Estado == "COMPLETED");

        // Get quiz stats for this course
        var cuestionariosStats = await ObtenerCuestionariosParaCursoAsync(userId, courseId);

        return new EstadisticasDetalleCursoDto
        {
            IdCurso = courseId,
            TituloCurso = curso?.Title ?? $"Curso {courseId}",
            PorcentajeProgreso = inscripcion.Progress,
            LeccionesCompletadas = leccionesCompletadas,
            TotalLecciones = leccionesList.Count,
            TiempoInvertido = tiempoInvertido,
            FechaInscripcion = inscripcion.EnrolledAt,
            EstadoCurso = inscripcion.Progress >= 100 ? "COMPLETADO" : inscripcion.Progress > 0 ? "EN_PROGRESO" : "NO_INICIADO",
            Lecciones = estadisticasLecciones,
            Cuestionarios = cuestionariosStats
        };
    }

    public async Task<IEnumerable<EstadisticasCuestionarioDto>?> ObtenerEstadisticasCuestionariosAsync(int userId)
    {
        var usuario = await _repoInscripciones.ObtenerUsuarioPorIdAsync(userId);
        if (usuario == null) return null;

        var intentos = await _repoIntentos.ObtenerIntentosPorUsuarioAsync(userId);
        var intentosList = intentos.ToList();

        var quizIds = intentosList.Select(i => i.QuizId).Distinct().ToList();
        var cuestionarios = await _repoIntentos.ObtenerCuestionariosPorIdsAsync(quizIds);
        var cuestionariosDict = cuestionarios.ToDictionary(q => q.Id);

        return intentosList
            .GroupBy(i => i.QuizId)
            .Select(g =>
            {
                cuestionariosDict.TryGetValue(g.Key, out var cuestionario);
                var intentosGrupo = g.ToList();
                var mejorIntento = intentosGrupo.OrderByDescending(i => i.Score).First();
                var maxScore = mejorIntento.MaxScore > 0 ? mejorIntento.MaxScore : 100;
                var promedioScore = intentosGrupo.Where(i => i.MaxScore > 0)
                    .Select(i => (i.Score / i.MaxScore) * 100)
                    .DefaultIfEmpty(0)
                    .Average();
                var mejorScore = mejorIntento.MaxScore > 0 ? (mejorIntento.Score / mejorIntento.MaxScore) * 100 : 0;

                return new EstadisticasCuestionarioDto
                {
                    IdCuestionario = g.Key,
                    TituloCuestionario = cuestionario?.Title ?? $"Cuestionario {g.Key}",
                    TotalIntentos = intentosGrupo.Count,
                    MejorCalificacion = Math.Round(mejorScore, 2),
                    CalificacionPromedio = Math.Round(promedioScore, 2),
                    CalificacionMaxima = maxScore,
                    Aprobado = mejorScore >= 60
                };
            }).ToList();
    }

    public async Task<IEnumerable<EstadisticasLeccionDto>?> ObtenerEstadisticasLeccionesAsync(int userId)
    {
        var usuario = await _repoInscripciones.ObtenerUsuarioPorIdAsync(userId);
        if (usuario == null) return null;

        var progresos = await _repoProgreso.ObtenerProgresosPorUsuarioAsync(userId);
        var progresosLista = progresos.ToList();

        var lessonIds = progresosLista.Select(p => p.LessonId).Distinct().ToList();

        // Get all module IDs that have these lessons
        var inscripciones = await _repoInscripciones.ObtenerInscripcionesPorUsuarioAsync(userId);
        var courseIds = inscripciones.Select(i => i.CourseId).ToList();

        var todasLecciones = new List<Monolitica.Datos.Modelos.Leccion>();
        foreach (var courseId in courseIds)
        {
            var lecciones = await _repoProgreso.ObtenerLeccionesPorCursoAsync(courseId);
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

    public async Task<IEnumerable<NotaEstudianteDto>?> ObtenerNotasEstudianteAsync(int userId)
    {
        var usuario = await _repoInscripciones.ObtenerUsuarioPorIdAsync(userId);
        if (usuario == null) return null;

        var intentos = await _repoIntentos.ObtenerIntentosPorUsuarioAsync(userId);
        var intentosList = intentos.ToList();

        var quizIds = intentosList.Select(i => i.QuizId).Distinct().ToList();
        var cuestionarios = await _repoIntentos.ObtenerCuestionariosPorIdsAsync(quizIds);
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

    public async Task<IEnumerable<CursoAcabadoDto>?> ObtenerCursosAcabadosEstudianteAsync(int userId)
    {
        var usuario = await _repoInscripciones.ObtenerUsuarioPorIdAsync(userId);
        if (usuario == null) return null;

        var inscripciones = await _repoInscripciones.ObtenerInscripcionesPorUsuarioAsync(userId);
        var completadas = inscripciones.Where(i => i.Progress >= 100).ToList();

        var resultado = new List<CursoAcabadoDto>();
        foreach (var inscripcion in completadas)
        {
            var curso = await _repoProgreso.ObtenerCursoPorIdAsync(inscripcion.CourseId);
            var lecciones = await _repoProgreso.ObtenerLeccionesPorCursoAsync(inscripcion.CourseId);
            var leccionesList = lecciones.ToList();
            var lessonIds = leccionesList.Select(l => l.Id).ToList();
            var progresos = await _repoProgreso.ObtenerProgresosPorUsuarioYLeccionesAsync(userId, lessonIds);
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

    public async Task<IEnumerable<ClaseMasTomadaDto>> ObtenerClasesMasTomadas()
    {
        var todasInscripciones = await _repoInscripciones.ObtenerTodasLasInscripcionesAsync();
        var inscripcionesList = todasInscripciones.ToList();

        var courseIds = inscripcionesList.Select(i => i.CourseId).Distinct().ToList();
        var cursos = await _repoProgreso.ObtenerCursosPorIdsAsync(courseIds);
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

    public async Task<IEnumerable<MejorEstudianteDto>> ObtenerMejoresEstudiantes()
    {
        var todosIntentos = await _repoIntentos.ObtenerTodosLosIntentosAsync();
        var intentosList = todosIntentos.ToList();

        var todosUsuarios = await _repoIntentos.ObtenerTodosLosUsuariosAsync();
        var usuariosDict = todosUsuarios.ToDictionary(u => u.Id);

        var todasInscripciones = await _repoInscripciones.ObtenerTodasLasInscripcionesAsync();
        var inscripcionesList = todasInscripciones.ToList();

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
                var cursosCompletados = inscripcionesList.Count(i => i.UserId == g.Key && i.Progress >= 100);
                return new MejorEstudianteDto
                {
                    IdEstudiante = g.Key,
                    NombreEstudiante = usuario?.Name ?? $"Estudiante {g.Key}",
                    PromedioCalificacion = Math.Round(promedio, 2),
                    TotalIntentos = intentosGrupo.Count,
                    CursosCompletados = cursosCompletados
                };
            })
            .OrderByDescending(e => e.PromedioCalificacion)
            .ToList();
    }

    private async Task<IEnumerable<EstadisticasCuestionarioDto>> ObtenerCuestionariosParaCursoAsync(int userId, int courseId)
    {
        var intentos = await _repoIntentos.ObtenerIntentosPorUsuarioAsync(userId);
        var intentosList = intentos.ToList();

        var quizIds = intentosList.Select(i => i.QuizId).Distinct().ToList();
        var cuestionarios = await _repoIntentos.ObtenerCuestionariosPorIdsAsync(quizIds);
        var cuestionariosDict = cuestionarios.ToDictionary(q => q.Id);

        return intentosList
            .GroupBy(i => i.QuizId)
            .Select(g =>
            {
                cuestionariosDict.TryGetValue(g.Key, out var cuestionario);
                var intentosGrupo = g.ToList();
                var mejorIntento = intentosGrupo.OrderByDescending(i => i.Score).First();
                var maxScore = mejorIntento.MaxScore > 0 ? mejorIntento.MaxScore : 100;
                var promedioScore = intentosGrupo.Where(i => i.MaxScore > 0)
                    .Select(i => (i.Score / i.MaxScore) * 100)
                    .DefaultIfEmpty(0)
                    .Average();
                var mejorScore = mejorIntento.MaxScore > 0 ? (mejorIntento.Score / mejorIntento.MaxScore) * 100 : 0;

                return new EstadisticasCuestionarioDto
                {
                    IdCuestionario = g.Key,
                    TituloCuestionario = cuestionario?.Title ?? $"Cuestionario {g.Key}",
                    TotalIntentos = intentosGrupo.Count,
                    MejorCalificacion = Math.Round(mejorScore, 2),
                    CalificacionPromedio = Math.Round(promedioScore, 2),
                    CalificacionMaxima = maxScore,
                    Aprobado = mejorScore >= 60
                };
            }).ToList();
    }
}
