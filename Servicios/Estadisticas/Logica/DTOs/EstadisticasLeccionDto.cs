namespace Estadisticas.Logica.DTOs;

public class EstadisticasLeccionDto
{
    public int IdLeccion { get; set; }
    public string TituloLeccion { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public int TiempoInvertido { get; set; }
    public DateTime? FechaCompletado { get; set; }
    public double PorcentajeProgreso { get; set; }
}
