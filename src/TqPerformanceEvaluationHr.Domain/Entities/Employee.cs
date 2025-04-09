namespace TqPerformanceEvaluationHr.Domain.Entities;

public class Employee
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int PositionId { get; set; }

    public Position Position { get; set; } = null!;
    public ICollection<GroupEmployee> GroupEmployees { get; set; } = new List<GroupEmployee>();
    public ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
}
