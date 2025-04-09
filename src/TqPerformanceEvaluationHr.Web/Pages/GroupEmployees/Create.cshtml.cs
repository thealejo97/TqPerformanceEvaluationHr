using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
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
    public InputModel Input { get; set; } = new();

    public List<SelectListItem> EvaluationGroups { get; set; } = new();
    public List<SelectListItem> Employees { get; set; } = new();

    [BindProperty]
    public GroupEmployee GroupEmployee { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Evaluation group is required")]
        [Display(Name = "Evaluation Group")]
        public int EvaluationGroupId { get; set; }

        [Required(ErrorMessage = "Employee is required")]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            await LoadSelectListsAsync();
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading employee assignment creation page");
            TempData["ErrorMessage"] = "An error occurred while loading the page. Please try again.";
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            _logger.LogInformation("Starting assignment creation: Employee {EmployeeId}, Group {GroupId}", 
                Input.EmployeeId, Input.EvaluationGroupId);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model when creating assignment");
                await LoadSelectListsAsync();
                return Page();
            }

            // Verify that the group exists
            var evaluationGroup = await _context.EvaluationGroups
                .Include(g => g.EvaluationCycle)
                .FirstOrDefaultAsync(g => g.Id == Input.EvaluationGroupId);

            if (evaluationGroup == null)
            {
                _logger.LogWarning("Evaluation group not found: {GroupId}", Input.EvaluationGroupId);
                ModelState.AddModelError("Input.EvaluationGroupId", "The selected evaluation group does not exist.");
                await LoadSelectListsAsync();
                return Page();
            }

            // Verify that the employee exists
            var employee = await _context.Employees.FindAsync(Input.EmployeeId);
            if (employee == null)
            {
                _logger.LogWarning("Employee not found: {EmployeeId}", Input.EmployeeId);
                ModelState.AddModelError("Input.EmployeeId", "The selected employee does not exist.");
                await LoadSelectListsAsync();
                return Page();
            }

            // Verify that the employee is not already assigned to this group
            var existingAssignment = await _context.GroupEmployees
                .FirstOrDefaultAsync(ge => 
                    ge.EvaluationGroupId == Input.EvaluationGroupId && 
                    ge.EmployeeId == Input.EmployeeId);

            if (existingAssignment != null)
            {
                _logger.LogWarning("Duplicate assignment: Employee {EmployeeId}, Group {GroupId}", 
                    Input.EmployeeId, Input.EvaluationGroupId);
                ModelState.AddModelError(string.Empty, "This employee is already assigned to the selected group.");
                await LoadSelectListsAsync();
                return Page();
            }

            GroupEmployee.EvaluationGroupId = Input.EvaluationGroupId;
            GroupEmployee.EmployeeId = Input.EmployeeId;

            _context.GroupEmployees.Add(GroupEmployee);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Assignment created successfully: Employee {EmployeeId}, Group {GroupId}", 
                Input.EmployeeId, Input.EvaluationGroupId);

            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating assignment: Employee {EmployeeId}, Group {GroupId}", 
                Input.EmployeeId, Input.EvaluationGroupId);
            ModelState.AddModelError(string.Empty, "An error occurred while creating the assignment. Please try again.");
            await LoadSelectListsAsync();
            return Page();
        }
    }

    private async Task LoadSelectListsAsync()
    {
        try
        {
            // Load active evaluation groups
            var groups = await _context.EvaluationGroups
                .Include(g => g.EvaluationCycle)
                .Where(g => g.EvaluationCycle.IsActive)
                .OrderByDescending(g => g.EvaluationCycle.Year)
                .ThenBy(g => g.Name)
                .Select(g => new
                {
                    g.Id,
                    g.Name,
                    CycleYear = g.EvaluationCycle.Year,
                    g.StartDate,
                    g.EndDate
                })
                .ToListAsync();

            EvaluationGroups = groups
                .Select(g => new SelectListItem
                {
                    Value = g.Id.ToString(),
                    Text = $"{g.Name} - Cycle: {g.CycleYear} ({g.StartDate:dd/MM/yyyy} - {g.EndDate:dd/MM/yyyy})"
                })
                .ToList();

            // Load employees
            Employees = await _context.Employees
                .OrderBy(e => e.FullName)
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.FullName
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading selection lists");
            throw;
        }
    }
} 