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
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public List<SelectListItem> EvaluationCycles { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "El nombre del grupo es requerido")]
            [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
            [Display(Name = "Nombre del Grupo")]
            public string Name { get; set; } = string.Empty;

            [Required(ErrorMessage = "El ciclo de evaluación es requerido")]
            [Display(Name = "Ciclo de Evaluación")]
            public int EvaluationCycleId { get; set; }

            [Required(ErrorMessage = "La fecha de inicio es requerida")]
            [DataType(DataType.Date)]
            [Display(Name = "Fecha de Inicio")]
            public DateTime StartDate { get; set; }

            [Required(ErrorMessage = "La fecha de fin es requerida")]
            [DataType(DataType.Date)]
            [Display(Name = "Fecha de Fin")]
            public DateTime EndDate { get; set; }
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

            // Convertir las fechas a UTC para la validación
            var startDateUtc = DateTime.SpecifyKind(Input.StartDate, DateTimeKind.Utc);
            var endDateUtc = DateTime.SpecifyKind(Input.EndDate, DateTimeKind.Utc);

            // Verificar que la fecha de inicio sea anterior a la fecha de fin
            if (startDateUtc >= endDateUtc)
            {
                ModelState.AddModelError(string.Empty, "La fecha de fin debe ser posterior a la fecha de inicio.");
                await LoadSelectListsAsync();
                return Page();
            }

            // Verificar que el ciclo de evaluación existe
            var evaluationCycle = await _context.EvaluationCycles.FindAsync(Input.EvaluationCycleId);
            if (evaluationCycle == null)
            {
                ModelState.AddModelError("Input.EvaluationCycleId", "El ciclo de evaluación seleccionado no existe.");
                await LoadSelectListsAsync();
                return Page();
            }

            // Verificar que las fechas estén dentro del rango del ciclo
            if (startDateUtc < evaluationCycle.StartDate || endDateUtc > evaluationCycle.EndDate)
            {
                ModelState.AddModelError(string.Empty, "Las fechas del grupo deben estar dentro del rango del ciclo de evaluación.");
                await LoadSelectListsAsync();
                return Page();
            }

            // Verificar que no exista un grupo con el mismo nombre en el mismo ciclo
            var existingGroup = await _context.EvaluationGroups
                .FirstOrDefaultAsync(g => 
                    g.EvaluationCycleId == Input.EvaluationCycleId && 
                    g.Name.ToLower() == Input.Name.ToLower());

            if (existingGroup != null)
            {
                ModelState.AddModelError("Input.Name", "Ya existe un grupo con este nombre en el ciclo seleccionado.");
                await LoadSelectListsAsync();
                return Page();
            }

            var evaluationGroup = new EvaluationGroup
            {
                Name = Input.Name,
                EvaluationCycleId = Input.EvaluationCycleId,
                StartDate = startDateUtc,
                EndDate = endDateUtc
            };

            _context.EvaluationGroups.Add(evaluationGroup);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private async Task LoadSelectListsAsync()
        {
            EvaluationCycles = await _context.EvaluationCycles
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.Year)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Year} ({c.StartDate:dd/MM/yyyy} - {c.EndDate:dd/MM/yyyy})"
                })
                .ToListAsync();
        }
    }
} 