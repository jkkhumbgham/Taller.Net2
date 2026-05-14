using Presentacion.Modelos.VistaModelos;

namespace Presentacion.Datos.Repositorios.Interfaces;

public interface IRepositorioInscripciones
{
    Task<IEnumerable<InscripcionVistaModelo>> ObtenerTodasAsync();
    Task<IEnumerable<InscripcionVistaModelo>> ObtenerPorUsuarioAsync(long userId);
    Task<long> CrearAsync(long userId, long courseId);
    Task<bool> EliminarAsync(long id);
}
