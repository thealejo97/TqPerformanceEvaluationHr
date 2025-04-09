using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqPerformanceEvaluationHr.Domain.Entities;
using TqPerformanceEvaluationHr.Infrastructure.Persistence;

namespace TqPerformanceEvaluationHr.Web.Pages.EvaluationResults
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
            EvaluationResults = new List<EvaluationResult>();
        }

        public IList<EvaluationResult> EvaluationResults { get; set; }

        public async Task OnGetAsync()
        {
            EvaluationResults = await _context.EvaluationResults
                .Include(er => er.Evaluation)
                    .ThenInclude(e => e.Employee)
                .Include(er => er.Evaluation)
                    .ThenInclude(e => e.GroupEmployee)
                        .ThenInclude(ge => ge.EvaluationGroup)
                .OrderByDescending(er => er.Evaluation.EvaluationDate)
                .ToListAsync();
        }
    }
} 