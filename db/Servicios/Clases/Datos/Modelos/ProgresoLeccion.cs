namespace Clases.Datos.Modelos;

public class ProgresoLeccion
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int LessonId { get; set; }
    public string Status { get; set; } = string.Empty;
    public double ProgressPercent { get; set; }
    public int TimeSpent { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
