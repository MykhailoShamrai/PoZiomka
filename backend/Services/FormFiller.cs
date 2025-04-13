using System.Reflection.Metadata.Ecma335;
using backend.Data;
using backend.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

public class FormFiller
{
    // private readonly AppDbContext _appDbContext;

    // public FormFiller(AppDbContext appDbContext)
    // {
    //     _appDbContext = appDbContext;
    // }

    // public async void FillForm(AnswerDto answer)
    // {
    //     var form = await _appDbContext.Forms
    //         .FirstOrDefaultAsync(f => f.FormId == answer.FormId);
    //     if (form == null)
    //         return;

    //     var chosenOptions = await _appDbContext.OptionsForQuestions
    //         .Where(o => answer.ChosenOptionIds.Contains(o.OptionForQuestionId))
    //         .ToListAsync();

    //     var answerEntity = new Answer
    //     {
    //         CorrespondingForm = form,
    //         UserId = answer.UserId,
    //         ChosenOptions = chosenOptions,
    //         Status = answer.Status
    //     };

    //     _appDbContext.Answers.Add(answerEntity);
    //     await _appDbContext.SaveChangesAsync();
    // }
}
