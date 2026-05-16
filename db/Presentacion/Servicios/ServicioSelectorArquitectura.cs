using Presentacion.Servicios.Interfaces;

namespace Presentacion.Servicios;

public class ServicioSelectorArquitectura : IServicioSelectorArquitectura
{
    private TipoArquitectura _arquitecturaSeleccionada = TipoArquitectura.Monolitico;

    public TipoArquitectura ArquitecturaSeleccionada
    {
        get => _arquitecturaSeleccionada;
        set
        {
            _arquitecturaSeleccionada = value;
            OnCambio?.Invoke();
        }
    }

    public event Action? OnCambio;
}
