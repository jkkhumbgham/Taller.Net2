namespace Notas.Datos.Modelos;

public class Usuario
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public ICollection<IntentoCuestionario> IntentosCuestionarios { get; set; } = new List<IntentoCuestionario>();
}
