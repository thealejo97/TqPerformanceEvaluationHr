using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TqPerformanceEvaluationHr.Domain.Entities;
using TqPerformanceEvaluationHr.Infrastructure.Persistence;

namespace TqPerformanceEvaluationHr.Web.Pages.Employees;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _context;
    public DeleteModel(AppDbContext context) => _context = context;

    [BindProperty]
    public Employee Employee { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Employee = await _context.Employees.FindAsync(id);
        return Employee == null ? NotFound() : Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var employee = await _context.Employees.FindAsync(Employee.Id);
        if (employee == null) return NotFound();

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
