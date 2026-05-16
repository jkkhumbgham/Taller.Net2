using Npgsql;
using ClasesMasTomadas.Datos.Modelos;

namespace ClasesMasTomadas.Datos.Repositorios;

public class RepositorioClasesMasTomadas : IRepositorioClasesMasTomadas
{
    private readonly string _userConnStr;
    private readonly string _contentConnStr;

    public RepositorioClasesMasTomadas(IConfiguration config)
    {
        _userConnStr    = config.GetConnectionString("UserDb")!;
        _contentConnStr = config.GetConnectionString("ContentDb")!;
    }

    public async Task<IEnumerable<Inscripcion>> ObtenerTodasLasInscripcionesAsync()
    {
        var lista = new List<Inscripcion>();
        await using var conn = new NpgsqlConnection(_userConnStr);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "SELECT id, user_id, course_id, enrolled_at, progress FROM enrollments", conn);
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            lista.Add(new Inscripcion
            {
                Id         = reader.GetInt32(0),
                UserId     = reader.GetInt32(1),
                CourseId   = reader.GetInt32(2),
                EnrolledAt = reader.GetDateTime(3),
                Progress   = reader.GetDouble(4)
            });
        }
        return lista;
    }

    public async Task<IEnumerable<Curso>> ObtenerCursosPorIdsAsync(IEnumerable<int> courseIds)
    {
        var ids = courseIds.ToArray();
        var lista = new List<Curso>();
        if (ids.Length == 0) return lista;

        await using var conn = new NpgsqlConnection(_contentConnStr);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "SELECT id, title, description FROM courses WHERE id = ANY(@ids)", conn);
        cmd.Parameters.AddWithValue("ids", ids);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lista.Add(new Curso
            {
                Id          = reader.GetInt32(0),
                Title       = reader.GetString(1),
                Description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2)
            });
        }
        return lista;
    }
}
