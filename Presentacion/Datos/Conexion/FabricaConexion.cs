using Npgsql;

namespace Presentacion.Datos.Conexion;

public class FabricaConexion : IFabricaConexion
{
    private readonly IConfiguration _configuration;

    public FabricaConexion(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public NpgsqlConnection CreateUserDbConnection()
    {
        var connectionString = _configuration.GetConnectionString("UserDb")
            ?? throw new InvalidOperationException("Connection string 'UserDb' not found.");
        return new NpgsqlConnection(connectionString);
    }

    public NpgsqlConnection CreateContentDbConnection()
    {
        var connectionString = _configuration.GetConnectionString("ContentDb")
            ?? throw new InvalidOperationException("Connection string 'ContentDb' not found.");
        return new NpgsqlConnection(connectionString);
    }
}
