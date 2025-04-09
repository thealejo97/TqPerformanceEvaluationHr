using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TqPerformanceEvaluationHr.Domain.Entities;
using TqPerformanceEvaluationHr.Infrastructure.Persistence;

namespace TqPerformanceEvaluationHr.Web.Pages.EvaluationGroups
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
            EvaluationCycles = new List<SelectListItem>();
            Input = new InputModel();
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public List<SelectListItem> EvaluationCycles { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "The group name is required")]
            [StringLength(100, ErrorMessage = "The group name must be between {2} and {1} characters", MinimumLength = 3)]
            [Display(Name = "Group Name")]
            public string Name { get; set; } = string.Empty;

            [Required(ErrorMessage = "The evaluation cycle is required")]
            [Display(Name = "Evaluation Cycle")]
            public int EvaluationCycleId { get; set; }

            [Required(ErrorMessage = "The start date is required")]
            [DataType(DataType.Date)]
            [Display(Name = "Start Date")]
            public DateTime StartDate { get; set; } = DateTime.Today;

            [Required(ErrorMessage = "The end date is required")]
            [DataType(DataType.Date)]
            [Display(Name = "End Date")]
            public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(1);
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

            // Verify that the evaluation cycle exists
            var evaluationCycle = await _context.EvaluationCycles.FindAsync(Input.EvaluationCycleId);
            if (evaluationCycle == null)
            {
                ModelState.AddModelError("Input.EvaluationCycleId", "The selected evaluation cycle does not exist");
                await LoadSelectListsAsync();
                return Page();
            }

            // Verify date range
            if (Input.StartDate >= Input.EndDate)
            {
                ModelState.AddModelError("Input.EndDate", "The end date must be after the start date");
                await LoadSelectListsAsync();
                return Page();
            }

            // Verify that the group name is unique for the cycle
            var existingGroup = await _context.EvaluationGroups
                .FirstOrDefaultAsync(g => g.Name == Input.Name && g.EvaluationCycleId == Input.EvaluationCycleId);
            
            if (existingGroup != null)
            {
                ModelState.AddModelError("Input.Name", "A group with this name already exists for the selected cycle");
                await LoadSelectListsAsync();
                return Page();
            }

            var group = new EvaluationGroup
            {
                Name = Input.Name,
                EvaluationCycleId = Input.EvaluationCycleId,
                StartDate = Input.StartDate,
                EndDate = Input.EndDate
            };

            _context.EvaluationGroups.Add(group);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private async Task LoadSelectListsAsync()
        {
            EvaluationCycles = await _context.EvaluationCycles
                .Where(c => c.IsActive)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Year.ToString()
                })
                .ToListAsync();
        }
    }
} 