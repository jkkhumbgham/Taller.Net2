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
        var inscripciones = (await _repo.ObtenerTodasLasInscripcionesAsync()).ToList();

        if (!inscripciones.Any())
            return Enumerable.Empty<ClaseMasTomadaDto>();

        var cursos = (await _repo.ObtenerCursosPorIdsAsync(
                inscripciones.Select(i => i.CourseId).Distinct()))
            .ToDictionary(c => c.Id);

        var resultado = new List<ClaseMasTomadaDto>();

        foreach (var grupo in inscripciones.GroupBy(i => i.CourseId))
        {
            var total = grupo.Count();
            var completados = grupo.Count(i => i.Progress >= 100);

            cursos.TryGetValue(grupo.Key, out var curso);

            resultado.Add(new ClaseMasTomadaDto
            {
                IdCurso = grupo.Key,
                TituloCurso = curso?.Title ?? $"Curso {grupo.Key}",
                TotalInscritos = total,
                PorcentajeCompletados = total == 0
                    ? 0
                    : Math.Round((double)completados * 100 / total, 2)
            });
        }

        return resultado
            .OrderByDescending(x => x.TotalInscritos)
            .ToList();
    }

    public async Task<IEnumerable<ClaseMasTomadaDto>> ObtenerTopClasesMasTomadas(int n)
    {
        return (await ObtenerClasesMasTomadas())
            .Take(n)
            .ToList();
    }
}