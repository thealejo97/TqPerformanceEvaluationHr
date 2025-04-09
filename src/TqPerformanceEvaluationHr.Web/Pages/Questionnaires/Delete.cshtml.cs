using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqPerformanceEvaluationHr.Domain.Entities;
using TqPerformanceEvaluationHr.Infrastructure.Persistence;

namespace TqPerformanceEvaluationHr.Web.Pages.Questionnaires;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _context;

    public DeleteModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
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

    public async Task<IActionResult> OnPostAsync()
    {
        var questionnaire = await _context.Questionnaires
            .Include(q => q.Questions)
            .FirstOrDefaultAsync(q => q.Id == Questionnaire.Id);

        if (questionnaire == null)
        {
            return NotFound();
        }

        _context.Questions.RemoveRange(questionnaire.Questions);
        
        _context.Questionnaires.Remove(questionnaire);
        
        await _context.SaveChangesAsync();
        
        return RedirectToPage("./Index");
    }
} 