using Cuestionarios.Logica.Servicios;

namespace Cuestionarios.Contratos;

public class ServicioCuestionariosSoap : IServicioCuestionariosSoap
{
    private readonly IServicioCuestionarios _servicio;

    public ServicioCuestionariosSoap(IServicioCuestionarios servicio)
    {
        _servicio = servicio;
    }

    public async Task<ListaNotasEstudiante> ObtenerNotasEstudianteAsync(int userId)
    {
        var resultado = await _servicio.ObtenerNotasEstudianteAsync(userId);
        if (resultado == null) return new ListaNotasEstudiante { Encontrado = false };

        return new ListaNotasEstudiante
        {
            Encontrado = true,
            Notas = resultado.Select(n => new NotaEstudianteSoap
            {
                IdIntento = n.IdIntento,
                IdCuestionario = n.IdCuestionario,
                TituloCuestionario = n.TituloCuestionario,
                Calificacion = n.Calificacion,
                CalificacionMaxima = n.CalificacionMaxima,
                PorcentajeCalificacion = n.PorcentajeCalificacion,
                NumeroIntento = n.NumeroIntento,
                FechaIntento = n.FechaIntento
            }).ToList()
        };
    }

    public async Task<ListaMejoresEstudiantes> ObtenerMejoresEstudiantesAsync()
    {
        var resultado = await _servicio.ObtenerMejoresEstudiantesAsync();
        int posicion = 1;
        return new ListaMejoresEstudiantes
        {
            Estudiantes = resultado.Select(e => new MejorEstudianteSoap
            {
                IdEstudiante = e.IdEstudiante,
                NombreEstudiante = e.NombreEstudiante,
                PromedioCalificacion = e.PromedioCalificacion,
                TotalIntentos = e.TotalIntentos,
                Posicion = posicion++
            }).ToList()
        };
    }
}
