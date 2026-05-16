using Npgsql;
using MejoresEstudiantes.Datos.Modelos;

namespace MejoresEstudiantes.Datos.Repositorios;

public class RepositorioMejoresEstudiantes : IRepositorioMejoresEstudiantes
{
    private readonly string _userConnStr;

    public RepositorioMejoresEstudiantes(IConfiguration config)
    {
        _userConnStr = config.GetConnectionString("UserDb")!;
    }

    public async Task<IEnumerable<IntentoCuestionario>> ObtenerTodosLosIntentosAsync()
    {
        var lista = new List<IntentoCuestionario>();
        await using var conn = new NpgsqlConnection(_userConnStr);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "SELECT id, user_id, quiz_id, score, max_score, attempt_number, time_spent, attempted_at FROM quiz_attempts", conn);
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

    public async Task<IEnumerable<Usuario>> ObtenerTodosLosUsuariosAsync()
    {
        var lista = new List<Usuario>();
        await using var conn = new NpgsqlConnection(_userConnStr);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "SELECT id, name, email FROM users", conn);
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            lista.Add(new Usuario
            {
                Id    = reader.GetInt32(0),
                Name  = reader.GetString(1),
                Email = reader.GetString(2)
            });
        }
        return lista;
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
}
