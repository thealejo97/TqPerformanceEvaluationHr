using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqPerformanceEvaluationHr.Domain.Entities;
using TqPerformanceEvaluationHr.Infrastructure.Persistence;

namespace TqPerformanceEvaluationHr.Web.Pages.Evaluations;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(AppDbContext context, ILogger<DeleteModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    [BindProperty]
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
            .Include(e => e.Responses)
            .Include(e => e.Results)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (Evaluation == null)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Evaluation = await _context.Evaluations
            .Include(e => e.Responses)
            .Include(e => e.Results)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (Evaluation == null)
        {
            return NotFound();
        }

        try
        {
            // Eliminar primero las respuestas y resultados asociados
            _context.EvaluationResponses.RemoveRange(Evaluation.Responses);
            _context.EvaluationResults.RemoveRange(Evaluation.Results);
            
            // Luego eliminar la evaluación
            _context.Evaluations.Remove(Evaluation);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Evaluation {Id} deleted successfully", id);
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting evaluation {Id}", id);
            ModelState.AddModelError(string.Empty, "An error occurred while deleting the evaluation. Please try again.");
            
            // Recargar la evaluación para mostrarla nuevamente
            Evaluation = await _context.Evaluations
                .Include(e => e.Employee)
                .Include(e => e.GroupEmployee)
                    .ThenInclude(ge => ge.EvaluationGroup)
                .Include(e => e.Questionnaire)
                .FirstOrDefaultAsync(e => e.Id == id);
                
            return Page();
        }
    }
} 