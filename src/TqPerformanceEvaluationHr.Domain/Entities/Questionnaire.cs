namespace TqPerformanceEvaluationHr.Domain.Entities;

public class Questionnaire
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;

    public int EvaluationModelId { get; set; }
    public int PositionId { get; set; }

    public EvaluationModel EvaluationModel { get; set; } = null!;
    public Position Position { get; set; } = null!;
    public ICollection<Question> Questions { get; set; } = new List<Question>();
    public ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
}
