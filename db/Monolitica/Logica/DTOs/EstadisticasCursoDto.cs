namespace Monolitica.Logica.DTOs;

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

public class EstadisticasDetalleCursoDto : EstadisticasCursoDto
{
    public IEnumerable<EstadisticasLeccionDto> Lecciones { get; set; } = new List<EstadisticasLeccionDto>();
    public IEnumerable<EstadisticasCuestionarioDto> Cuestionarios { get; set; } = new List<EstadisticasCuestionarioDto>();
}

public class EstadisticasLeccionDto
{
    public int IdLeccion { get; set; }
    public string TituloLeccion { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public int TiempoInvertido { get; set; }
    public DateTime? FechaCompletado { get; set; }
    public double PorcentajeProgreso { get; set; }
}
