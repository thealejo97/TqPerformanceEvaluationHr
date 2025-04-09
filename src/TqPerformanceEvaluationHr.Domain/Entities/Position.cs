namespace TqPerformanceEvaluationHr.Domain.Entities;

public class Position
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public ICollection<Questionnaire> Questionnaires { get; set; } = new List<Questionnaire>();
}
