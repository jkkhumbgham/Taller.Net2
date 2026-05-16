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
            var tiempoTotal = progresos.Sum(p => p.TimeSpent);

            resultado.Add(new CursoAcabadoDto
            {
                IdCurso = inscripcion.CourseId,
                TituloCurso = curso?.Title ?? $"Curso {inscripcion.CourseId}",
                FechaInscripcion = inscripcion.EnrolledAt,
                TotalLecciones = leccionesList.Count,
                TiempoTotalSegundos = tiempoTotal
            });
        }

        return resultado;
    }

    public async Task<TotalCursosAcabadosDto?> ObtenerTotalCursosAcabadosAsync(int userId)
    {
        var usuario = await _repo.ObtenerUsuarioPorIdAsync(userId);
        if (usuario == null) return null;

        var inscripciones = await _repo.ObtenerInscripcionesPorUsuarioAsync(userId);
        var total = inscripciones.Count(i => i.Progress >= 100);

        return new TotalCursosAcabadosDto
        {
            IdEstudiante = usuario.Id,
            Nombre = usuario.Name,
            TotalCursosAcabados = total
        };
    }
}
