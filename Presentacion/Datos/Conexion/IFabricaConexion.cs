using Npgsql;

namespace Presentacion.Datos.Conexion;

public interface IFabricaConexion
{
    NpgsqlConnection CreateUserDbConnection();
    NpgsqlConnection CreateContentDbConnection();
}
