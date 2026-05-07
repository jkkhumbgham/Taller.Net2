namespace Monolitica.Logica.DTOs;

public class MejorEstudianteDto
{
    public int IdEstudiante { get; set; }
    public string NombreEstudiante { get; set; } = string.Empty;
    public double PromedioCalificacion { get; set; }
    public int TotalIntentos { get; set; }
    public int CursosCompletados { get; set; }
}
