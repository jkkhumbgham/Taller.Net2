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
        var intentos = (await _repo.ObtenerTodosLosIntentosAsync()).ToList();

        var usuarios = (await _repo.ObtenerTodosLosUsuariosAsync())
            .ToDictionary(u => u.Id);

        var cursosCompletadosPorUsuario =
            (await _repo.ObtenerTodasLasInscripcionesAsync())
            .Where(i => i.Progress >= 100)
            .GroupBy(i => i.UserId)
            .ToDictionary(
                g => g.Key,
                g => g.Count());

        var resultado = new List<MejorEstudianteDto>();

        foreach (var grupo in intentos.GroupBy(i => i.UserId))
        {
            usuarios.TryGetValue(grupo.Key, out var usuario);

            var listaIntentos = grupo.ToList();

            var promedio = listaIntentos
                .Where(i => i.MaxScore > 0)
                .Select(i => (i.Score / i.MaxScore) * 100)
                .DefaultIfEmpty(0)
                .Average();

            cursosCompletadosPorUsuario.TryGetValue(
                grupo.Key,
                out var cursosCompletados);

            resultado.Add(new MejorEstudianteDto
            {
                Posicion = 0,
                IdEstudiante = grupo.Key,
                NombreEstudiante = usuario?.Name ?? $"Estudiante {grupo.Key}",
                PromedioCalificacion = Math.Round(promedio, 2),
                TotalIntentos = listaIntentos.Count,
                CursosCompletados = cursosCompletados
            });
        }

        return resultado
            .OrderByDescending(x => x.PromedioCalificacion)
            .Select((x, index) =>
            {
                x.Posicion = index + 1;
                return x;
            })
            .ToList();
    }

    public async Task<IEnumerable<MejorEstudianteDto>> ObtenerTopMejoresEstudiantesAsync(int n)
    {
        return (await ObtenerMejoresEstudiantesAsync())
            .Take(n)
            .ToList();
    }
}