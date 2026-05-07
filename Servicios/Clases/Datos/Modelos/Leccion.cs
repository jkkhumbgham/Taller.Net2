namespace Clases.Datos.Modelos;

public class Leccion
{
    public int Id { get; set; }
    public int ModuleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int? Duration { get; set; }
    public DateTime CreatedAt { get; set; }

    public Modulo? Modulo { get; set; }
}
