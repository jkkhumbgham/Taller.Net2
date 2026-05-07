namespace CursosAcabados.Logica.DTOs;

public class CursoAcabadoDto
{
    public int IdCurso { get; set; }
    public string TituloCurso { get; set; } = string.Empty;
    public DateTime FechaInscripcion { get; set; }
    public int TotalLecciones { get; set; }
    public int TiempoTotalSegundos { get; set; }
}

public class TotalCursosAcabadosDto
{
    public int IdEstudiante { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int TotalCursosAcabados { get; set; }
}
