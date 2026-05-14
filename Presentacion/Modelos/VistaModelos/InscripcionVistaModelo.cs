namespace Presentacion.Modelos.VistaModelos;

public class InscripcionVistaModelo
{
    public long Id { get; set; }
    public long UsuarioId { get; set; }
    public long CursoId { get; set; }
    public DateTime InscritoEn { get; set; }
    public double Progreso { get; set; }
}
