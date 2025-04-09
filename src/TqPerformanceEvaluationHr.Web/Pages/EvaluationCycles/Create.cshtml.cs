using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TqPerformanceEvaluationHr.Domain.Entities;
using TqPerformanceEvaluationHr.Infrastructure.Persistence;

namespace TqPerformanceEvaluationHr.Web.Pages.EvaluationCycles
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "The year is required")]
            [Range(2000, 2100, ErrorMessage = "The year must be between 2000 and 2100")]
            [Display(Name = "Year")]
            public int Year { get; set; }

            [Required(ErrorMessage = "The start date is required")]
            [DataType(DataType.Date)]
            [Display(Name = "Start Date")]
            public DateTime StartDate { get; set; }

            [Required(ErrorMessage = "The end date is required")]
            [DataType(DataType.Date)]
            [Display(Name = "End Date")]
            public DateTime EndDate { get; set; }

            [Display(Name = "Active")]
            public bool IsActive { get; set; }
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Convertir las fechas a UTC para la validación
            var startDateUtc = DateTime.SpecifyKind(Input.StartDate, DateTimeKind.Utc);
            var endDateUtc = DateTime.SpecifyKind(Input.EndDate, DateTimeKind.Utc);

            // Check if a cycle already exists for this year
            var existingCycle = await _context.EvaluationCycles
                .FirstOrDefaultAsync(c => c.Year == Input.Year);

            if (existingCycle != null)
            {
                ModelState.AddModelError("Input.Year", "A cycle already exists for this year");
                return Page();
            }

            // Verify that start date is before end date
            if (startDateUtc >= endDateUtc)
            {
                ModelState.AddModelError(string.Empty, "The start date must be before the end date");
                return Page();
            }

            // Verify that no other cycle overlaps with this one
            var overlappingCycle = await _context.EvaluationCycles
                .FirstOrDefaultAsync(c =>
                    (startDateUtc <= c.EndDate && endDateUtc >= c.StartDate));

            if (overlappingCycle != null)
            {
                ModelState.AddModelError(string.Empty, "This cycle overlaps with an existing cycle");
                return Page();
            }

            // If this cycle is set as active, deactivate all other cycles
            if (Input.IsActive)
            {
                var activeCycles = await _context.EvaluationCycles
                    .Where(c => c.IsActive)
                    .ToListAsync();

                foreach (var cycle in activeCycles)
                {
                    cycle.IsActive = false;
                }
            }

            var evaluationCycle = new EvaluationCycle
            {
                Year = Input.Year,
                StartDate = startDateUtc,
                EndDate = endDateUtc,
                IsActive = Input.IsActive
            };

            _context.EvaluationCycles.Add(evaluationCycle);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
} 