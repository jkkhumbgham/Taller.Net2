using CursosAcabados.Datos.Repositorios;
using CursosAcabados.Logica.DTOs;

namespace CursosAcabados.Logica.Servicios;

public class ServicioCursosAcabados : IServicioCursosAcabados
{
    private readonly IRepositorioCursosAcabados _repo;

    public ServicioCursosAcabados(IRepositorioCursosAcabados repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<CursoAcabadoDto>?> ObtenerCursosAcabadosEstudianteAsync(int userId)
    {
        var usuario = await _repo.ObtenerUsuarioPorIdAsync(userId);

        if (usuario == null)
            return null;

        var inscripciones = await _repo.ObtenerInscripcionesPorUsuarioAsync(userId);

        var completadas = inscripciones
            .Where(i => i.Progress >= 100)
            .ToList();

        var resultado = new List<CursoAcabadoDto>(completadas.Count);

        foreach (var inscripcion in completadas)
        {
            var cursoTask =
                _repo.ObtenerCursoPorIdAsync(inscripcion.CourseId);

            var leccionesTask =
                _repo.ObtenerLeccionesPorCursoAsync(inscripcion.CourseId);

            await Task.WhenAll(cursoTask, leccionesTask);

            var curso = cursoTask.Result;

            var lecciones = leccionesTask.Result.ToList();

            var lessonIds = lecciones
                .Select(x => x.Id)
                .ToList();

            var progresos =
                await _repo.ObtenerProgresosPorUsuarioYLeccionesAsync(
                    userId,
                    lessonIds);

            resultado.Add(new CursoAcabadoDto
            {
                IdCurso = inscripcion.CourseId,
                TituloCurso = curso?.Title ?? $"Curso {inscripcion.CourseId}",
                FechaInscripcion = inscripcion.EnrolledAt,
                TotalLecciones = lecciones.Count,
                TiempoTotalSegundos = progresos.Sum(x => x.TimeSpent)
            });
        }

        return resultado;
    }

    public async Task<TotalCursosAcabadosDto?> ObtenerTotalCursosAcabadosAsync(int userId)
    {
        var usuario = await _repo.ObtenerUsuarioPorIdAsync(userId);

        if (usuario == null)
            return null;

        var inscripciones =
            await _repo.ObtenerInscripcionesPorUsuarioAsync(userId);

        return new TotalCursosAcabadosDto
        {
            IdEstudiante = usuario.Id,
            Nombre = usuario.Name,
            TotalCursosAcabados =
                inscripciones.Count(x => x.Progress >= 100)
        };
    }
}