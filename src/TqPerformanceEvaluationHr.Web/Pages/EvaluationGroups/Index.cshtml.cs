using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqPerformanceEvaluationHr.Domain.Entities;
using TqPerformanceEvaluationHr.Infrastructure.Persistence;

namespace TqPerformanceEvaluationHr.Web.Pages.EvaluationGroups;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    public IList<EvaluationGroup> EvaluationGroups { get; set; } = new List<EvaluationGroup>();

    public async Task OnGetAsync()
    {
        EvaluationGroups = await _context.EvaluationGroups
            .Include(eg => eg.EvaluationCycle)
            .Include(eg => eg.GroupEmployees)
            .OrderByDescending(eg => eg.EvaluationCycle.Year)
            .ThenBy(eg => eg.Name)
            .ToListAsync();
    }
} 