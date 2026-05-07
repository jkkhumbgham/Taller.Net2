namespace Monolitica.Datos.Modelos;

public class Curso
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Level { get; set; }
    public string? Language { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Modulo> Modulos { get; set; } = new List<Modulo>();
}
