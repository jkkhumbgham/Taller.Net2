using System.ServiceModel;

namespace Cuestionarios.Contratos;

[ServiceContract(Namespace = "http://elearning/servicios/cuestionarios")]
public interface IServicioCuestionariosSoap
{
    [OperationContract]
    Task<ListaNotasEstudiante> ObtenerNotasEstudianteAsync(int userId);

    [OperationContract]
    Task<ListaMejoresEstudiantes> ObtenerMejoresEstudiantesAsync();
}
