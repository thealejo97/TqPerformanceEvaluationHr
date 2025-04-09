using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqPerformanceEvaluationHr.Domain.Entities;
using TqPerformanceEvaluationHr.Infrastructure.Persistence;

namespace TqPerformanceEvaluationHr.Web.Pages.Questionnaires;

public class DetailsModel : PageModel
{
    private readonly AppDbContext _context;

    public DetailsModel(AppDbContext context)
    {
        _context = context;
    }

    public Questionnaire Questionnaire { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var questionnaire = await _context.Questionnaires
            .Include(q => q.EvaluationModel)
            .Include(q => q.Position)
            .Include(q => q.Questions)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (questionnaire == null)
        {
            return NotFound();
        }

        Questionnaire = questionnaire;
        return Page();
    }
} 