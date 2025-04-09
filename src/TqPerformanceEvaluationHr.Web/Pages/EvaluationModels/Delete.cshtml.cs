using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TqPerformanceEvaluationHr.Domain.Entities;
using TqPerformanceEvaluationHr.Infrastructure.Persistence;

namespace TqPerformanceEvaluationHr.Web.Pages.EvaluationModels;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _context;
    public DeleteModel(AppDbContext context) => _context = context;

    [BindProperty]
    public EvaluationModel EvaluationModel { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        EvaluationModel = await _context.EvaluationModels.FindAsync(id)
            ?? throw new Exception("Modelo no encontrado");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var model = await _context.EvaluationModels.FindAsync(EvaluationModel.Id);
        if (model is not null)
        {
            _context.EvaluationModels.Remove(model);
            await _context.SaveChangesAsync();
        }
        return RedirectToPage("Index");
    }
}
