using MejoresEstudiantes.Datos.Repositorios;
using MejoresEstudiantes.Logica.DTOs;

namespace MejoresEstudiantes.Logica.Servicios;

public class ServicioMejoresEstudiantes : IServicioMejoresEstudiantes
{
    private readonly IRepositorioMejoresEstudiantes _repo;

    public ServicioMejoresEstudiantes(IRepositorioMejoresEstudiantes repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<MejorEstudianteDto>> ObtenerMejoresEstudiantesAsync()
    {
        var todosIntentos = await _repo.ObtenerTodosLosIntentosAsync();
        var intentosList = todosIntentos.ToList();

        var todosUsuarios = await _repo.ObtenerTodosLosUsuariosAsync();
        var usuariosDict = todosUsuarios.ToDictionary(u => u.Id);

        var todasInscripciones = await _repo.ObtenerTodasLasInscripcionesAsync();
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
                    Posicion = 0,
                    IdEstudiante = g.Key,
                    NombreEstudiante = usuario?.Name ?? $"Estudiante {g.Key}",
                    PromedioCalificacion = Math.Round(promedio, 2),
                    TotalIntentos = intentosGrupo.Count,
                    CursosCompletados = cursosCompletados
                };
            })
            .OrderByDescending(e => e.PromedioCalificacion)
            .Select((e, index) => { e.Posicion = index + 1; return e; })
            .ToList();
    }

    public async Task<IEnumerable<MejorEstudianteDto>> ObtenerTopMejoresEstudiantesAsync(int n)
    {
        var todos = await ObtenerMejoresEstudiantesAsync();
        return todos.Take(n).ToList();
    }
}
