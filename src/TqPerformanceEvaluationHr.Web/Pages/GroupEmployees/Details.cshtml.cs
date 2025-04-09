using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqPerformanceEvaluationHr.Domain.Entities;
using TqPerformanceEvaluationHr.Infrastructure.Persistence;

namespace TqPerformanceEvaluationHr.Web.Pages.GroupEmployees;

public class DetailsModel : PageModel
{
    private readonly AppDbContext _context;

    public DetailsModel(AppDbContext context)
    {
        _context = context;
        GroupEmployee = new GroupEmployee();
    }

    public GroupEmployee GroupEmployee { get; set; }
    public IList<Evaluation> RelatedEvaluations { get; set; } = new List<Evaluation>();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var groupEmployee = await _context.GroupEmployees
            .Include(ge => ge.Employee)
                .ThenInclude(e => e.Position)
            .Include(ge => ge.EvaluationGroup)
                .ThenInclude(eg => eg.EvaluationCycle)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (groupEmployee == null)
        {
            return NotFound();
        }

        GroupEmployee = groupEmployee;

        // Cargar evaluaciones relacionadas
        RelatedEvaluations = await _context.Evaluations
            .Include(e => e.Questionnaire)
            .Include(e => e.Responses)
            .Include(e => e.Results)
            .Where(e => e.GroupEmployeeId == GroupEmployee.Id)
            .OrderByDescending(e => e.EvaluationDate)
            .ToListAsync();

        return Page();
    }
} 