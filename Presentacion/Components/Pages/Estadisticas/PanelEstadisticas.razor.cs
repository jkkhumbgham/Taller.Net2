using Microsoft.AspNetCore.Components;
using Presentacion.Modelos.DTOs;
using Presentacion.Servicios.Interfaces;

namespace Presentacion.Components.Pages.Estadisticas;

public partial class PanelEstadisticas : ComponentBase, IDisposable
{
    [Inject] protected IServicioMonolitico ServicioMonolitico { get; set; } = default!;
    [Inject] protected IServicioArquitecturaServicios ServicioServicios { get; set; } = default!;
    [Inject] protected IServicioArquitecturaMicroServicios ServicioMicroServicios { get; set; } = default!;
    [Inject] protected IServicioSelectorArquitectura SelectorArquitectura { get; set; } = default!;

    protected TipoArquitectura ArquitecturaActual => SelectorArquitectura.ArquitecturaSeleccionada;
    protected int EstudianteId { get; set; }
    protected bool Cargando { get; set; }
    protected bool CargandoGlobal { get; set; }
    protected string MensajeError { get; set; } = string.Empty;
    protected string MensajeAdvertencia { get; set; } = string.Empty;

    protected ResumenEstudianteDto? Resumen { get; set; }
    protected IEnumerable<NotaEstudianteDto> Notas { get; set; } = Enumerable.Empty<NotaEstudianteDto>();
    protected IEnumerable<ClaseMasTomadaDto> ClasesMasTomadas { get; set; } = Enumerable.Empty<ClaseMasTomadaDto>();
    protected IEnumerable<MejorEstudianteDto> MejoresEstudiantes { get; set; } = Enumerable.Empty<MejorEstudianteDto>();

    protected override void OnInitialized()
    {
        SelectorArquitectura.OnCambio += OnArquitecturaCambio;
    }

    private void OnArquitecturaCambio()
    {
        LimpiarDatos();
        StateHasChanged();
    }

    protected void CambiarArquitectura(TipoArquitectura tipo)
    {
        SelectorArquitectura.ArquitecturaSeleccionada = tipo;
    }

    protected async Task BuscarEstadisticas()
    {
        if (EstudianteId <= 0)
        {
            MensajeAdvertencia = "Ingrese un ID de estudiante válido.";
            return;
        }

        Cargando = true;
        MensajeError = string.Empty;
        LimpiarDatosEstudiante();

        try
        {
            switch (ArquitecturaActual)
            {
                case TipoArquitectura.Monolitico:
                    Resumen = await ServicioMonolitico.ObtenerResumenEstudianteAsync(EstudianteId);
                    Notas = await ServicioMonolitico.ObtenerNotasEstudianteAsync(EstudianteId);
                    break;
                case TipoArquitectura.Servicios:
                    Resumen = await ServicioServicios.ObtenerResumenEstudianteAsync(EstudianteId);
                    break;
                case TipoArquitectura.MicroServicios:
                    Notas = await ServicioMicroServicios.ObtenerNotasEstudianteAsync(EstudianteId);
                    break;
            }

            if (Resumen == null && !Notas.Any())
            {
                MensajeAdvertencia = "No se encontraron datos para este estudiante en la arquitectura seleccionada.";
            }
        }
        catch
        {
            MensajeError = "Error al obtener las estadísticas. Verifique que el backend esté disponible.";
        }
        finally
        {
            Cargando = false;
        }
    }

    protected async Task CargarDatosGlobales()
    {
        CargandoGlobal = true;
        MensajeError = string.Empty;
        ClasesMasTomadas = Enumerable.Empty<ClaseMasTomadaDto>();
        MejoresEstudiantes = Enumerable.Empty<MejorEstudianteDto>();

        try
        {
            switch (ArquitecturaActual)
            {
                case TipoArquitectura.Monolitico:
                    ClasesMasTomadas = await ServicioMonolitico.ObtenerClasesMasTomadosAsync();
                    MejoresEstudiantes = await ServicioMonolitico.ObtenerMejoresEstudiantesAsync();
                    break;
                case TipoArquitectura.Servicios:
                    ClasesMasTomadas = await ServicioServicios.ObtenerClasesMasTomadosAsync();
                    break;
                case TipoArquitectura.MicroServicios:
                    ClasesMasTomadas = await ServicioMicroServicios.ObtenerClasesMasTomadosAsync();
                    MejoresEstudiantes = await ServicioMicroServicios.ObtenerMejoresEstudiantesAsync();
                    break;
            }
        }
        catch
        {
            MensajeError = "Error al cargar los datos globales. Verifique que el backend esté disponible.";
        }
        finally
        {
            CargandoGlobal = false;
        }
    }

    private void LimpiarDatosEstudiante()
    {
        Resumen = null;
        Notas = Enumerable.Empty<NotaEstudianteDto>();
    }

    private void LimpiarDatos()
    {
        LimpiarDatosEstudiante();
        ClasesMasTomadas = Enumerable.Empty<ClaseMasTomadaDto>();
        MejoresEstudiantes = Enumerable.Empty<MejorEstudianteDto>();
        MensajeError = string.Empty;
        MensajeAdvertencia = string.Empty;
    }

    public void Dispose()
    {
        SelectorArquitectura.OnCambio -= OnArquitecturaCambio;
    }
}
