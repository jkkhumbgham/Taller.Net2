namespace CursosAcabados.Datos.Modelos;

public class Modulo
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Position { get; set; }
    public DateTime CreatedAt { get; set; }

    public Curso? Curso { get; set; }
    public ICollection<Leccion> Lecciones { get; set; } = new List<Leccion>();
}
