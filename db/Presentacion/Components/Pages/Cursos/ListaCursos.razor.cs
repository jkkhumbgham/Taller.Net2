using Microsoft.AspNetCore.Components;
using Presentacion.Modelos.VistaModelos;
using Presentacion.Servicios.Interfaces;

namespace Presentacion.Components.Pages.Cursos;

public partial class ListaCursos : ComponentBase
{
    [Inject] protected IServicioCrud ServicioCrud { get; set; } = default!;

    protected IEnumerable<CursoVistaModelo> Cursos { get; set; } = Enumerable.Empty<CursoVistaModelo>();
    protected bool Cargando { get; set; } = true;
    protected string MensajeExito { get; set; } = string.Empty;
    protected string MensajeError { get; set; } = string.Empty;
    protected bool MostrarConfirmacion { get; set; }
    protected CursoVistaModelo? CursoAEliminar { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await CargarCursos();
    }

    private async Task CargarCursos()
    {
        try
        {
            Cargando = true;
            Cursos = await ServicioCrud.ObtenerTodosCursosAsync();
        }
        catch
        {
            MensajeError = "Error al cargar los cursos.";
        }
        finally
        {
            Cargando = false;
        }
    }

    protected void ConfirmarEliminar(CursoVistaModelo curso)
    {
        CursoAEliminar = curso;
        MostrarConfirmacion = true;
    }

    protected void CancelarEliminar()
    {
        CursoAEliminar = null;
        MostrarConfirmacion = false;
    }

    protected async Task EliminarCurso()
    {
        if (CursoAEliminar == null) return;
        MostrarConfirmacion = false;
        try
        {
            var resultado = await ServicioCrud.EliminarCursoAsync(CursoAEliminar.Id);
            if (resultado)
            {
                MensajeExito = $"Curso '{CursoAEliminar.Titulo}' eliminado correctamente.";
                await CargarCursos();
            }
            else
            {
                MensajeError = "No se pudo eliminar el curso.";
            }
        }
        catch
        {
            MensajeError = "Error al eliminar el curso.";
        }
        finally
        {
            CursoAEliminar = null;
        }
    }
}
