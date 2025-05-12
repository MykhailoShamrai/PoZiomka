using backend.Data;
using backend.Dto;
using backend.Interfaces;
using backend.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace backend.Services;

public class FormService : IFormsInterface
{
    private readonly AppDbContext _appDbContext;
    public FormService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<bool> CreateNewForm(FormCreateDto formDto)
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
        var form = await FindFormWithQuestions(dto.FormName);
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


    public async Task<bool> DeleteQuestion(DeleteQuestionDto deleteQuestionDto)
    {
        var form = await _appDbContext.Forms
            .Include(f => f.Questions)
            .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(f => f.NameOfForm == deleteQuestionDto.FormName);

        if (form == null)
            throw new ArgumentException("There is no form with such name!");

        var tmpQuestion = form.Questions.FirstOrDefault(q => q.Name == deleteQuestionDto.QuestionName);
        if (tmpQuestion == null)
            throw new ArgumentException("There is no question with such name in the specified form!");

        _appDbContext.OptionsForQuestions.RemoveRange(tmpQuestion.Options);
        form.Questions.Remove(tmpQuestion);

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

    public async Task<Answer> GetAnswers(int userId, int formId)
    {
        return await _appDbContext.Answers
                    .Where(a => a.UserId == userId && a.CorrespondingForm.FormId == formId)
                    .Include(a => a.ChosenOptions)
                    .ThenInclude(o => o.Question)
                    .FirstOrDefaultAsync();
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

    private async Task<List<Question>> FindQuestions()
    {
        return await _appDbContext.Questions
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
            .Include(o => o.Question)
            .ToListAsync();
    }

    private async Task<List<Question>> FindQuestionsForOptions(List<OptionForQuestion> options)
    {
        return await _appDbContext.Questions
            .Where(q => options.Select(opt => opt.Question.QuestionId)
            .Contains(q.QuestionId))
            .Include(q => q.FormForWhichCorrespond)
            .ToListAsync();
    }


    public async Task<AnswerStatus> FindStatusForAnswer(List<OptionForQuestion> options, Form form)
    {
        // If form was found without questions for it
        // Here we assume that it isn't null
        List<Question> questions;
        questions = await FindQuestionsForForm(form.FormId);

        var listOfQuestionsWithAnswers = await FindQuestionsForOptions(options);
        var questonsFormNotThisForm = listOfQuestionsWithAnswers.Where(q => q.FormForWhichCorrespond!.FormId != form.FormId).ToList();
        if (questonsFormNotThisForm.Count == 0)
        {
            if (listOfQuestionsWithAnswers.Count == questions.Count)
                return AnswerStatus.Saved;
            else return AnswerStatus.Editing;
        }
        throw new ArgumentException();
    }

    public async Task<int> SaveAnswer(AnswerDto dto, AnswerStatus status, Form form, int userId, List<OptionForQuestion> chosenOptions)
    {
        // Find answers that from this users that have status Editing
        Answer? answerThatIsntComplete = await _appDbContext.Answers
            .Where(a => a.UserId == userId && a.CorrespondingForm.FormId == form.FormId && a.Status == AnswerStatus.Editing)
            .Include(a => a.ChosenOptions)
            .FirstOrDefaultAsync();

        if (answerThatIsntComplete is null)
        {
            var answer = new Answer
            {
                CorrespondingForm = form,
                ChosenOptions = chosenOptions,
                UserId = userId,
                Status = status
            };
            _appDbContext.Answers.Add(answer);
        }
        else
        {
            answerThatIsntComplete.ChosenOptions = chosenOptions;
            answerThatIsntComplete.Status = status;
        }
        int res = await _appDbContext.SaveChangesAsync();
        return res;
    }
}