namespace MejoresEstudiantes.Datos.Modelos;

public class IntentoCuestionario
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int QuizId { get; set; }
    public double Score { get; set; }
    public double MaxScore { get; set; }
    public int AttemptNumber { get; set; }
    public int TimeSpent { get; set; }
    public DateTime AttemptedAt { get; set; }

    public Usuario? Usuario { get; set; }
}
