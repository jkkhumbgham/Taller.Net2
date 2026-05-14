using System.ServiceModel;
using Presentacion.Modelos.DTOs;
using Presentacion.Servicios.Interfaces;
using Presentacion.Servicios.SoapContratos;

namespace Presentacion.Servicios;

public class ServicioArquitecturaServicios : IServicioArquitecturaServicios
{
    private readonly string _estadisticasUrl;
    private readonly string _clasesUrl;

    public ServicioArquitecturaServicios(IConfiguration config)
    {
        _estadisticasUrl = (config["Arquitecturas:Servicios:EstadisticasUrl"] ?? "http://localhost:5022") + "/soap/estadisticas";
        _clasesUrl = (config["Arquitecturas:Servicios:ClasesUrl"] ?? "http://localhost:5021") + "/soap/clases";
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

    public async Task<ResumenEstudianteDto?> ObtenerResumenEstudianteAsync(int userId)
    {
        try
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
        catch { return null; }
    }

    public async Task<IEnumerable<EstadisticasCursoDto>> ObtenerEstadisticasCursosAsync(int userId)
    {
        try
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
        catch { return Enumerable.Empty<EstadisticasCursoDto>(); }
    }

    public async Task<IEnumerable<CursoAcabadoDto>> ObtenerCursosAcabadosAsync(int userId)
    {
        try
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
        catch { return Enumerable.Empty<CursoAcabadoDto>(); }
    }

    public async Task<IEnumerable<ClaseMasTomadaDto>> ObtenerClasesMasTomadosAsync()
    {
        try
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
        catch { return Enumerable.Empty<ClaseMasTomadaDto>(); }
    }
}
