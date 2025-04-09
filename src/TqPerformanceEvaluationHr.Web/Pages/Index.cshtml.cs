using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqPerformanceEvaluationHr.Infrastructure.Persistence;

namespace TqPerformanceEvaluationHr.Web.Pages;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public int TotalEvaluationModels { get; set; }
    public int TotalEmployees { get; set; }
    public int TotalEvaluations { get; set; }
    public int TotalActiveCycles { get; set; }
    public int TotalPositions { get; set; }
    public int TotalQuestionnaires { get; set; }

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    public async Task OnGetAsync()
    {
        TotalEvaluationModels = await _context.EvaluationModels.CountAsync();
        TotalEmployees = await _context.Employees.CountAsync();
        TotalEvaluations = await _context.Evaluations.CountAsync();
        TotalActiveCycles = await _context.EvaluationCycles.CountAsync(c => c.IsActive);
        TotalPositions = await _context.Positions.CountAsync();
        TotalQuestionnaires = await _context.Questionnaires.CountAsync();
    }
}
