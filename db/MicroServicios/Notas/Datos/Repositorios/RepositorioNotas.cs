using Npgsql;
using Notas.Datos.Modelos;

namespace Notas.Datos.Repositorios;

public class RepositorioNotas : IRepositorioNotas
{
    private readonly string _userConnStr;
    private readonly string _contentConnStr;

    public RepositorioNotas(IConfiguration config)
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

    public async Task<IEnumerable<IntentoCuestionario>> ObtenerIntentosPorUsuarioAsync(int userId)
    {
        var lista = new List<IntentoCuestionario>();
        await using var conn = new NpgsqlConnection(_userConnStr);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "SELECT id, user_id, quiz_id, score, max_score, attempt_number, time_spent, attempted_at FROM quiz_attempts WHERE user_id = @uid", conn);
        cmd.Parameters.AddWithValue("uid", userId);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lista.Add(new IntentoCuestionario
            {
                Id            = reader.GetInt32(0),
                UserId        = reader.GetInt32(1),
                QuizId        = reader.GetInt32(2),
                Score         = reader.GetDouble(3),
                MaxScore      = reader.GetDouble(4),
                AttemptNumber = reader.GetInt32(5),
                TimeSpent     = reader.GetInt32(6),
                AttemptedAt   = reader.GetDateTime(7)
            });
        }
        return lista;
    }

    public async Task<IEnumerable<Cuestionario>> ObtenerCuestionariosPorIdsAsync(IEnumerable<int> quizIds)
    {
        var ids = quizIds.ToArray();
        var lista = new List<Cuestionario>();
        if (ids.Length == 0) return lista;

        await using var conn = new NpgsqlConnection(_contentConnStr);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "SELECT id, title FROM quizzes WHERE id = ANY(@ids)", conn);
        cmd.Parameters.AddWithValue("ids", ids);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lista.Add(new Cuestionario
            {
                Id    = reader.GetInt32(0),
                Title = reader.GetString(1)
            });
        }
        return lista;
    }

    public async Task<IEnumerable<Inscripcion>> ObtenerEnrollmentsPorUsuarioAsync(int userId)
    {
        var lista = new List<Inscripcion>();
        await using var conn = new NpgsqlConnection(_userConnStr);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "SELECT id, user_id, course_id, enrolled_at FROM enrollments WHERE user_id = @uid", conn);
        cmd.Parameters.AddWithValue("uid", userId);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lista.Add(new Inscripcion
            {
                Id         = reader.GetInt32(0),
                UserId     = reader.GetInt32(1),
                CourseId   = reader.GetInt32(2),
                EnrolledAt = reader.GetDateTime(3)
            });
        }
        return lista;
    }
}
