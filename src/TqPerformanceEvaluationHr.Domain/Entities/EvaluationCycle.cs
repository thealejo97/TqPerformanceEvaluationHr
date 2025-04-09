namespace TqPerformanceEvaluationHr.Domain.Entities;

public class EvaluationCycle
{
    public int Id { get; set; }
    public int Year { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<EvaluationGroup> EvaluationGroups { get; set; } = new List<EvaluationGroup>();
}
