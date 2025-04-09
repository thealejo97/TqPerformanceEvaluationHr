namespace TqPerformanceEvaluationHr.Application.DTOs;

public class ReportByPositionDto
{
    public string PositionName { get; set; } = string.Empty;
    public double AverageScore { get; set; }
    public int EvaluatedCount { get; set; }
}
