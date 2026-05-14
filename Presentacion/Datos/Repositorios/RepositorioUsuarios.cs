using Npgsql;
using Presentacion.Datos.Conexion;
using Presentacion.Datos.Repositorios.Interfaces;
using Presentacion.Modelos.VistaModelos;

namespace Presentacion.Datos.Repositorios;

public class RepositorioUsuarios : IRepositorioUsuarios
{
    private readonly IFabricaConexion _fabricaConexion;

    public RepositorioUsuarios(IFabricaConexion fabricaConexion)
    {
        _fabricaConexion = fabricaConexion;
    }

    public async Task<IEnumerable<UsuarioVistaModelo>> ObtenerTodosAsync()
    {
        var lista = new List<UsuarioVistaModelo>();
        await using var conn = _fabricaConexion.CreateUserDbConnection();
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            "SELECT id, name, email, password, created_at FROM users ORDER BY id", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lista.Add(MapearUsuario(reader));
        }
        return lista;
    }

    public async Task<UsuarioVistaModelo?> ObtenerPorIdAsync(long id)
    {
        await using var conn = _fabricaConexion.CreateUserDbConnection();
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            "SELECT id, name, email, password, created_at FROM users WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
            return MapearUsuario(reader);
        return null;
    }

    public async Task<long> CrearAsync(CrearUsuarioModelo modelo)
    {
        await using var conn = _fabricaConexion.CreateUserDbConnection();
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            "INSERT INTO users (name, email, password, created_at) VALUES (@name, @email, @password, NOW()) RETURNING id",
            conn);
        cmd.Parameters.AddWithValue("@name", modelo.Nombre);
        cmd.Parameters.AddWithValue("@email", modelo.Email);
        cmd.Parameters.AddWithValue("@password", modelo.Password);
        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt64(result);
    }

    public async Task<bool> ActualizarAsync(UsuarioVistaModelo modelo)
    {
        await using var conn = _fabricaConexion.CreateUserDbConnection();
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            "UPDATE users SET name = @name, email = @email, password = @password WHERE id = @id",
            conn);
        cmd.Parameters.AddWithValue("@name", modelo.Nombre);
        cmd.Parameters.AddWithValue("@email", modelo.Email);
        cmd.Parameters.AddWithValue("@password", modelo.Password);
        cmd.Parameters.AddWithValue("@id", modelo.Id);
        var rows = await cmd.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<bool> EliminarAsync(long id)
    {
        await using var conn = _fabricaConexion.CreateUserDbConnection();
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand("DELETE FROM users WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        var rows = await cmd.ExecuteNonQueryAsync();
        return rows > 0;
    }

    private static UsuarioVistaModelo MapearUsuario(NpgsqlDataReader reader)
    {
        return new UsuarioVistaModelo
        {
            Id = reader.GetInt64(0),
            Nombre = reader.GetString(1),
            Email = reader.GetString(2),
            Password = reader.GetString(3),
            CreadoEn = reader.GetDateTime(4)
        };
    }
}
