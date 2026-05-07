namespace Cuestionarios.Logica.DTOs;

public class NotaEstudianteDto
{
    public int IdIntento { get; set; }
    public int IdCuestionario { get; set; }
    public string TituloCuestionario { get; set; } = string.Empty;
    public double Calificacion { get; set; }
    public double CalificacionMaxima { get; set; }
    public double PorcentajeCalificacion { get; set; }
    public int NumeroIntento { get; set; }
    public DateTime FechaIntento { get; set; }
}
