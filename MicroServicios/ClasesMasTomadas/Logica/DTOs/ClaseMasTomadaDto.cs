namespace ClasesMasTomadas.Logica.DTOs;

public class ClaseMasTomadaDto
{
    public int IdCurso { get; set; }
    public string TituloCurso { get; set; } = string.Empty;
    public int TotalInscritos { get; set; }
    public double PorcentajeCompletados { get; set; }
}
