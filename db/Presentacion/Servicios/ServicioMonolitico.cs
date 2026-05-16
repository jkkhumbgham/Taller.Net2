using Monolitica.Grpc;
using Presentacion.Modelos.DTOs;
using Presentacion.Servicios.Interfaces;

namespace Presentacion.Servicios;

public class ServicioMonolitico : IServicioMonolitico
{
    private readonly EstadisticasGrpc.EstadisticasGrpcClient _cliente;

    public ServicioMonolitico(EstadisticasGrpc.EstadisticasGrpcClient cliente)
    {
        _cliente = cliente;
    }

    public async Task<ResumenEstudianteDto?> ObtenerResumenEstudianteAsync(int userId)
    {
        var respuesta = await _cliente.ObtenerResumenEstudianteAsync(new SolicitudUsuario { UserId = userId });
        if (!respuesta.Encontrado || respuesta.Resumen == null) return null;
        var r = respuesta.Resumen;
        return new ResumenEstudianteDto
        {
            IdEstudiante = r.IdEstudiante,
            NombreEstudiante = r.NombreEstudiante,
            EmailEstudiante = r.EmailEstudiante,
            TotalCursosInscritos = r.TotalCursosInscritos,
            CursosActivos = r.CursosActivos,
            CursosCompletados = r.CursosCompletados,
            TotalTiempoInvertido = r.TotalTiempoInvertido,
            TotalLeccionesCompletadas = r.TotalLeccionesCompletadas,
            PromedioCalificacionCuestionarios = r.PromedioCalificacionCuestionarios
        };
    }

    public async Task<IEnumerable<EstadisticasCursoDto>> ObtenerEstadisticasCursosAsync(int userId)
    {
        return Enumerable.Empty<EstadisticasCursoDto>();
    }

    public async Task<IEnumerable<EstadisticasCuestionarioDto>> ObtenerEstadisticasCuestionariosAsync(int userId)
    {
        return Enumerable.Empty<EstadisticasCuestionarioDto>();
    }

    public async Task<IEnumerable<NotaEstudianteDto>> ObtenerNotasEstudianteAsync(int userId)
    {
        var respuesta = await _cliente.ObtenerNotasEstudianteAsync(new SolicitudUsuario { UserId = userId });
        return respuesta.Notas.Select(n => new NotaEstudianteDto
        {
            IdIntento = n.IdIntento,
            TituloCuestionario = n.TituloCuestionario,
            NombreCuestionario = n.TituloCuestionario,
            Calificacion = n.Calificacion,
            CalificacionMaxima = n.CalificacionMaxima,
            PorcentajeCalificacion = n.PorcentajeCalificacion,
            NumeroIntento = n.NumeroIntento,
            FechaIntento = DateTime.Parse(n.FechaIntento)
        }).ToList();
    }

    public async Task<IEnumerable<CursoAcabadoDto>> ObtenerCursosAcabadosAsync(int userId)
    {
        var respuesta = await _cliente.ObtenerCursosAcabadosAsync(new SolicitudUsuario { UserId = userId });
        return respuesta.Cursos.Select(c => new CursoAcabadoDto
        {
            IdCurso = c.IdCurso,
            TituloCurso = c.TituloCurso,
            FechaInscripcion = DateTime.Parse(c.FechaInscripcion),
            FechaCompletado = c.TieneFechaCompletado && !string.IsNullOrEmpty(c.FechaCompletado)
                ? DateTime.Parse(c.FechaCompletado)
                : null,
            TotalLecciones = c.TotalLecciones,
            DuracionTotalSegundos = c.DuracionTotalSegundos
        }).ToList();
    }

    public async Task<IEnumerable<ClaseMasTomadaDto>> ObtenerClasesMasTomadosAsync()
    {
        var respuesta = await _cliente.ObtenerClasesMasTomadasAsync(new SolicitudVacia());
        return respuesta.Clases.Select(c => new ClaseMasTomadaDto
        {
            IdCurso = c.IdCurso,
            TituloCurso = c.TituloCurso,
            TotalInscritos = c.TotalInscritos,
            PorcentajeCompletados = c.PorcentajeCompletados
        }).ToList();
    }

    public async Task<IEnumerable<MejorEstudianteDto>> ObtenerMejoresEstudiantesAsync()
    {
        var respuesta = await _cliente.ObtenerMejoresEstudiantesAsync(new SolicitudVacia());
        return respuesta.Estudiantes.Select(e => new MejorEstudianteDto
        {
            IdEstudiante = e.IdEstudiante,
            NombreEstudiante = e.NombreEstudiante,
            PromedioCalificacion = e.PromedioCalificacion,
            TotalIntentos = e.TotalIntentos,
            CursosCompletados = e.CursosCompletados,
            Posicion = e.Posicion
        }).ToList();
    }
}
