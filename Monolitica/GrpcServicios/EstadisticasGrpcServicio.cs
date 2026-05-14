using Grpc.Core;
using Monolitica.Grpc;
using Monolitica.Logica.Servicios;

namespace Monolitica.GrpcServicios;

public class EstadisticasGrpcServicio : EstadisticasGrpc.EstadisticasGrpcBase
{
    private readonly IServicioEstadisticas _servicio;

    public EstadisticasGrpcServicio(IServicioEstadisticas servicio)
    {
        _servicio = servicio;
    }

    public override async Task<RespuestaResumen> ObtenerResumenEstudiante(
        SolicitudUsuario request, ServerCallContext context)
    {
        var resultado = await _servicio.ObtenerResumenEstudianteAsync(request.UserId);
        if (resultado == null)
            return new RespuestaResumen { Encontrado = false };

        return new RespuestaResumen
        {
            Encontrado = true,
            Resumen = new MensajeResumenEstudiante
            {
                IdEstudiante = resultado.IdEstudiante,
                NombreEstudiante = resultado.NombreEstudiante,
                EmailEstudiante = resultado.EmailEstudiante,
                TotalCursosInscritos = resultado.TotalCursosInscritos,
                CursosActivos = resultado.CursosActivos,
                CursosCompletados = resultado.CursosCompletados,
                TotalTiempoInvertido = resultado.TotalTiempoInvertido,
                TotalLeccionesCompletadas = resultado.TotalLeccionesCompletadas,
                PromedioCalificacionCuestionarios = resultado.PromedioCalificacionCuestionarios
            }
        };
    }

    public override async Task<RespuestaNotas> ObtenerNotasEstudiante(
        SolicitudUsuario request, ServerCallContext context)
    {
        var resultado = await _servicio.ObtenerNotasEstudianteAsync(request.UserId);
        var respuesta = new RespuestaNotas();
        if (resultado != null)
        {
            foreach (var nota in resultado)
            {
                respuesta.Notas.Add(new MensajNota
                {
                    IdIntento = nota.IdIntento,
                    IdCuestionario = nota.IdCuestionario,
                    TituloCuestionario = nota.TituloCuestionario,
                    Calificacion = nota.Calificacion,
                    CalificacionMaxima = nota.CalificacionMaxima,
                    PorcentajeCalificacion = nota.PorcentajeCalificacion,
                    NumeroIntento = nota.NumeroIntento,
                    FechaIntento = nota.FechaIntento.ToString("O")
                });
            }
        }
        return respuesta;
    }

    public override async Task<RespuestaCursosAcabados> ObtenerCursosAcabados(
        SolicitudUsuario request, ServerCallContext context)
    {
        var resultado = await _servicio.ObtenerCursosAcabadosEstudianteAsync(request.UserId);
        var respuesta = new RespuestaCursosAcabados();
        if (resultado != null)
        {
            foreach (var curso in resultado)
            {
                respuesta.Cursos.Add(new MensajeCursoAcabado
                {
                    IdCurso = curso.IdCurso,
                    TituloCurso = curso.TituloCurso,
                    FechaInscripcion = curso.FechaInscripcion.ToString("O"),
                    FechaCompletado = curso.FechaCompletado?.ToString("O") ?? string.Empty,
                    TieneFechaCompletado = curso.FechaCompletado.HasValue,
                    TotalLecciones = curso.TotalLecciones,
                    DuracionTotalSegundos = curso.DuracionTotalSegundos
                });
            }
        }
        return respuesta;
    }

    public override async Task<RespuestaClasesMasTomadas> ObtenerClasesMasTomadas(
        SolicitudVacia request, ServerCallContext context)
    {
        var resultado = await _servicio.ObtenerClasesMasTomadas();
        var respuesta = new RespuestaClasesMasTomadas();
        foreach (var clase in resultado)
        {
            respuesta.Clases.Add(new MensajeClaseMasTomada
            {
                IdCurso = clase.IdCurso,
                TituloCurso = clase.TituloCurso,
                TotalInscritos = clase.TotalInscritos,
                PorcentajeCompletados = clase.PorcentajeCompletados
            });
        }
        return respuesta;
    }

    public override async Task<RespuestaMejoresEstudiantes> ObtenerMejoresEstudiantes(
        SolicitudVacia request, ServerCallContext context)
    {
        var resultado = await _servicio.ObtenerMejoresEstudiantes();
        var respuesta = new RespuestaMejoresEstudiantes();
        int pos = 1;
        foreach (var est in resultado)
        {
            respuesta.Estudiantes.Add(new MensajeMejorEstudiante
            {
                IdEstudiante = est.IdEstudiante,
                NombreEstudiante = est.NombreEstudiante,
                PromedioCalificacion = est.PromedioCalificacion,
                TotalIntentos = est.TotalIntentos,
                CursosCompletados = est.CursosCompletados,
                Posicion = pos++
            });
        }
        return respuesta;
    }
}
