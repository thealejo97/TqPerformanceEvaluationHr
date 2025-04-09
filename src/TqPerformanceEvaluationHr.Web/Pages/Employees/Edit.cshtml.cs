using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TqPerformanceEvaluationHr.Domain.Entities;
using TqPerformanceEvaluationHr.Infrastructure.Persistence;

namespace TqPerformanceEvaluationHr.Web.Pages.Employees;

public class EditModel : PageModel
{
    private readonly AppDbContext _context;
    public EditModel(AppDbContext context) => _context = context;

    [BindProperty]
    public Employee Employee { get; set; } = new();

    public SelectList Positions { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Employee = await _context.Employees.FindAsync(id);
        if (Employee == null) return NotFound();

        Positions = new SelectList(_context.Positions.ToList(), "Id", "Name");
        return Page();
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

        try
        {
            
            var empleadoExistente = await _context.Employees
                .AsNoTracking()  
                .FirstOrDefaultAsync(e => e.Id == Employee.Id);

            if (empleadoExistente == null)
            {
                Console.WriteLine("🔴 Empleado no encontrado");
                ModelState.AddModelError(string.Empty, "No se encontró el empleado");
                Positions = new SelectList(_context.Positions.ToList(), "Id", "Name");
                return Page();
            }

            Console.WriteLine("🔴 Empleado encontrado, actualizando datos");
            
            
            _context.Update(Employee);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Error al actualizar: {ex.Message}");
            Positions = new SelectList(_context.Positions.ToList(), "Id", "Name");
            return Page();
        }
    }
}
