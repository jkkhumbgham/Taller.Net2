using Clases.Contratos;
using Clases.Logica.Servicios;

namespace Clases.Contratos;

public class ServicioClasesSoap : IServicioClasesSoap
{
    private readonly IServicioClases _servicio;

    public ServicioClasesSoap(IServicioClases servicio)
    {
        _servicio = servicio;
    }

    public async Task<ListaClasesMasTomadas> ObtenerClasesMasTomadosAsync()
    {
        var resultado = await _servicio.ObtenerMasTomadas();
        return new ListaClasesMasTomadas
        {
            Clases = resultado.Select(c => new ClaseMasTomadaSoap
            {
                IdCurso = c.IdCurso,
                TituloCurso = c.TituloCurso,
                TotalInscritos = c.TotalInscritos,
                PorcentajeCompletados = c.PorcentajeCompletados
            }).ToList()
        };
    }

    public async Task<ListaCursosAcabados> ObtenerCursosAcabadosAsync(int userId)
    {
        var resultado = await _servicio.ObtenerCursosAcabadosEstudianteAsync(userId);
        if (resultado == null)
            return new ListaCursosAcabados();

        return new ListaCursosAcabados
        {
            Cursos = resultado.Select(c => new CursoAcabadoSoap
            {
                IdCurso = c.IdCurso,
                TituloCurso = c.TituloCurso,
                FechaInscripcion = c.FechaInscripcion,
                FechaCompletado = c.FechaCompletado,
                TotalLecciones = c.TotalLecciones,
                DuracionTotalSegundos = (double)c.DuracionTotalSegundos
            }).ToList()
        };
    }
}
