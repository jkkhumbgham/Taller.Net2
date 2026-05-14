namespace Presentacion.Servicios.Interfaces;

public enum TipoArquitectura
{
    Monolitico,
    Servicios,
    MicroServicios
}

public interface IServicioSelectorArquitectura
{
    TipoArquitectura ArquitecturaSeleccionada { get; set; }
    event Action? OnCambio;
}
