using System.Runtime.Serialization;
using System.ServiceModel;

namespace Presentacion.Servicios.SoapContratos;

[ServiceContract(Namespace = "http://elearning/servicios/clases")]
public interface IClasesSoapCliente
{
    [OperationContract]
    Task<ListaClasesMasTomadas> ObtenerClasesMasTomadosAsync();

    [OperationContract]
    Task<ListaCursosAcabados> ObtenerCursosAcabadosAsync(int userId);
}

[DataContract(Namespace = "http://elearning/servicios/clases")]
public class ClaseMasTomadaSoap
{
    [DataMember] public long IdCurso { get; set; }
    [DataMember] public string TituloCurso { get; set; } = string.Empty;
    [DataMember] public int TotalInscritos { get; set; }
    [DataMember] public double PorcentajeCompletados { get; set; }
}

[DataContract(Namespace = "http://elearning/servicios/clases")]
public class ListaClasesMasTomadas
{
    [DataMember] public List<ClaseMasTomadaSoap> Clases { get; set; } = new();
}

[DataContract(Namespace = "http://elearning/servicios/clases")]
public class CursoAcabadoSoap
{
    [DataMember] public long IdCurso { get; set; }
    [DataMember] public string TituloCurso { get; set; } = string.Empty;
    [DataMember] public DateTime FechaInscripcion { get; set; }
    [DataMember] public DateTime? FechaCompletado { get; set; }
    [DataMember] public int TotalLecciones { get; set; }
    [DataMember] public double DuracionTotalSegundos { get; set; }
}

[DataContract(Namespace = "http://elearning/servicios/clases")]
public class ListaCursosAcabados
{
    [DataMember] public List<CursoAcabadoSoap> Cursos { get; set; } = new();
}
