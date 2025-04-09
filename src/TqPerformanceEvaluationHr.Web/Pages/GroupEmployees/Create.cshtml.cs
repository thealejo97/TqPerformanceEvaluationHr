using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TqPerformanceEvaluationHr.Domain.Entities;
using TqPerformanceEvaluationHr.Infrastructure.Persistence;

namespace TqPerformanceEvaluationHr.Web.Pages.GroupEmployees;

public class CreateModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(AppDbContext context, ILogger<CreateModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    [BindProperty]
    public int EmployeeId { get; set; }

    [BindProperty]
    public int EvaluationGroupId { get; set; }

    public SelectList Employees { get; set; }
    public SelectList EvaluationGroups { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadSelectLists();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        _logger.LogInformation("Comenzando creación de GroupEmployee: Employee {EmployeeId}, Group {GroupId}", 
            EmployeeId, EvaluationGroupId);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Modelo inválido");
            await LoadSelectLists();
            return Page();
        }

        // Validar existencia del empleado
        var employee = await _context.Employees.FindAsync(EmployeeId);
        if (employee == null)
        {
            ModelState.AddModelError("EmployeeId", "Employee not found");
            await LoadSelectLists();
            return Page();
        }

        // Validar existencia del grupo
        var evaluationGroup = await _context.EvaluationGroups.FindAsync(EvaluationGroupId);
        if (evaluationGroup == null)
        {
            ModelState.AddModelError("EvaluationGroupId", "Evaluation Group not found");
            await LoadSelectLists();
            return Page();
        }

        // Verificar que no exista ya esta asignación
        var exists = await _context.GroupEmployees
            .AnyAsync(ge => ge.EmployeeId == EmployeeId && ge.EvaluationGroupId == EvaluationGroupId);
        
        if (exists)
        {
            ModelState.AddModelError(string.Empty, "This employee is already assigned to this evaluation group");
            await LoadSelectLists();
            return Page();
        }

        // Crear la nueva asignación
        var groupEmployee = new GroupEmployee
        {
            EmployeeId = EmployeeId,
            EvaluationGroupId = EvaluationGroupId
        };

        _context.GroupEmployees.Add(groupEmployee);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }

    private async Task LoadSelectLists()
    {
        // Cargar empleados
        Employees = new SelectList(
            await _context.Employees
                .Include(e => e.Position)
                .Select(e => new
                {
                    Id = e.Id, 
                    DisplayName = e.FullName + " (" + e.Position.Name + ")"
                })
                .OrderBy(e => e.DisplayName)
                .ToListAsync(),
            "Id", "DisplayName");

        // Cargar grupos de evaluación
        var evaluationGroupItems = await _context.EvaluationGroups
            .Include(eg => eg.EvaluationCycle)
            .Select(eg => new
            {
                Id = eg.Id,
                DisplayName = eg.Name + " - Cycle: " + eg.EvaluationCycle.Year + 
                    " (" + eg.StartDate.ToString("dd/MM/yyyy") + " - " + eg.EndDate.ToString("dd/MM/yyyy") + ")"
            })
            .OrderByDescending(eg => eg.Id)
            .ToListAsync();

        EvaluationGroups = new SelectList(evaluationGroupItems, "Id", "DisplayName");
    }
} 