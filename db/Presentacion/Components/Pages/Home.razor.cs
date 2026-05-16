using Microsoft.AspNetCore.Components;
using Presentacion.Servicios.Interfaces;

namespace Presentacion.Components.Pages;

public partial class Home : ComponentBase, IDisposable
{
    [Inject] protected IServicioCrud ServicioCrud { get; set; } = default!;
    [Inject] protected IServicioSelectorArquitectura SelectorArquitectura { get; set; } = default!;

    protected bool Cargando { get; set; } = true;
    protected int TotalUsuarios { get; set; }
    protected int TotalCursos { get; set; }
    protected int TotalInscripciones { get; set; }

    protected string NombreArquitectura => SelectorArquitectura.ArquitecturaSeleccionada switch
    {
        TipoArquitectura.Monolitico => "Monolítico",
        TipoArquitectura.Servicios => "Servicios",
        TipoArquitectura.MicroServicios => "Microservicios",
        _ => "Monolítico"
    };

    protected override async Task OnInitializedAsync()
    {
        SelectorArquitectura.OnCambio += StateHasChanged;
        try
        {
            TotalUsuarios = await ServicioCrud.ContarUsuariosAsync();
            TotalCursos = await ServicioCrud.ContarCursosAsync();
            TotalInscripciones = await ServicioCrud.ContarInscripcionesAsync();
        }
        finally
        {
            Cargando = false;
        }
    }

    public void Dispose()
    {
        SelectorArquitectura.OnCambio -= StateHasChanged;
    }
}
