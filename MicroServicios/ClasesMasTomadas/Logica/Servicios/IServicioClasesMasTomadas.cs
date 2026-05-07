using ClasesMasTomadas.Logica.DTOs;

namespace ClasesMasTomadas.Logica.Servicios;

public interface IServicioClasesMasTomadas
{
    Task<IEnumerable<ClaseMasTomadaDto>> ObtenerClasesMasTomadas();
    Task<IEnumerable<ClaseMasTomadaDto>> ObtenerTopClasesMasTomadas(int n);
}
