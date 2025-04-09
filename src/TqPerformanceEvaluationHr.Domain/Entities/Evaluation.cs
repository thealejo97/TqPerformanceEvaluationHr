namespace TqPerformanceEvaluationHr.Domain.Entities;

public class Evaluation
{
    public int Id { get; set; }
    public int GroupEmployeeId { get; set; }
    public int QuestionnaireId { get; set; }
    public int EmployeeId { get; set; }
    public DateTime EvaluationDate { get; set; }

    public GroupEmployee GroupEmployee { get; set; } = null!;
    public Questionnaire Questionnaire { get; set; } = null!;
    public Employee Employee { get; set; } = null!;
    public ICollection<EvaluationResponse> Responses { get; set; } = new List<EvaluationResponse>();
    public ICollection<EvaluationResult> Results { get; set; } = new List<EvaluationResult>();
}
