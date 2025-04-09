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
        Employees = new List<SelectListItem>();
        EvaluationGroups = new List<SelectListItem>();
    }

    [BindProperty]
    public int EmployeeId { get; set; }

    [BindProperty]
    public int EvaluationGroupId { get; set; }

    public List<SelectListItem> Employees { get; set; }
    public List<SelectListItem> EvaluationGroups { get; set; }

    [BindProperty]
    public GroupEmployee GroupEmployee { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadSelectListsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        _logger.LogInformation("Comenzando creación de GroupEmployee: Employee {EmployeeId}, Group {GroupId}", 
            EmployeeId, EvaluationGroupId);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Modelo inválido");
            await LoadSelectListsAsync();
            return Page();
        }

        
        var employee = await _context.Employees.FindAsync(EmployeeId);
        if (employee == null)
        {
            ModelState.AddModelError("EmployeeId", "Employee not found");
            await LoadSelectListsAsync();
            return Page();
        }

        
        var evaluationGroup = await _context.EvaluationGroups.FindAsync(EvaluationGroupId);
        if (evaluationGroup == null)
        {
            ModelState.AddModelError("EvaluationGroupId", "Evaluation Group not found");
            await LoadSelectListsAsync();
            return Page();
        }

        
        var exists = await _context.GroupEmployees
            .AnyAsync(ge => ge.EmployeeId == EmployeeId && ge.EvaluationGroupId == EvaluationGroupId);
        
        if (exists)
        {
            ModelState.AddModelError(string.Empty, "This employee is already assigned to this evaluation group");
            await LoadSelectListsAsync();
            return Page();
        }

        
        GroupEmployee.EmployeeId = EmployeeId;
        GroupEmployee.EvaluationGroupId = EvaluationGroupId;

        _context.GroupEmployees.Add(GroupEmployee);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }

    private async Task LoadSelectListsAsync()
    {
        var employees = await _context.Employees
            .Include(e => e.Position)
            .Select(e => new SelectListItem
            {
                Value = e.Id.ToString(),
                Text = e.FullName + " (" + e.Position.Name + ")"
            })
            .OrderBy(e => e.Text)
            .ToListAsync();

        var groups = await _context.EvaluationGroups
            .Include(eg => eg.EvaluationCycle)
            .Select(g => new SelectListItem
            {
                Value = g.Id.ToString(),
                Text = g.Name + " - Cycle: " + g.EvaluationCycle.Year + 
                    " (" + g.StartDate.ToString("dd/MM/yyyy") + " - " + g.EndDate.ToString("dd/MM/yyyy") + ")"
            })
            .OrderByDescending(g => g.Text)
            .ToListAsync();

        Employees = employees;
        EvaluationGroups = groups;
    }
} 