using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqPerformanceEvaluationHr.Domain.Entities;
using TqPerformanceEvaluationHr.Infrastructure.Persistence;

namespace TqPerformanceEvaluationHr.Web.Pages.GroupEmployees;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(AppDbContext context, ILogger<DeleteModel> logger)
    {
        _context = context;
        _logger = logger;
        GroupEmployee = new GroupEmployee();
    }

    [BindProperty]
    public GroupEmployee GroupEmployee { get; set; }
    
    public int RelatedEvaluationsCount { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var groupEmployee = await _context.GroupEmployees
            .Include(ge => ge.Employee)
            .Include(ge => ge.EvaluationGroup)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (groupEmployee == null)
        {
            return NotFound();
        }

        GroupEmployee = groupEmployee;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var groupEmployee = await _context.GroupEmployees
            .Include(ge => ge.Employee)
            .Include(ge => ge.EvaluationGroup)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (groupEmployee == null)
        {
            return NotFound();
        }

        GroupEmployee = groupEmployee;

        // Verificar si hay evaluaciones relacionadas
        var hasEvaluations = await _context.Evaluations
            .AnyAsync(e => e.GroupEmployeeId == GroupEmployee.Id);

        if (hasEvaluations)
        {
            ModelState.AddModelError(string.Empty, "Cannot delete this assignment because it has associated evaluations. Delete the evaluations first.");
            
            // Recargar el GroupEmployee con todos sus datos para mostrar en la página
            GroupEmployee = await _context.GroupEmployees
                .Include(ge => ge.Employee)
                .Include(ge => ge.EvaluationGroup)
                    .ThenInclude(eg => eg.EvaluationCycle)
                .FirstOrDefaultAsync(ge => ge.Id == id);
                
            RelatedEvaluationsCount = await _context.Evaluations
                .CountAsync(e => e.GroupEmployeeId == GroupEmployee.Id);
                
            return Page();
        }

        try
        {
            _context.GroupEmployees.Remove(GroupEmployee);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("GroupEmployee {Id} deleted successfully", id);
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting GroupEmployee {Id}", id);
            ModelState.AddModelError(string.Empty, "An error occurred while deleting the assignment. Please try again.");
            
            // Recargar el GroupEmployee para mostrarlo nuevamente
            GroupEmployee = await _context.GroupEmployees
                .Include(ge => ge.Employee)
                .Include(ge => ge.EvaluationGroup)
                    .ThenInclude(eg => eg.EvaluationCycle)
                .FirstOrDefaultAsync(ge => ge.Id == id);
                
            RelatedEvaluationsCount = await _context.Evaluations
                .CountAsync(e => e.GroupEmployeeId == GroupEmployee.Id);
                
            return Page();
        }
    }
} 