namespace Notas.Logica.DTOs;

public class NotaEstudianteDto
{
    public int IdIntento { get; set; }
    public string NombreCuestionario { get; set; } = string.Empty;
    public double Calificacion { get; set; }
    public double CalificacionMaxima { get; set; }
    public double PorcentajeCalificacion { get; set; }
    public int NumeroIntento { get; set; }
    public DateTime FechaIntento { get; set; }
}

public class PromedioEstudianteDto
{
    public int IdEstudiante { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public double PromedioCalificacion { get; set; }
    public int TotalIntentos { get; set; }
}
