using Npgsql;
using Presentacion.Datos.Conexion;
using Presentacion.Datos.Repositorios.Interfaces;
using Presentacion.Modelos.VistaModelos;

namespace Presentacion.Datos.Repositorios;

public class RepositorioCursos : IRepositorioCursos
{
    private readonly IFabricaConexion _fabricaConexion;

    public RepositorioCursos(IFabricaConexion fabricaConexion)
    {
        _fabricaConexion = fabricaConexion;
    }

    public async Task<IEnumerable<CursoVistaModelo>> ObtenerTodosAsync()
    {
        var lista = new List<CursoVistaModelo>();
        await using var conn = _fabricaConexion.CreateContentDbConnection();
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            "SELECT id, title, description, level, language, is_published, created_at, updated_at FROM courses ORDER BY id",
            conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lista.Add(MapearCurso(reader));
        }
        return lista;
    }

    public async Task<CursoVistaModelo?> ObtenerPorIdAsync(long id)
    {
        await using var conn = _fabricaConexion.CreateContentDbConnection();
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            "SELECT id, title, description, level, language, is_published, created_at, updated_at FROM courses WHERE id = @id",
            conn);
        cmd.Parameters.AddWithValue("@id", id);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
            return MapearCurso(reader);
        return null;
    }

    public async Task<long> CrearAsync(CrearCursoModelo modelo)
    {
        await using var conn = _fabricaConexion.CreateContentDbConnection();
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            "INSERT INTO courses (title, description, level, language, is_published, created_at, updated_at) VALUES (@title, @desc, @level, @lang, @published, NOW(), NOW()) RETURNING id",
            conn);
        cmd.Parameters.AddWithValue("@title", modelo.Titulo);
        cmd.Parameters.AddWithValue("@desc", modelo.Descripcion);
        cmd.Parameters.AddWithValue("@level", modelo.Nivel);
        cmd.Parameters.AddWithValue("@lang", modelo.Idioma);
        cmd.Parameters.AddWithValue("@published", modelo.EsPublicado);
        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt64(result);
    }

    public async Task<bool> ActualizarAsync(CursoVistaModelo modelo)
    {
        await using var conn = _fabricaConexion.CreateContentDbConnection();
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            "UPDATE courses SET title = @title, description = @desc, level = @level, language = @lang, is_published = @published, updated_at = NOW() WHERE id = @id",
            conn);
        cmd.Parameters.AddWithValue("@title", modelo.Titulo);
        cmd.Parameters.AddWithValue("@desc", modelo.Descripcion);
        cmd.Parameters.AddWithValue("@level", modelo.Nivel);
        cmd.Parameters.AddWithValue("@lang", modelo.Idioma);
        cmd.Parameters.AddWithValue("@published", modelo.EsPublicado);
        cmd.Parameters.AddWithValue("@id", modelo.Id);
        var rows = await cmd.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<bool> EliminarAsync(long id)
    {
        await using var conn = _fabricaConexion.CreateContentDbConnection();
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand("DELETE FROM courses WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        var rows = await cmd.ExecuteNonQueryAsync();
        return rows > 0;
    }

    private static CursoVistaModelo MapearCurso(NpgsqlDataReader reader)
    {
        return new CursoVistaModelo
        {
            Id = reader.GetInt64(0),
            Titulo = reader.GetString(1),
            Descripcion = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
            Nivel = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
            Idioma = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
            EsPublicado = reader.GetBoolean(5),
            CreadoEn = reader.GetDateTime(6),
            ActualizadoEn = reader.GetDateTime(7)
        };
    }
}
