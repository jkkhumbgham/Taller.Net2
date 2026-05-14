using System.ServiceModel;
using Presentacion.Modelos.DTOs;
using Presentacion.Servicios.Interfaces;
using Presentacion.Servicios.SoapContratos;

namespace Presentacion.Servicios;

public class ServicioArquitecturaServicios : IServicioArquitecturaServicios
{
    private readonly string _estadisticasUrl;
    private readonly string _clasesUrl;
    private readonly string _cuestionariosUrl;

    public ServicioArquitecturaServicios(IConfiguration config)
    {
        _estadisticasUrl = (config["Arquitecturas:Servicios:EstadisticasUrl"] ?? "http://localhost:5022") + "/soap/estadisticas";
        _clasesUrl = (config["Arquitecturas:Servicios:ClasesUrl"] ?? "http://localhost:5021") + "/soap/clases";
        _cuestionariosUrl = (config["Arquitecturas:Servicios:CuestionariosUrl"] ?? "http://localhost:5023") + "/soap/cuestionarios";
    }

    private IEstadisticasSoapCliente CrearClienteEstadisticas()
    {
        var binding = new BasicHttpBinding();
        var endpoint = new EndpointAddress(_estadisticasUrl);
        var factory = new ChannelFactory<IEstadisticasSoapCliente>(binding, endpoint);
        return factory.CreateChannel();
    }

    private IClasesSoapCliente CrearClienteClases()
    {
        var binding = new BasicHttpBinding();
        var endpoint = new EndpointAddress(_clasesUrl);
        var factory = new ChannelFactory<IClasesSoapCliente>(binding, endpoint);
        return factory.CreateChannel();
    }

    private ICuestionariosSoapCliente CrearClienteCuestionarios()
    {
        var binding = new BasicHttpBinding();
        var endpoint = new EndpointAddress(_cuestionariosUrl);
        var factory = new ChannelFactory<ICuestionariosSoapCliente>(binding, endpoint);
        return factory.CreateChannel();
    }

    public async Task<ResumenEstudianteDto?> ObtenerResumenEstudianteAsync(int userId)
    {
        var cliente = CrearClienteEstadisticas();
        var respuesta = await cliente.ObtenerResumenEstudianteAsync(userId);
        if (!respuesta.Encontrado) return null;
        return new ResumenEstudianteDto
        {
            IdEstudiante = respuesta.IdEstudiante,
            NombreEstudiante = respuesta.NombreEstudiante,
            EmailEstudiante = respuesta.EmailEstudiante,
            TotalCursosInscritos = respuesta.TotalCursosInscritos,
            CursosActivos = respuesta.CursosActivos,
            CursosCompletados = respuesta.CursosCompletados,
            TotalTiempoInvertido = respuesta.TotalTiempoInvertido,
            TotalLeccionesCompletadas = respuesta.TotalLeccionesCompletadas,
            PromedioCalificacionCuestionarios = respuesta.PromedioCalificacionCuestionarios
        };
    }

    public async Task<IEnumerable<EstadisticasCursoDto>> ObtenerEstadisticasCursosAsync(int userId)
    {
        var cliente = CrearClienteEstadisticas();
        var respuesta = await cliente.ObtenerEstadisticasCursosAsync(userId);
        return respuesta.Cursos.Select(c => new EstadisticasCursoDto
        {
            IdCurso = c.IdCurso,
            TituloCurso = c.TituloCurso,
            PorcentajeProgreso = c.PorcentajeProgreso,
            LeccionesCompletadas = c.LeccionesCompletadas,
            TotalLecciones = c.TotalLecciones,
            TiempoInvertido = c.TiempoInvertido,
            FechaInscripcion = c.FechaInscripcion,
            EstadoCurso = c.EstadoCurso
        }).ToList();
    }

    public async Task<IEnumerable<CursoAcabadoDto>> ObtenerCursosAcabadosAsync(int userId)
    {
        var cliente = CrearClienteClases();
        var respuesta = await cliente.ObtenerCursosAcabadosAsync(userId);
        return respuesta.Cursos.Select(c => new CursoAcabadoDto
        {
            IdCurso = c.IdCurso,
            TituloCurso = c.TituloCurso,
            FechaInscripcion = c.FechaInscripcion,
            FechaCompletado = c.FechaCompletado,
            TotalLecciones = c.TotalLecciones,
            DuracionTotalSegundos = c.DuracionTotalSegundos
        }).ToList();
    }

    public async Task<IEnumerable<ClaseMasTomadaDto>> ObtenerClasesMasTomadosAsync()
    {
        var cliente = CrearClienteClases();
        var respuesta = await cliente.ObtenerClasesMasTomadosAsync();
        return respuesta.Clases.Select(c => new ClaseMasTomadaDto
        {
            IdCurso = c.IdCurso,
            TituloCurso = c.TituloCurso,
            TotalInscritos = c.TotalInscritos,
            PorcentajeCompletados = c.PorcentajeCompletados
        }).ToList();
    }

    public async Task<IEnumerable<NotaEstudianteDto>> ObtenerNotasEstudianteAsync(int userId)
    {
        var cliente = CrearClienteCuestionarios();
        var respuesta = await cliente.ObtenerNotasEstudianteAsync(userId);
        if (!respuesta.Encontrado) return Enumerable.Empty<NotaEstudianteDto>();
        return respuesta.Notas.Select(n => new NotaEstudianteDto
        {
            IdIntento = n.IdIntento,
            TituloCuestionario = n.TituloCuestionario,
            NombreCuestionario = n.TituloCuestionario,
            Calificacion = n.Calificacion,
            CalificacionMaxima = n.CalificacionMaxima,
            PorcentajeCalificacion = n.PorcentajeCalificacion,
            NumeroIntento = n.NumeroIntento,
            FechaIntento = n.FechaIntento
        }).ToList();
    }

    public async Task<IEnumerable<MejorEstudianteDto>> ObtenerMejoresEstudiantesAsync()
    {
        var cliente = CrearClienteCuestionarios();
        var respuesta = await cliente.ObtenerMejoresEstudiantesAsync();
        return respuesta.Estudiantes.Select(e => new MejorEstudianteDto
        {
            Posicion = e.Posicion,
            IdEstudiante = e.IdEstudiante,
            NombreEstudiante = e.NombreEstudiante,
            PromedioCalificacion = e.PromedioCalificacion,
            TotalIntentos = e.TotalIntentos
        }).ToList();
    }
}
