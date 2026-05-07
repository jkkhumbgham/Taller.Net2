namespace Estadisticas.Logica.DTOs;

public class EstadisticasCursoDto
{
    public int IdCurso { get; set; }
    public string TituloCurso { get; set; } = string.Empty;
    public double PorcentajeProgreso { get; set; }
    public int LeccionesCompletadas { get; set; }
    public int TotalLecciones { get; set; }
    public int TiempoInvertido { get; set; }
    public DateTime FechaInscripcion { get; set; }
    public string EstadoCurso { get; set; } = string.Empty;
}
