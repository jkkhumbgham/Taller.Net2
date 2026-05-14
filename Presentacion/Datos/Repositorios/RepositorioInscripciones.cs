using Npgsql;
using Presentacion.Datos.Conexion;
using Presentacion.Datos.Repositorios.Interfaces;
using Presentacion.Modelos.VistaModelos;

namespace Presentacion.Datos.Repositorios;

public class RepositorioInscripciones : IRepositorioInscripciones
{
    private readonly IFabricaConexion _fabricaConexion;

    public RepositorioInscripciones(IFabricaConexion fabricaConexion)
    {
        _fabricaConexion = fabricaConexion;
    }

    public async Task<IEnumerable<InscripcionVistaModelo>> ObtenerTodasAsync()
    {
        var lista = new List<InscripcionVistaModelo>();
        await using var conn = _fabricaConexion.CreateUserDbConnection();
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            "SELECT id, user_id, course_id, enrolled_at, progress FROM enrollments ORDER BY id",
            conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lista.Add(MapearInscripcion(reader));
        }
        return lista;
    }

    public async Task<IEnumerable<InscripcionVistaModelo>> ObtenerPorUsuarioAsync(long userId)
    {
        var lista = new List<InscripcionVistaModelo>();
        await using var conn = _fabricaConexion.CreateUserDbConnection();
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            "SELECT id, user_id, course_id, enrolled_at, progress FROM enrollments WHERE user_id = @userId ORDER BY id",
            conn);
        cmd.Parameters.AddWithValue("@userId", userId);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lista.Add(MapearInscripcion(reader));
        }
        return lista;
    }

    public async Task<long> CrearAsync(long userId, long courseId)
    {
        await using var conn = _fabricaConexion.CreateUserDbConnection();
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            "INSERT INTO enrollments (user_id, course_id, enrolled_at, progress) VALUES (@userId, @courseId, NOW(), 0) RETURNING id",
            conn);
        cmd.Parameters.AddWithValue("@userId", userId);
        cmd.Parameters.AddWithValue("@courseId", courseId);
        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt64(result);
    }

    public async Task<bool> EliminarAsync(long id)
    {
        await using var conn = _fabricaConexion.CreateUserDbConnection();
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand("DELETE FROM enrollments WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        var rows = await cmd.ExecuteNonQueryAsync();
        return rows > 0;
    }

    private static InscripcionVistaModelo MapearInscripcion(NpgsqlDataReader reader)
    {
        return new InscripcionVistaModelo
        {
            Id = reader.GetInt64(0),
            UsuarioId = reader.GetInt64(1),
            CursoId = reader.GetInt64(2),
            InscritoEn = reader.GetDateTime(3),
            Progreso = reader.IsDBNull(4) ? 0.0 : reader.GetDouble(4)
        };
    }
}
