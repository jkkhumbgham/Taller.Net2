namespace Presentacion.Modelos.VistaModelos;

public class UsuarioVistaModelo
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime CreadoEn { get; set; }
}

public class CrearUsuarioModelo
{
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
