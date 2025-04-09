using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TqPerformanceEvaluationHr.Domain.Entities;
using TqPerformanceEvaluationHr.Infrastructure.Persistence;

namespace TqPerformanceEvaluationHr.Web.Pages.EvaluationCycles
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "El año es requerido")]
            [Range(2000, 2100, ErrorMessage = "El año debe estar entre 2000 y 2100")]
            [Display(Name = "Año")]
            public int Year { get; set; }

            [Required(ErrorMessage = "La fecha de inicio es requerida")]
            [DataType(DataType.Date)]
            [Display(Name = "Fecha de Inicio")]
            public DateTime StartDate { get; set; }

            [Required(ErrorMessage = "La fecha de fin es requerida")]
            [DataType(DataType.Date)]
            [Display(Name = "Fecha de Fin")]
            public DateTime EndDate { get; set; }

            [Display(Name = "Activo")]
            public bool IsActive { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evaluationCycle = await _context.EvaluationCycles.FindAsync(id);
            if (evaluationCycle == null)
            {
                return NotFound();
            }

            Input = new InputModel
            {
                Id = evaluationCycle.Id,
                Year = evaluationCycle.Year,
                StartDate = evaluationCycle.StartDate,
                EndDate = evaluationCycle.EndDate,
                IsActive = evaluationCycle.IsActive
            };

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

            // Verificar que no exista otro ciclo para el mismo año (excluyendo el actual)
            var existingCycle = await _context.EvaluationCycles
                .FirstOrDefaultAsync(c => c.Year == Input.Year && c.Id != Input.Id);

            if (existingCycle != null)
            {
                ModelState.AddModelError("Input.Year", "Ya existe un ciclo de evaluación para este año.");
                return Page();
            }

            // Verificar que la fecha de inicio sea anterior a la fecha de fin
            if (startDateUtc >= endDateUtc)
            {
                ModelState.AddModelError(string.Empty, "La fecha de fin debe ser posterior a la fecha de inicio.");
                return Page();
            }

            // Verificar que no se solape con otros ciclos (excluyendo el actual)
            var overlappingCycle = await _context.EvaluationCycles
                .FirstOrDefaultAsync(c =>
                    c.Id != Input.Id &&
                    (startDateUtc <= c.EndDate && endDateUtc >= c.StartDate));

            if (overlappingCycle != null)
            {
                ModelState.AddModelError(string.Empty, "El ciclo se solapa con otro ciclo existente.");
                return Page();
            }

            var evaluationCycle = await _context.EvaluationCycles.FindAsync(Input.Id);
            if (evaluationCycle == null)
            {
                return NotFound();
            }

            // Si este ciclo se establece como activo, desactivar todos los demás
            if (Input.IsActive)
            {
                var activeCycles = await _context.EvaluationCycles
                    .Where(c => c.IsActive && c.Id != Input.Id)
                    .ToListAsync();

                foreach (var cycle in activeCycles)
                {
                    cycle.IsActive = false;
                }
            }

            evaluationCycle.Year = Input.Year;
            evaluationCycle.StartDate = startDateUtc;
            evaluationCycle.EndDate = endDateUtc;
            evaluationCycle.IsActive = Input.IsActive;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EvaluationCycleExistsAsync(Input.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private async Task<bool> EvaluationCycleExistsAsync(int id)
        {
            return await _context.EvaluationCycles.AnyAsync(e => e.Id == id);
        }
    }
} 