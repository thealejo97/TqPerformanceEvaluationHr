using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqPerformanceEvaluationHr.Domain.Entities;
using TqPerformanceEvaluationHr.Infrastructure.Persistence;

namespace TqPerformanceEvaluationHr.Web.Pages.Evaluations;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    public IList<Evaluation> Evaluations { get; set; } = new List<Evaluation>();

    public async Task OnGetAsync()
    {
        Evaluations = await _context.Evaluations
            .Include(e => e.Employee)
            .Include(e => e.GroupEmployee)
                .ThenInclude(ge => ge.EvaluationGroup)
            .Include(e => e.Questionnaire)
            .Include(e => e.Responses)
            .Include(e => e.Results)
            .ToListAsync();
    }
} 