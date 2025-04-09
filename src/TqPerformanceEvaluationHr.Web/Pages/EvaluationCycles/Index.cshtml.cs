using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqPerformanceEvaluationHr.Domain.Entities;
using TqPerformanceEvaluationHr.Infrastructure.Persistence;

namespace TqPerformanceEvaluationHr.Web.Pages.EvaluationCycles
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<EvaluationCycle> EvaluationCycles { get; set; } = new List<EvaluationCycle>();

        public async Task OnGetAsync()
        {
            EvaluationCycles = await _context.EvaluationCycles
                .OrderByDescending(c => c.Year)
                .ToListAsync();
        }
    }
} 