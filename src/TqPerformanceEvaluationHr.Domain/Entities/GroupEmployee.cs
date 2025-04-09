namespace TqPerformanceEvaluationHr.Domain.Entities;

public class GroupEmployee
{
    public int Id { get; set; }
    public int EvaluationGroupId { get; set; }
    public int EmployeeId { get; set; }

    public EvaluationGroup EvaluationGroup { get; set; } = null!;
    public Employee Employee { get; set; } = null!;
    public ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
}
