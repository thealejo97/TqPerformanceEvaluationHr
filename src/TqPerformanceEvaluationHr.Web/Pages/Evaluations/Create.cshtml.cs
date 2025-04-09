using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TqPerformanceEvaluationHr.Domain.Entities;
using TqPerformanceEvaluationHr.Infrastructure.Persistence;

namespace TqPerformanceEvaluationHr.Web.Pages.Evaluations;

public class CreateModel : PageModel
{
    private readonly AppDbContext _context;

    public CreateModel(AppDbContext context)
    {
        _context = context;
        Employees = new List<SelectListItem>();
        EvaluationCycles = new List<SelectListItem>();
        Questionnaires = new List<SelectListItem>();
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<SelectListItem> Employees { get; set; }
    public List<SelectListItem> EvaluationCycles { get; set; }
    public List<SelectListItem> Questionnaires { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "The employee is required")]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "The evaluation cycle is required")]
        [Display(Name = "Evaluation Cycle")]
        public int EvaluationCycleId { get; set; }

        [Required(ErrorMessage = "The questionnaire is required")]
        [Display(Name = "Questionnaire")]
        public int QuestionnaireId { get; set; }

        [Required(ErrorMessage = "The evaluation date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Evaluation Date")]
        public DateTime EvaluationDate { get; set; } = DateTime.Today;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadSelectListsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadSelectListsAsync();
            return Page();
        }

        // Verify that the employee exists
        var employee = await _context.Employees.FindAsync(Input.EmployeeId);
        if (employee == null)
        {
            ModelState.AddModelError("Input.EmployeeId", "The selected employee does not exist");
            await LoadSelectListsAsync();
            return Page();
        }

        // Verify that the evaluation cycle exists
        var evaluationCycle = await _context.EvaluationCycles.FindAsync(Input.EvaluationCycleId);
        if (evaluationCycle == null)
        {
            ModelState.AddModelError("Input.EvaluationCycleId", "The selected evaluation cycle does not exist");
            await LoadSelectListsAsync();
            return Page();
        }

        // Verify that the questionnaire exists
        var questionnaire = await _context.Questionnaires.FindAsync(Input.QuestionnaireId);
        if (questionnaire == null)
        {
            ModelState.AddModelError("Input.QuestionnaireId", "The selected questionnaire does not exist");
            await LoadSelectListsAsync();
            return Page();
        }

        var evaluation = new Evaluation
        {
            EmployeeId = Input.EmployeeId,
            QuestionnaireId = Input.QuestionnaireId,
            EvaluationDate = Input.EvaluationDate
        };

        _context.Evaluations.Add(evaluation);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }

    private async Task LoadSelectListsAsync()
    {
        Employees = await _context.Employees
            .Select(e => new SelectListItem
            {
                Value = e.Id.ToString(),
                Text = e.FullName
            })
            .ToListAsync();

        EvaluationCycles = await _context.EvaluationCycles
            .Where(c => c.IsActive)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Year.ToString()
            })
            .ToListAsync();

        Questionnaires = await _context.Questionnaires
            .Select(q => new SelectListItem
            {
                Value = q.Id.ToString(),
                Text = q.Title
            })
            .ToListAsync();
    }
} 