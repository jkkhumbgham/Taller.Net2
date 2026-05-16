using System.Runtime.Serialization;
using System.ServiceModel;

namespace Presentacion.Servicios.SoapContratos;

[ServiceContract(Namespace = "http://elearning/servicios/estadisticas")]
public interface IEstadisticasSoapCliente
{
    [OperationContract]
    Task<ResumenEstudianteSoapResp> ObtenerResumenEstudianteAsync(int userId);

    [OperationContract]
    Task<ListaEstadisticasCursoSoapResp> ObtenerEstadisticasCursosAsync(int userId);
}

[DataContract(Namespace = "http://elearning/servicios/estadisticas")]
public class ResumenEstudianteSoapResp
{
    [DataMember] public bool Encontrado { get; set; }
    [DataMember] public long IdEstudiante { get; set; }
    [DataMember] public string NombreEstudiante { get; set; } = string.Empty;
    [DataMember] public string EmailEstudiante { get; set; } = string.Empty;
    [DataMember] public int TotalCursosInscritos { get; set; }
    [DataMember] public int CursosActivos { get; set; }
    [DataMember] public int CursosCompletados { get; set; }
    [DataMember] public double TotalTiempoInvertido { get; set; }
    [DataMember] public int TotalLeccionesCompletadas { get; set; }
    [DataMember] public double PromedioCalificacionCuestionarios { get; set; }
}

[DataContract(Namespace = "http://elearning/servicios/estadisticas")]
public class EstadisticasCursoSoapResp
{
    [DataMember] public long IdCurso { get; set; }
    [DataMember] public string TituloCurso { get; set; } = string.Empty;
    [DataMember] public double PorcentajeProgreso { get; set; }
    [DataMember] public int LeccionesCompletadas { get; set; }
    [DataMember] public int TotalLecciones { get; set; }
    [DataMember] public double TiempoInvertido { get; set; }
    [DataMember] public DateTime FechaInscripcion { get; set; }
    [DataMember] public string EstadoCurso { get; set; } = string.Empty;
}

[DataContract(Namespace = "http://elearning/servicios/estadisticas")]
public class ListaEstadisticasCursoSoapResp
{
    [DataMember] public List<EstadisticasCursoSoapResp> Cursos { get; set; } = new();
}
