using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqPerformanceEvaluationHr.Domain.Entities;
using TqPerformanceEvaluationHr.Infrastructure.Persistence;

namespace TqPerformanceEvaluationHr.Web.Pages.Questionnaires;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    public IList<Questionnaire> Questionnaires { get; set; } = new List<Questionnaire>();

    public async Task OnGetAsync()
    {
        Questionnaires = await _context.Questionnaires
            .Include(q => q.EvaluationModel)
            .Include(q => q.Position)
            .Include(q => q.Questions)
            .ToListAsync();
    }
} 