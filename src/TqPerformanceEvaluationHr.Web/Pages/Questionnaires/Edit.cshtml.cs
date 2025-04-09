using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TqPerformanceEvaluationHr.Domain.Entities;
using TqPerformanceEvaluationHr.Infrastructure.Persistence;

namespace TqPerformanceEvaluationHr.Web.Pages.Questionnaires;

public class EditModel : PageModel
{
    private readonly AppDbContext _context;

    public EditModel(AppDbContext context)
    {
        _context = context;
        EvaluationModels = new SelectList(Enumerable.Empty<EvaluationModel>(), "Id", "Name");
        Positions = new SelectList(Enumerable.Empty<Position>(), "Id", "Name");
    }

    [BindProperty]
    public Questionnaire Questionnaire { get; set; } = null!;

    public SelectList EvaluationModels { get; set; }
    public SelectList Positions { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var questionnaire = await _context.Questionnaires
            .Include(q => q.Questions)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (questionnaire == null)
        {
            return NotFound();
        }

        Questionnaire = new Questionnaire
        {
            Id = questionnaire.Id,
            Title = questionnaire.Title,
            EvaluationModelId = questionnaire.EvaluationModelId,
            PositionId = questionnaire.PositionId,
            Questions = questionnaire.Questions.ToList()
        };
        
        await LoadSelectLists();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadSelectLists();
            return Page();
        }

        var existingQuestionnaire = await _context.Questionnaires
            .Include(q => q.Questions)
            .FirstOrDefaultAsync(q => q.Id == Questionnaire.Id);

        if (existingQuestionnaire == null)
        {
            return NotFound();
        }

        existingQuestionnaire.Title = Questionnaire.Title;
        existingQuestionnaire.EvaluationModelId = Questionnaire.EvaluationModelId;
        existingQuestionnaire.PositionId = Questionnaire.PositionId;

        if (Questionnaire.Questions != null)
        {
            var existingQuestionIds = Questionnaire.Questions
                .Where(q => q.Id > 0)
                .Select(q => q.Id)
                .ToList();

            var questionsToRemove = existingQuestionnaire.Questions
                .Where(q => !existingQuestionIds.Contains(q.Id))
                .ToList();

            foreach (var question in questionsToRemove)
            {
                _context.Questions.Remove(question);
            }

            foreach (var question in Questionnaire.Questions)
            {
                if (question.Id > 0)
                {
                    var existingQuestion = existingQuestionnaire.Questions
                        .FirstOrDefault(q => q.Id == question.Id);

                    if (existingQuestion != null)
                    {
                        existingQuestion.Text = question.Text;
                    }
                }
                else
                {
                    existingQuestionnaire.Questions.Add(new Question
                    {
                        Text = question.Text,
                        QuestionnaireId = existingQuestionnaire.Id
                    });
                }
            }
        }

        try
        {
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!QuestionnaireExists(Questionnaire.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
    }

    private async Task LoadSelectLists()
    {
        EvaluationModels = new SelectList(
            await _context.EvaluationModels.ToListAsync(),
            "Id", 
            "Name"
        );

        Positions = new SelectList(
            await _context.Positions.ToListAsync(),
            "Id", 
            "Name"
        );
    }

    private bool QuestionnaireExists(int id)
    {
        return _context.Questionnaires.Any(e => e.Id == id);
    }
} 