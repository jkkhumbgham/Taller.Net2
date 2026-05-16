namespace Monolitica.Datos.Modelos;

public class Inscripcion
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrolledAt { get; set; }
    public double Progress { get; set; }

    public Usuario? Usuario { get; set; }
}
