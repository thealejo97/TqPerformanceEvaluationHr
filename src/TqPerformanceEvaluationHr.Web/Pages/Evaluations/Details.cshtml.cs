using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqPerformanceEvaluationHr.Domain.Entities;
using TqPerformanceEvaluationHr.Infrastructure.Persistence;

namespace TqPerformanceEvaluationHr.Web.Pages.Evaluations;

public class DetailsModel : PageModel
{
    private readonly AppDbContext _context;

    public DetailsModel(AppDbContext context)
    {
        _context = context;
    }

    public Evaluation Evaluation { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Evaluation = await _context.Evaluations
            .Include(e => e.Employee)
            .Include(e => e.GroupEmployee)
                .ThenInclude(ge => ge.EvaluationGroup)
            .Include(e => e.Questionnaire)
                .ThenInclude(q => q.EvaluationModel)
            .Include(e => e.Questionnaire)
                .ThenInclude(q => q.Position)
            .Include(e => e.Responses)
                .ThenInclude(r => r.Question)
            .Include(e => e.Results)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (Evaluation == null)
        {
            return NotFound();
        }

        return Page();
    }
} 