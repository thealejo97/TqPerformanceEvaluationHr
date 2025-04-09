using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TqPerformanceEvaluationHr.Domain.Entities;
using TqPerformanceEvaluationHr.Infrastructure.Persistence;

namespace TqPerformanceEvaluationHr.Web.Pages.Evaluations;

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
    public int GroupEmployeeId { get; set; }

    [BindProperty]
    public int QuestionnaireId { get; set; }

    [BindProperty]
    public DateTime EvaluationDate { get; set; } = DateTime.Today;

    public SelectList Employees { get; set; }
    public SelectList GroupEmployees { get; set; }
    public SelectList Questionnaires { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadSelectLists();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        _logger.LogInformation("Comenzando creación de evaluación: Employee {EmployeeId}, Group {GroupId}, Questionnaire {QuestionnaireId}", 
            EmployeeId, GroupEmployeeId, QuestionnaireId);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Modelo inválido");
            await LoadSelectLists();
            return Page();
        }

        // Validaciones adicionales
        var employee = await _context.Employees.FindAsync(EmployeeId);
        if (employee == null)
        {
            ModelState.AddModelError("EmployeeId", "Employee not found");
            await LoadSelectLists();
            return Page();
        }

        var groupEmployee = await _context.GroupEmployees.FindAsync(GroupEmployeeId);
        if (groupEmployee == null)
        {
            ModelState.AddModelError("GroupEmployeeId", "Group Employee not found");
            await LoadSelectLists();
            return Page();
        }

        var questionnaire = await _context.Questionnaires.FindAsync(QuestionnaireId);
        if (questionnaire == null)
        {
            ModelState.AddModelError("QuestionnaireId", "Questionnaire not found");
            await LoadSelectLists();
            return Page();
        }

        var evaluation = new Evaluation
        {
            EmployeeId = EmployeeId,
            GroupEmployeeId = GroupEmployeeId,
            QuestionnaireId = QuestionnaireId,
            EvaluationDate = EvaluationDate
        };

        _context.Evaluations.Add(evaluation);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }

    private async Task LoadSelectLists()
    {
        // Cargar empleados
        Employees = new SelectList(
            await _context.Employees.OrderBy(e => e.FullName).ToListAsync(),
            "Id", "FullName");

        // Cargar GroupEmployees de manera simplificada
        var groupEmployeeItems = await _context.GroupEmployees
            .Include(ge => ge.EvaluationGroup)
            .Include(ge => ge.Employee)
            .Select(ge => new
            {
                Id = ge.Id,
                DisplayName = ge.EvaluationGroup.Name + " - " + ge.Employee.FullName
            })
            .ToListAsync();

        GroupEmployees = new SelectList(groupEmployeeItems, "Id", "DisplayName");

        // Cargar cuestionarios
        var questionnaireItems = await _context.Questionnaires
            .Include(q => q.EvaluationModel)
            .Include(q => q.Position)
            .Select(q => new
            {
                Id = q.Id,
                DisplayName = q.Title + " (" + q.EvaluationModel.Name + " - " + q.Position.Name + ")"
            })
            .ToListAsync();

        Questionnaires = new SelectList(questionnaireItems, "Id", "DisplayName");
    }
} 