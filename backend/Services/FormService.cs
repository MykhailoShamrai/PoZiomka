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

    public async Task<Form?> FindFormWithQuestions(int formId)
    {
        return await _appDbContext.Forms
            .Where(f => f.FormId == formId)
            .Include(f => f.Questions)
            .FirstAsync();
    }
    
    public async Task<List<Question>> FindQuestionsForForm(int formId)
    {
        return await _appDbContext.Questions
            .Where(q => q.FormForWhichCorrespond!.FormId == formId)
            .ToListAsync();
    }

    private async Task<Question?> FindQuestionWithAnswers(string questionName)
    {
        return await _appDbContext.Questions
            .Include(o => o.Options)
            .FirstOrDefaultAsync(o => o.Name.ToLower() == questionName.ToLower());
    }

    private async Task<List<Question>> FindQuestionsWithAnswers()
    {
        return await _appDbContext.Questions
            .Include(o => o.Options)
            .ToListAsync();
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

    private async Task<HashSet<Question>> FindQuestionsForOptions(List<OptionForQuestion> options)
    {
        List<Question> questions = await FindQuestionsWithAnswers();
        Dictionary<int, Question> tmpDict = new Dictionary<int, Question>();
        foreach (var question in questions) 
        {
            tmpDict.Add(question.QuestionId, question);
        }
        HashSet<Question> resultSet = new HashSet<Question>(questions.Count);
        foreach (var option in options)
        {
            if (tmpDict.ContainsKey(option.Question.QuestionId))
            {
                resultSet.Add(tmpDict[option.Question.QuestionId]);
            }
        }
        return resultSet;
    }

    public async Task<AnswerStatus> FindStatusForAnswer(List<OptionForQuestion> options, Form form)
    {
        // If form was found without questions for it
        // Here we assume that it isn't null
        List<Question> questions;
        if (form!.Questions is null)
            questions = await FindQuestionsForForm(form.FormId);
        else
            questions = form.Questions;

        var setOfQuestionsWithAnswers = await FindQuestionsForOptions(options);
        if (setOfQuestionsWithAnswers.Count == options.Count)
            return AnswerStatus.Saved;
        else return AnswerStatus.Editing;
    }
}