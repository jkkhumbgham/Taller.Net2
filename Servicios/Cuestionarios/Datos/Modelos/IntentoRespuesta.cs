namespace Cuestionarios.Datos.Modelos;

public class IntentoRespuesta
{
    public int Id { get; set; }
    public int QuizAttemptId { get; set; }
    public int QuestionId { get; set; }
    public bool IsCorrect { get; set; }
    public int TimeSpent { get; set; }

    public IntentoCuestionario? IntentoCuestionario { get; set; }
}
