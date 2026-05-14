using System.ServiceModel;
using Clases.Contratos;

namespace Clases.Contratos;

[ServiceContract(Namespace = "http://elearning/servicios/clases")]
public interface IServicioClasesSoap
{
    [OperationContract]
    Task<ListaClasesMasTomadas> ObtenerClasesMasTomadosAsync();

    [OperationContract]
    Task<ListaCursosAcabados> ObtenerCursosAcabadosAsync(int userId);
}
