using Microsoft.AspNetCore.Components;
using Presentacion.Modelos.VistaModelos;
using Presentacion.Servicios.Interfaces;

namespace Presentacion.Components.Pages.Usuarios;

public partial class FormularioUsuario : ComponentBase
{
    [Inject] protected IServicioCrud ServicioCrud { get; set; } = default!;
    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;

    [Parameter] public long? Id { get; set; }

    protected CrearUsuarioModelo Modelo { get; set; } = new();
    protected UsuarioVistaModelo? UsuarioExistente { get; set; }
    protected bool Cargando { get; set; } = true;
    protected bool Guardando { get; set; }
    protected string MensajeError { get; set; } = string.Empty;
    protected bool EsNuevo => Id == null;

    protected override async Task OnInitializedAsync()
    {
        if (!EsNuevo && Id.HasValue)
        {
            try
            {
                UsuarioExistente = await ServicioCrud.ObtenerUsuarioPorIdAsync(Id.Value);
                if (UsuarioExistente != null)
                {
                    Modelo = new CrearUsuarioModelo
                    {
                        Nombre = UsuarioExistente.Nombre,
                        Email = UsuarioExistente.Email,
                        Password = string.Empty
                    };
                }
                else
                {
                    NavigationManager.NavigateTo("/usuarios");
                }
            }
            catch
            {
                MensajeError = "Error al cargar el usuario.";
            }
        }
        Cargando = false;
    }

    protected async Task Guardar()
    {
        if (string.IsNullOrWhiteSpace(Modelo.Nombre) || string.IsNullOrWhiteSpace(Modelo.Email))
        {
            MensajeError = "El nombre y el email son obligatorios.";
            return;
        }

        Guardando = true;
        MensajeError = string.Empty;

        try
        {
            if (EsNuevo)
            {
                if (string.IsNullOrWhiteSpace(Modelo.Password))
                {
                    MensajeError = "La contraseña es obligatoria para usuarios nuevos.";
                    return;
                }
                await ServicioCrud.CrearUsuarioAsync(Modelo);
            }
            else if (UsuarioExistente != null)
            {
                var actualizado = new UsuarioVistaModelo
                {
                    Id = UsuarioExistente.Id,
                    Nombre = Modelo.Nombre,
                    Email = Modelo.Email,
                    Password = string.IsNullOrWhiteSpace(Modelo.Password) ? UsuarioExistente.Password : Modelo.Password,
                    CreadoEn = UsuarioExistente.CreadoEn
                };
                await ServicioCrud.ActualizarUsuarioAsync(actualizado);
            }
            NavigationManager.NavigateTo("/usuarios");
        }
        catch
        {
            MensajeError = "Error al guardar el usuario.";
        }
        finally
        {
            Guardando = false;
        }
    }
}
