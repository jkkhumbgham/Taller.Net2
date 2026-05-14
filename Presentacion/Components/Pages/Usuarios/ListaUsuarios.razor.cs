using Microsoft.AspNetCore.Components;
using Presentacion.Modelos.VistaModelos;
using Presentacion.Servicios.Interfaces;

namespace Presentacion.Components.Pages.Usuarios;

public partial class ListaUsuarios : ComponentBase
{
    [Inject] protected IServicioCrud ServicioCrud { get; set; } = default!;

    protected IEnumerable<UsuarioVistaModelo> Usuarios { get; set; } = Enumerable.Empty<UsuarioVistaModelo>();
    protected bool Cargando { get; set; } = true;
    protected string MensajeExito { get; set; } = string.Empty;
    protected string MensajeError { get; set; } = string.Empty;
    protected bool MostrarConfirmacion { get; set; }
    protected UsuarioVistaModelo? UsuarioAEliminar { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await CargarUsuarios();
    }

    private async Task CargarUsuarios()
    {
        try
        {
            Cargando = true;
            Usuarios = await ServicioCrud.ObtenerTodosUsuariosAsync();
        }
        catch
        {
            MensajeError = "Error al cargar los usuarios.";
        }
        finally
        {
            Cargando = false;
        }
    }

    protected void ConfirmarEliminar(UsuarioVistaModelo usuario)
    {
        UsuarioAEliminar = usuario;
        MostrarConfirmacion = true;
    }

    protected void CancelarEliminar()
    {
        UsuarioAEliminar = null;
        MostrarConfirmacion = false;
    }

    protected async Task EliminarUsuario()
    {
        if (UsuarioAEliminar == null) return;
        MostrarConfirmacion = false;
        try
        {
            var resultado = await ServicioCrud.EliminarUsuarioAsync(UsuarioAEliminar.Id);
            if (resultado)
            {
                MensajeExito = $"Usuario '{UsuarioAEliminar.Nombre}' eliminado correctamente.";
                await CargarUsuarios();
            }
            else
            {
                MensajeError = "No se pudo eliminar el usuario.";
            }
        }
        catch
        {
            MensajeError = "Error al eliminar el usuario.";
        }
        finally
        {
            UsuarioAEliminar = null;
        }
    }
}
