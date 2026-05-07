namespace Monolitica.Logica.DTOs;

public class EstadisticasCuestionarioDto
{
    public int IdCuestionario { get; set; }
    public string TituloCuestionario { get; set; } = string.Empty;
    public int TotalIntentos { get; set; }
    public double MejorCalificacion { get; set; }
    public double CalificacionPromedio { get; set; }
    public double CalificacionMaxima { get; set; }
    public bool Aprobado { get; set; }
}
