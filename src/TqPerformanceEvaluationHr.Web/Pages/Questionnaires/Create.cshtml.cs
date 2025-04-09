using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using TqPerformanceEvaluationHr.Domain.Entities;
using TqPerformanceEvaluationHr.Infrastructure.Persistence;

namespace TqPerformanceEvaluationHr.Web.Pages.Questionnaires;

public class CreateModel : PageModel
{
    private readonly AppDbContext _context;

    public CreateModel(AppDbContext context)
    {
        _context = context;
        
        EvaluationModels = new SelectList(Enumerable.Empty<EvaluationModel>(), "Id", "Name");
        Positions = new SelectList(Enumerable.Empty<Position>(), "Id", "Name");
    }

    
    [BindProperty]
    public string Title { get; set; } = string.Empty;
    
    [BindProperty]
    public int EvaluationModelId { get; set; }
    
    [BindProperty]
    public int PositionId { get; set; }

    public SelectList EvaluationModels { get; set; }
    public SelectList Positions { get; set; }

    public async Task OnGetAsync()
    {
        
        await LoadSelectLists();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            
            var isValid = true;
            
            if (string.IsNullOrEmpty(Title))
            {
                ModelState.AddModelError("Title", "The Title field is required.");
                isValid = false;
            }
            
            if (EvaluationModelId <= 0)
            {
                ModelState.AddModelError("EvaluationModelId", "Please select an Evaluation Model.");
                isValid = false;
            }
            
            if (PositionId <= 0)
            {
                ModelState.AddModelError("PositionId", "Please select a Position.");
                isValid = false;
            }
            
            if (!isValid)
            {
                await LoadSelectLists();
                return Page();
            }
            
            
            var questionnaire = new Questionnaire
            {
                Title = Title,
                EvaluationModelId = EvaluationModelId,
                PositionId = PositionId
            };
            
            
            var evaluationModel = await _context.EvaluationModels.FindAsync(EvaluationModelId);
            var position = await _context.Positions.FindAsync(PositionId);
            
            if (evaluationModel == null)
            {
                ModelState.AddModelError("EvaluationModelId", "The selected Evaluation Model does not exist.");
                await LoadSelectLists();
                return Page();
            }
            
            if (position == null)
            {
                ModelState.AddModelError("PositionId", "The selected Position does not exist.");
                await LoadSelectLists();
                return Page();
            }
            
            
            _context.Questionnaires.Add(questionnaire);
            await _context.SaveChangesAsync();
            
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            
            
            ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            await LoadSelectLists();
            return Page();
        }
    }

    private async Task LoadSelectLists()
    {
        var evaluationModels = await _context.EvaluationModels.ToListAsync();
        EvaluationModels = new SelectList(evaluationModels, "Id", "Name");
        
        var positions = await _context.Positions.ToListAsync();
        Positions = new SelectList(positions, "Id", "Name");
        
    }
}
