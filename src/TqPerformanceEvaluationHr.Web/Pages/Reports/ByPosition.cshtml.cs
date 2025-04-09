using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqPerformanceEvaluationHr.Application.DTOs;
using TqPerformanceEvaluationHr.Infrastructure.Persistence;

namespace TqPerformanceEvaluationHr.Web.Pages.Reports;

public class ByPositionModel : PageModel
{
    private readonly AppDbContext _context;

    public ByPositionModel(AppDbContext context)
    {
        _context = context;
    }

    public List<ReportByPositionDto> ReportData { get; set; } = [];

    public async Task OnGetAsync()
    {
        ReportData = await _context.Evaluations
            .Include(e => e.Employee)
                .ThenInclude(emp => emp.Position)
            .Include(e => e.Responses)
            .GroupBy(e => e.Employee.Position.Name)
            .Select(g => new ReportByPositionDto
            {
                PositionName = g.Key,
                AverageScore = g
                    .Where(e => e.Responses.Any())
                    .Average(e => e.Responses.Average(r => r.Score)),
                EvaluatedCount = g.Count()
            })
            .ToListAsync();
    }
}
