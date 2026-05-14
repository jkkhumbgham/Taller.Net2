using Microsoft.AspNetCore.Components;
using Presentacion.Datos.Repositorios.Interfaces;
using Presentacion.Servicios.Interfaces;

namespace Presentacion.Components.Pages;

public partial class Home : ComponentBase, IDisposable
{
    [Inject] protected IRepositorioUsuarios RepositorioUsuarios { get; set; } = default!;
    [Inject] protected IRepositorioCursos RepositorioCursos { get; set; } = default!;
    [Inject] protected IRepositorioInscripciones RepositorioInscripciones { get; set; } = default!;
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
            var usuarios = await RepositorioUsuarios.ObtenerTodosAsync();
            TotalUsuarios = usuarios.Count();
            var cursos = await RepositorioCursos.ObtenerTodosAsync();
            TotalCursos = cursos.Count();
            var inscripciones = await RepositorioInscripciones.ObtenerTodasAsync();
            TotalInscripciones = inscripciones.Count();
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
