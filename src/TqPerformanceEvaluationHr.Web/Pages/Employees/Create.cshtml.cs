using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TqPerformanceEvaluationHr.Domain.Entities;
using TqPerformanceEvaluationHr.Infrastructure.Persistence;

namespace TqPerformanceEvaluationHr.Web.Pages.Employees;

public class CreateModel : PageModel
{
    private readonly AppDbContext _context;
    public CreateModel(AppDbContext context) => _context = context;

    [BindProperty]
    public Employee Employee { get; set; } = new();

    public SelectList Positions { get; set; }

    public void OnGet()
    {
        Positions = new SelectList(_context.Positions.ToList(), "Id", "Name");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        
        if (Employee.PositionId > 0 && !ModelState.IsValid)
        {
            
            if (ModelState.ErrorCount == 1 && ModelState.ContainsKey("Employee.PositionId"))
            {
                ModelState.Clear(); 
            }
            else
            {
                
                Positions = new SelectList(_context.Positions.ToList(), "Id", "Name");
                return Page();
            }
        }
        else if (!ModelState.IsValid)
        {
            Positions = new SelectList(_context.Positions.ToList(), "Id", "Name");
            return Page();
        }

        _context.Employees.Add(Employee);
        await _context.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
