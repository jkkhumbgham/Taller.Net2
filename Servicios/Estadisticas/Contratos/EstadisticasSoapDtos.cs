using System.Runtime.Serialization;

namespace Estadisticas.Contratos;

[DataContract(Namespace = "http://elearning/servicios/estadisticas")]
public class ResumenEstudianteSoap
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
public class EstadisticasCursoSoap
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
public class ListaEstadisticasCursoSoap
{
    [DataMember] public List<EstadisticasCursoSoap> Cursos { get; set; } = new();
}
