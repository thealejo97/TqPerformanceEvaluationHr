namespace TqPerformanceEvaluationHr.Domain.Entities;

public class EvaluationResult
{
    public int Id { get; set; }
    public string QualitativeConcept { get; set; } = string.Empty;
    public int EvaluationId { get; set; }

    public Evaluation Evaluation { get; set; } = null!;
}
