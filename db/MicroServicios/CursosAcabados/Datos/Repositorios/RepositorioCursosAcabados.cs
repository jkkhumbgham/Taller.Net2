using Npgsql;
using CursosAcabados.Datos.Modelos;

namespace CursosAcabados.Datos.Repositorios;

public class RepositorioCursosAcabados : IRepositorioCursosAcabados
{
    private readonly string _userConnStr;
    private readonly string _contentConnStr;

    public RepositorioCursosAcabados(IConfiguration config)
    {
        _userConnStr    = config.GetConnectionString("UserDb")!;
        _contentConnStr = config.GetConnectionString("ContentDb")!;
    }

    public async Task<Usuario?> ObtenerUsuarioPorIdAsync(int userId)
    {
        await using var conn = new NpgsqlConnection(_userConnStr);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "SELECT id, name, email FROM users WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", userId);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;

        return new Usuario
        {
            Id    = reader.GetInt32(0),
            Name  = reader.GetString(1),
            Email = reader.GetString(2)
        };
    }

    public async Task<IEnumerable<Inscripcion>> ObtenerInscripcionesPorUsuarioAsync(int userId)
    {
        var lista = new List<Inscripcion>();
        await using var conn = new NpgsqlConnection(_userConnStr);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "SELECT id, user_id, course_id, enrolled_at, progress FROM enrollments WHERE user_id = @uid", conn);
        cmd.Parameters.AddWithValue("uid", userId);

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

    public async Task<Curso?> ObtenerCursoPorIdAsync(int courseId)
    {
        await using var conn = new NpgsqlConnection(_contentConnStr);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "SELECT id, title, description FROM courses WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", courseId);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;

        return new Curso
        {
            Id          = reader.GetInt32(0),
            Title       = reader.GetString(1),
            Description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2)
        };
    }

    public async Task<IEnumerable<Leccion>> ObtenerLeccionesPorCursoAsync(int courseId)
    {
        var lista = new List<Leccion>();
        await using var conn = new NpgsqlConnection(_contentConnStr);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(@"
            SELECT l.id, l.module_id, l.title, l.duration
            FROM lessons l
            INNER JOIN modules m ON l.module_id = m.id
            WHERE m.course_id = @cid", conn);
        cmd.Parameters.AddWithValue("cid", courseId);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lista.Add(new Leccion
            {
                Id       = reader.GetInt32(0),
                ModuleId = reader.GetInt32(1),
                Title    = reader.GetString(2),
                Duration = reader.IsDBNull(3) ? 0 : reader.GetInt32(3)
            });
        }
        return lista;
    }

    public async Task<IEnumerable<ProgresoLeccion>> ObtenerProgresosPorUsuarioYLeccionesAsync(int userId, IEnumerable<int> lessonIds)
    {
        var ids = lessonIds.ToArray();
        var lista = new List<ProgresoLeccion>();
        if (ids.Length == 0) return lista;

        await using var conn = new NpgsqlConnection(_userConnStr);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(@"
            SELECT id, user_id, lesson_id, status, progress_percent, time_spent, completed_at, updated_at
            FROM lesson_progress
            WHERE user_id = @uid AND lesson_id = ANY(@ids)", conn);
        cmd.Parameters.AddWithValue("uid", userId);
        cmd.Parameters.AddWithValue("ids", ids);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lista.Add(new ProgresoLeccion
            {
                Id              = reader.GetInt32(0),
                UserId          = reader.GetInt32(1),
                LessonId        = reader.GetInt32(2),
                Status          = reader.GetString(3),
                ProgressPercent = reader.GetDouble(4),
                TimeSpent       = reader.GetInt32(5),
                CompletedAt     = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                UpdatedAt       = reader.GetDateTime(7)
            });
        }
        return lista;
    }
}
