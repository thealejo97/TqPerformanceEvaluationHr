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

        var allPositions = await _context.Positions
            .Select(p => new { p.Id, p.Name })
            .ToListAsync();


        var positionsWithEvaluations = await _context.Evaluations
            .Include(e => e.Employee)
                .ThenInclude(emp => emp.Position)
            .Include(e => e.Responses)
            .Where(e => e.Responses.Any())
            .GroupBy(e => new { e.Employee.PositionId, PositionName = e.Employee.Position.Name })
            .Select(g => new 
            {
                PositionId = g.Key.PositionId,
                PositionName = g.Key.PositionName,
                AverageScore = g.Average(e => e.Responses.Average(r => r.Score)),
                EvaluatedCount = g.Count()
            })
            .ToListAsync();


        ReportData = allPositions
            .Select(p => new ReportByPositionDto
            {
                PositionName = p.Name,
        
                AverageScore = positionsWithEvaluations
                    .FirstOrDefault(pe => pe.PositionId == p.Id)?.AverageScore ?? 0,
        
                EvaluatedCount = positionsWithEvaluations
                    .FirstOrDefault(pe => pe.PositionId == p.Id)?.EvaluatedCount ?? 0
            })
            .ToList();
    }
}
