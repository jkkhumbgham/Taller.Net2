using Microsoft.AspNetCore.Components;
using Presentacion.Modelos.VistaModelos;
using Presentacion.Servicios.Interfaces;

namespace Presentacion.Components.Pages.Inscripciones;

public partial class ListaInscripciones : ComponentBase
{
    [Inject] protected IServicioCrud ServicioCrud { get; set; } = default!;

    protected IEnumerable<InscripcionVistaModelo> Inscripciones { get; set; } = Enumerable.Empty<InscripcionVistaModelo>();
    protected bool Cargando { get; set; } = true;
    protected bool Guardando { get; set; }
    protected bool MostrarFormulario { get; set; }
    protected bool MostrarConfirmacion { get; set; }
    protected string MensajeExito { get; set; } = string.Empty;
    protected string MensajeError { get; set; } = string.Empty;
    protected InscripcionVistaModelo? InscripcionAEliminar { get; set; }
    protected long NuevoUsuarioId { get; set; }
    protected long NuevoCursoId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await CargarInscripciones();
    }

    private async Task CargarInscripciones()
    {
        try
        {
            Cargando = true;
            Inscripciones = await ServicioCrud.ObtenerTodasInscripcionesAsync();
        }
        catch
        {
            MensajeError = "Error al cargar las inscripciones.";
        }
        finally
        {
            Cargando = false;
        }
    }

    protected void MostrarFormularioNuevo()
    {
        NuevoUsuarioId = 0;
        NuevoCursoId = 0;
        MostrarFormulario = true;
    }

    protected void CancelarFormulario()
    {
        MostrarFormulario = false;
    }

    protected async Task CrearInscripcion()
    {
        if (NuevoUsuarioId <= 0 || NuevoCursoId <= 0)
        {
            MensajeError = "Ingrese IDs válidos para usuario y curso.";
            return;
        }
        Guardando = true;
        try
        {
            await ServicioCrud.CrearInscripcionAsync(NuevoUsuarioId, NuevoCursoId);
            MensajeExito = "Inscripción creada correctamente.";
            MostrarFormulario = false;
            await CargarInscripciones();
        }
        catch
        {
            MensajeError = "Error al crear la inscripción.";
        }
        finally
        {
            Guardando = false;
        }
    }

    protected void ConfirmarEliminar(InscripcionVistaModelo inscripcion)
    {
        InscripcionAEliminar = inscripcion;
        MostrarConfirmacion = true;
    }

    protected void CancelarEliminar()
    {
        InscripcionAEliminar = null;
        MostrarConfirmacion = false;
    }

    protected async Task EliminarInscripcion()
    {
        if (InscripcionAEliminar == null) return;
        MostrarConfirmacion = false;
        try
        {
            var resultado = await ServicioCrud.EliminarInscripcionAsync(InscripcionAEliminar.Id);
            if (resultado)
            {
                MensajeExito = $"Inscripción #{InscripcionAEliminar.Id} eliminada correctamente.";
                await CargarInscripciones();
            }
            else
            {
                MensajeError = "No se pudo eliminar la inscripción.";
            }
        }
        catch
        {
            MensajeError = "Error al eliminar la inscripción.";
        }
        finally
        {
            InscripcionAEliminar = null;
        }
    }

    protected static string BarraColor(double progreso) => progreso switch
    {
        >= 100 => "bg-success",
        >= 50 => "bg-info",
        _ => "bg-warning"
    };
}
