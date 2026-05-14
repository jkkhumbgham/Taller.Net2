using Estadisticas.Contratos;
using Estadisticas.Logica.Servicios;

namespace Estadisticas.Contratos;

public class ServicioEstadisticasSoap : IServicioEstadisticasSoap
{
    private readonly IServicioEstadisticas _servicio;

    public ServicioEstadisticasSoap(IServicioEstadisticas servicio)
    {
        _servicio = servicio;
    }

    public async Task<ResumenEstudianteSoap> ObtenerResumenEstudianteAsync(int userId)
    {
        var resultado = await _servicio.ObtenerResumenEstudianteAsync(userId);
        if (resultado == null)
            return new ResumenEstudianteSoap { Encontrado = false };

        return new ResumenEstudianteSoap
        {
            Encontrado = true,
            IdEstudiante = resultado.IdEstudiante,
            NombreEstudiante = resultado.NombreEstudiante,
            EmailEstudiante = resultado.EmailEstudiante,
            TotalCursosInscritos = resultado.TotalCursosInscritos,
            CursosActivos = resultado.CursosActivos,
            CursosCompletados = resultado.CursosCompletados,
            TotalTiempoInvertido = resultado.TotalTiempoInvertido,
            TotalLeccionesCompletadas = resultado.TotalLeccionesCompletadas,
            PromedioCalificacionCuestionarios = resultado.PromedioCalificacionCuestionarios
        };
    }

    public async Task<ListaEstadisticasCursoSoap> ObtenerEstadisticasCursosAsync(int userId)
    {
        var resultado = await _servicio.ObtenerEstadisticasCursosAsync(userId);
        if (resultado == null)
            return new ListaEstadisticasCursoSoap();

        return new ListaEstadisticasCursoSoap
        {
            Cursos = resultado.Select(c => new EstadisticasCursoSoap
            {
                IdCurso = c.IdCurso,
                TituloCurso = c.TituloCurso,
                PorcentajeProgreso = c.PorcentajeProgreso,
                LeccionesCompletadas = c.LeccionesCompletadas,
                TotalLecciones = c.TotalLecciones,
                TiempoInvertido = c.TiempoInvertido,
                FechaInscripcion = c.FechaInscripcion,
                EstadoCurso = c.EstadoCurso
            }).ToList()
        };
    }
}
