using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqPerformanceEvaluationHr.Domain.Entities;
using TqPerformanceEvaluationHr.Infrastructure.Persistence;

namespace TqPerformanceEvaluationHr.Web.Pages.Employees;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;
    public IndexModel(AppDbContext context) => _context = context;

    public IList<Employee> Employees { get; set; } = new List<Employee>();

    public async Task OnGetAsync()
    {
        Employees = await _context.Employees.Include(e => e.Position).ToListAsync();
    }
}
