using ClasesMasTomadas.Datos.Repositorios;
using ClasesMasTomadas.Logica.DTOs;

namespace ClasesMasTomadas.Logica.Servicios;

public class ServicioClasesMasTomadas : IServicioClasesMasTomadas
{
    private readonly IRepositorioClasesMasTomadas _repo;

    public ServicioClasesMasTomadas(IRepositorioClasesMasTomadas repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<ClaseMasTomadaDto>> ObtenerClasesMasTomadas()
    {
        var inscripciones = await _repo.ObtenerTodasLasInscripcionesAsync();
        var inscripcionesList = inscripciones.ToList();

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

    public async Task<IEnumerable<ClaseMasTomadaDto>> ObtenerTopClasesMasTomadas(int n)
    {
        var todas = await ObtenerClasesMasTomadas();
        return todas.Take(n).ToList();
    }
}
