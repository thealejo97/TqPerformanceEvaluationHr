namespace TqPerformanceEvaluationHr.Domain.Entities;

public class EvaluationResponse
{
    public int Id { get; set; }
    public int EvaluationId { get; set; }
    public int QuestionId { get; set; }
    public int Score { get; set; }

    public Evaluation Evaluation { get; set; } = null!;
    public Question Question { get; set; } = null!;
}
