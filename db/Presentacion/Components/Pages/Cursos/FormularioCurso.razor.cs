using Microsoft.AspNetCore.Components;
using Presentacion.Modelos.VistaModelos;
using Presentacion.Servicios.Interfaces;

namespace Presentacion.Components.Pages.Cursos;

public partial class FormularioCurso : ComponentBase
{
    [Inject] protected IServicioCrud ServicioCrud { get; set; } = default!;
    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;

    [Parameter] public long? Id { get; set; }

    protected CrearCursoModelo Modelo { get; set; } = new();
    protected CursoVistaModelo? CursoExistente { get; set; }
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
                CursoExistente = await ServicioCrud.ObtenerCursoPorIdAsync(Id.Value);
                if (CursoExistente != null)
                {
                    Modelo = new CrearCursoModelo
                    {
                        Titulo = CursoExistente.Titulo,
                        Descripcion = CursoExistente.Descripcion,
                        Nivel = CursoExistente.Nivel,
                        Idioma = CursoExistente.Idioma,
                        EsPublicado = CursoExistente.EsPublicado
                    };
                }
                else
                {
                    NavigationManager.NavigateTo("/cursos");
                }
            }
            catch
            {
                MensajeError = "Error al cargar el curso.";
            }
        }
        Cargando = false;
    }

    protected async Task Guardar()
    {
        if (string.IsNullOrWhiteSpace(Modelo.Titulo))
        {
            MensajeError = "El título es obligatorio.";
            return;
        }

        Guardando = true;
        MensajeError = string.Empty;

        try
        {
            if (EsNuevo)
            {
                await ServicioCrud.CrearCursoAsync(Modelo);
            }
            else if (CursoExistente != null)
            {
                var actualizado = new CursoVistaModelo
                {
                    Id = CursoExistente.Id,
                    Titulo = Modelo.Titulo,
                    Descripcion = Modelo.Descripcion,
                    Nivel = Modelo.Nivel,
                    Idioma = Modelo.Idioma,
                    EsPublicado = Modelo.EsPublicado,
                    CreadoEn = CursoExistente.CreadoEn,
                    ActualizadoEn = DateTime.UtcNow
                };
                await ServicioCrud.ActualizarCursoAsync(actualizado);
            }
            NavigationManager.NavigateTo("/cursos");
        }
        catch
        {
            MensajeError = "Error al guardar el curso.";
        }
        finally
        {
            Guardando = false;
        }
    }
}
