namespace Estadisticas.Logica.DTOs;

public class ResumenEstudianteDto
{
    public int IdEstudiante { get; set; }
    public string NombreEstudiante { get; set; } = string.Empty;
    public string EmailEstudiante { get; set; } = string.Empty;
    public int TotalCursosInscritos { get; set; }
    public int CursosActivos { get; set; }
    public int CursosCompletados { get; set; }
    public int TotalTiempoInvertido { get; set; }
    public int TotalLeccionesCompletadas { get; set; }
    public double PromedioCalificacionCuestionarios { get; set; }
}
