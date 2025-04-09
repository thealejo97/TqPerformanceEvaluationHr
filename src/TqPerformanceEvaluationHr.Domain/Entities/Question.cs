namespace TqPerformanceEvaluationHr.Domain.Entities;

public class Question
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int QuestionnaireId { get; set; }

    public Questionnaire Questionnaire { get; set; } = null!;
    public ICollection<EvaluationResponse> Responses { get; set; } = new List<EvaluationResponse>();
}
