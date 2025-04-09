namespace TqPerformanceEvaluationHr.Domain.Entities;

public class EvaluationGroup
{
    public int Id { get; set; }
    public int EvaluationCycleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public EvaluationCycle EvaluationCycle { get; set; } = null!;
    public ICollection<GroupEmployee> GroupEmployees { get; set; } = new List<GroupEmployee>();
}
