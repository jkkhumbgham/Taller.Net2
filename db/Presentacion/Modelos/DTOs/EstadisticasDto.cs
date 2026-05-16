namespace Presentacion.Modelos.DTOs;

public class ResumenEstudianteDto
{
    public long IdEstudiante { get; set; }
    public string NombreEstudiante { get; set; } = string.Empty;
    public string EmailEstudiante { get; set; } = string.Empty;
    public int TotalCursosInscritos { get; set; }
    public int CursosActivos { get; set; }
    public int CursosCompletados { get; set; }
    public double TotalTiempoInvertido { get; set; }
    public int TotalLeccionesCompletadas { get; set; }
    public double PromedioCalificacionCuestionarios { get; set; }
}

public class EstadisticasCursoDto
{
    public long IdCurso { get; set; }
    public string TituloCurso { get; set; } = string.Empty;
    public double PorcentajeProgreso { get; set; }
    public int LeccionesCompletadas { get; set; }
    public int TotalLecciones { get; set; }
    public double TiempoInvertido { get; set; }
    public DateTime FechaInscripcion { get; set; }
    public string EstadoCurso { get; set; } = string.Empty;
}

public class EstadisticasCuestionarioDto
{
    public long IdCuestionario { get; set; }
    public string TituloCuestionario { get; set; } = string.Empty;
    public int TotalIntentos { get; set; }
    public double MejorCalificacion { get; set; }
    public double CalificacionPromedio { get; set; }
    public double CalificacionMaxima { get; set; }
    public bool Aprobado { get; set; }
}

public class NotaEstudianteDto
{
    public long IdIntento { get; set; }
    public string NombreCuestionario { get; set; } = string.Empty;
    public string TituloCuestionario { get; set; } = string.Empty;
    public double Calificacion { get; set; }
    public double CalificacionMaxima { get; set; }
    public double PorcentajeCalificacion { get; set; }
    public int NumeroIntento { get; set; }
    public DateTime FechaIntento { get; set; }
}

public class CursoAcabadoDto
{
    public long IdCurso { get; set; }
    public string TituloCurso { get; set; } = string.Empty;
    public DateTime FechaInscripcion { get; set; }
    public DateTime? FechaCompletado { get; set; }
    public int TotalLecciones { get; set; }
    public double DuracionTotalSegundos { get; set; }
}

public class ClaseMasTomadaDto
{
    public long IdCurso { get; set; }
    public string TituloCurso { get; set; } = string.Empty;
    public int TotalInscritos { get; set; }
    public double PorcentajeCompletados { get; set; }
}

public class MejorEstudianteDto
{
    public int Posicion { get; set; }
    public long IdEstudiante { get; set; }
    public string NombreEstudiante { get; set; } = string.Empty;
    public double PromedioCalificacion { get; set; }
    public int TotalIntentos { get; set; }
    public int CursosCompletados { get; set; }
}

public class PromedioEstudianteDto
{
    public long IdEstudiante { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public double PromedioCalificacion { get; set; }
    public int TotalIntentos { get; set; }
}
