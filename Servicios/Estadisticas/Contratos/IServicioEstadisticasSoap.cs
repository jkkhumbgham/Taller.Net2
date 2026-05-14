using System.ServiceModel;
using Estadisticas.Contratos;

namespace Estadisticas.Contratos;

[ServiceContract(Namespace = "http://elearning/servicios/estadisticas")]
public interface IServicioEstadisticasSoap
{
    [OperationContract]
    Task<ResumenEstudianteSoap> ObtenerResumenEstudianteAsync(int userId);

    [OperationContract]
    Task<ListaEstadisticasCursoSoap> ObtenerEstadisticasCursosAsync(int userId);
}
