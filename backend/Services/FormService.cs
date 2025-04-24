using backend.Data;
using backend.Dto;
using backend.Interfaces;
using backend.Mappers;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class FormService : IFormsInterface
{
    private readonly AppDbContext _appDbContext;
    public FormService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<bool> CreateNewForm(FormDto formDto)
    {
        // Error handling is on controller level
        var form = formDto.DtoToForm();
        var ftmp = await FindForm(form.NameOfForm);
        if (ftmp is not null)
            return false;
        await _appDbContext.AddAsync(form);
        var res = await _appDbContext.SaveChangesAsync();
        return res > 0;   
    }

    public async Task<bool> AddNewObligatoryQuestionToForm(AddQuestionDto dto)
    {
        var form = await FindFormWithQuestions(dto.NameOfForm);
        if (form is null)
            throw new ArgumentException("There is no form with provided name!");
        var tmpQuestion = new Question
        {
            Name = dto.Name,
            FormForWhichCorrespond = form,
            IsObligatory = dto.IsObligatory
        };
        // Can something change here asynchronuosly?
        foreach (var answ in dto.Answers)
        {
            tmpQuestion.Options.Add(new OptionForQuestion
            {
                Name = answ,
                Question = tmpQuestion
            });
        }
        form.Questions.Add(tmpQuestion);

        await _appDbContext.AddAsync(tmpQuestion);
        var res = await _appDbContext.SaveChangesAsync();
        return res > 0;
    }

    public async Task<bool> DeleteForm(string nameOfForm)
    {
        var form = await FindForm(nameOfForm);
        if (form is null)
            throw new ArgumentException("There is no form with provided name!");
        var questions = _appDbContext.Questions
            .Where(x => x.FormForWhichCorrespond!.NameOfForm == form.NameOfForm);
        _appDbContext.Questions.RemoveRange(questions);
        _appDbContext.Forms.Remove(form);
        var res = await _appDbContext.SaveChangesAsync();
        return res > 0;
    }


    public async Task<bool> DeleteQuestion(string question)
    {
        var tmpQuestion = await FindQuestionWithAnswers(question);
        if (tmpQuestion is null)
            throw new ArgumentException("There is no question with such name!");
        _appDbContext.OptionsForQuestions.RemoveRange(tmpQuestion.Options);
        _appDbContext.Questions.Remove(tmpQuestion);
        var res = await _appDbContext.SaveChangesAsync();
        return res > 0;
    }

    public async Task<Form[]> GetAll()
    {
        return await _appDbContext.Forms
            .Include(f => f.Questions)
            .ThenInclude(q => q.Options)
            .ToArrayAsync();
    }
    
    private async Task<Question?> FindQuestionWithAnswers(string questionName)
    {
        return await _appDbContext.Questions
            .Include(o => o.Options)
            .FirstOrDefaultAsync(o => o.Name.ToLower() == questionName.ToLower());
    }

    public async Task<Form?> FindForm(string formName)
    {
        return await _appDbContext.Forms
            .FirstOrDefaultAsync(f => f.NameOfForm == formName);
    }

    public async Task<Form?> FindForm(int id)
    {
        return await _appDbContext.Forms
            .FirstOrDefaultAsync(f => f.FormId == id);
    }

    private async Task<Form?> FindFormWithQuestions(string formName)
    {
        return await _appDbContext.Forms
            .Include(f => f.Questions)
            .ThenInclude(o => o.Options)
            .FirstOrDefaultAsync(f => f.NameOfForm == formName);
    }

    public async Task<List<OptionForQuestion>> FindOptions(List<int> ids)
    {
        return await _appDbContext.OptionsForQuestions
            .Where(o => ids.Contains(o.OptionForQuestionId))
            .ToListAsync();
    }

    public Task<AnswerStatus> FindStatusForAnswer(List<OptionForQuestion> options, Form form)
    {
        throw new NotImplementedException();
    }
}