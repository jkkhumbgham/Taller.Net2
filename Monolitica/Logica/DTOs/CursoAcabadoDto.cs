namespace Monolitica.Logica.DTOs;

public class CursoAcabadoDto
{
    public int IdCurso { get; set; }
    public string TituloCurso { get; set; } = string.Empty;
    public DateTime FechaInscripcion { get; set; }
    public DateTime? FechaCompletado { get; set; }
    public int TotalLecciones { get; set; }
    public int DuracionTotalSegundos { get; set; }
}
