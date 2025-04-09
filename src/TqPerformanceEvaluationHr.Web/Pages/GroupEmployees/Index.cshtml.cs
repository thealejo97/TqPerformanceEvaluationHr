using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqPerformanceEvaluationHr.Domain.Entities;
using TqPerformanceEvaluationHr.Infrastructure.Persistence;

namespace TqPerformanceEvaluationHr.Web.Pages.GroupEmployees;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    public IList<GroupEmployee> GroupEmployees { get; set; } = new List<GroupEmployee>();

    public async Task OnGetAsync()
    {
        GroupEmployees = await _context.GroupEmployees
            .Include(ge => ge.Employee)
            .Include(ge => ge.EvaluationGroup)
                .ThenInclude(eg => eg.EvaluationCycle)
            .OrderBy(ge => ge.EvaluationGroup.Name)
            .ThenBy(ge => ge.Employee.FullName)
            .ToListAsync();
    }
} 