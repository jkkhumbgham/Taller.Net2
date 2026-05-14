using System.Runtime.Serialization;

namespace Cuestionarios.Contratos;

[DataContract(Namespace = "http://elearning/servicios/cuestionarios")]
public class NotaEstudianteSoap
{
    [DataMember] public int IdIntento { get; set; }
    [DataMember] public int IdCuestionario { get; set; }
    [DataMember] public string TituloCuestionario { get; set; } = string.Empty;
    [DataMember] public double Calificacion { get; set; }
    [DataMember] public double CalificacionMaxima { get; set; }
    [DataMember] public double PorcentajeCalificacion { get; set; }
    [DataMember] public int NumeroIntento { get; set; }
    [DataMember] public DateTime FechaIntento { get; set; }
}

[DataContract(Namespace = "http://elearning/servicios/cuestionarios")]
public class ListaNotasEstudiante
{
    [DataMember] public bool Encontrado { get; set; }
    [DataMember] public List<NotaEstudianteSoap> Notas { get; set; } = new();
}

[DataContract(Namespace = "http://elearning/servicios/cuestionarios")]
public class MejorEstudianteSoap
{
    [DataMember] public int IdEstudiante { get; set; }
    [DataMember] public string NombreEstudiante { get; set; } = string.Empty;
    [DataMember] public double PromedioCalificacion { get; set; }
    [DataMember] public int TotalIntentos { get; set; }
    [DataMember] public int Posicion { get; set; }
}

[DataContract(Namespace = "http://elearning/servicios/cuestionarios")]
public class ListaMejoresEstudiantes
{
    [DataMember] public List<MejorEstudianteSoap> Estudiantes { get; set; } = new();
}
