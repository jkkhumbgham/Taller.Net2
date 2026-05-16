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
    protected IEnumerable<EstadisticasCursoDto> EstadisticasCursos { get; set; } = Enumerable.Empty<EstadisticasCursoDto>();
    protected IEnumerable<CursoAcabadoDto> CursosAcabados { get; set; } = Enumerable.Empty<CursoAcabadoDto>();
    protected IEnumerable<NotaEstudianteDto> Notas { get; set; } = Enumerable.Empty<NotaEstudianteDto>();
    protected IEnumerable<ClaseMasTomadaDto> ClasesMasTomadas { get; set; } = Enumerable.Empty<ClaseMasTomadaDto>();
    protected IEnumerable<MejorEstudianteDto> MejoresEstudiantes { get; set; } = Enumerable.Empty<MejorEstudianteDto>();

    protected string NombreArquitectura => ArquitecturaActual switch
    {
        TipoArquitectura.Monolitico    => "Monolítico",
        TipoArquitectura.Servicios     => "Servicios",
        TipoArquitectura.MicroServicios => "Microservicios",
        _                              => "Monolítico"
    };

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
                    Resumen           = await ServicioMonolitico.ObtenerResumenEstudianteAsync(EstudianteId);
                    Notas             = await ServicioMonolitico.ObtenerNotasEstudianteAsync(EstudianteId);
                    EstadisticasCursos = await ServicioMonolitico.ObtenerEstadisticasCursosAsync(EstudianteId);
                    CursosAcabados    = await ServicioMonolitico.ObtenerCursosAcabadosAsync(EstudianteId);
                    break;
                case TipoArquitectura.Servicios:
                    Resumen           = await ServicioServicios.ObtenerResumenEstudianteAsync(EstudianteId);
                    Notas             = await ServicioServicios.ObtenerNotasEstudianteAsync(EstudianteId);
                    EstadisticasCursos = await ServicioServicios.ObtenerEstadisticasCursosAsync(EstudianteId);
                    CursosAcabados    = await ServicioServicios.ObtenerCursosAcabadosAsync(EstudianteId);
                    break;
                case TipoArquitectura.MicroServicios:
                    Resumen        = await ServicioMicroServicios.ObtenerResumenEstudianteAsync(EstudianteId);
                    Notas          = await ServicioMicroServicios.ObtenerNotasEstudianteAsync(EstudianteId);
                    CursosAcabados = await ServicioMicroServicios.ObtenerCursosAcabadosAsync(EstudianteId);
                    // EstadisticasCursos no disponible en microservicios
                    break;
            }

            if (Resumen == null && !Notas.Any() && !CursosAcabados.Any())
                MensajeAdvertencia = "No se encontraron datos para este estudiante.";
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
                    ClasesMasTomadas   = await ServicioMonolitico.ObtenerClasesMasTomadosAsync();
                    MejoresEstudiantes = await ServicioMonolitico.ObtenerMejoresEstudiantesAsync();
                    break;
                case TipoArquitectura.Servicios:
                    ClasesMasTomadas   = await ServicioServicios.ObtenerClasesMasTomadosAsync();
                    MejoresEstudiantes = await ServicioServicios.ObtenerMejoresEstudiantesAsync();
                    break;
                case TipoArquitectura.MicroServicios:
                    ClasesMasTomadas   = await ServicioMicroServicios.ObtenerClasesMasTomadosAsync();
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

    // Helpers de formato
    protected static string FormatearTiempo(double segundos)
    {
        var ts = TimeSpan.FromSeconds(segundos);
        if (ts.TotalHours >= 1)
            return $"{(int)ts.TotalHours}h {ts.Minutes}m";
        return $"{ts.Minutes}m";
    }

    protected static string EstadoBadge(string estado) => estado.ToLower() switch
    {
        "completado" or "completed" => "bg-success",
        "en progreso" or "active"   => "bg-primary",
        _                           => "bg-secondary"
    };

    private void LimpiarDatosEstudiante()
    {
        Resumen = null;
        Notas = Enumerable.Empty<NotaEstudianteDto>();
        EstadisticasCursos = Enumerable.Empty<EstadisticasCursoDto>();
        CursosAcabados = Enumerable.Empty<CursoAcabadoDto>();
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
