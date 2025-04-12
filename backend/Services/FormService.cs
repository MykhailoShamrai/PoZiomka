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

    public async Task<bool> AddNewObligatoryQuestionToForm(AddObligatoryPreferenceDto dto)
    {
        var form = await FindFormWithQuestions(dto.NameOfForm);
        if (form is null)
            throw new ArgumentException("There is no form with provided name!");
        var newObligatoryPreference = new ObligatoryPreference
        {
            Name = dto.Name,
            FormForWhichCorrespond = form,
        };
        // Can something change here asynchronuosly?
        foreach (var answ in dto.Answers)
        {
            newObligatoryPreference.Options.Append(new OptionForObligatoryPreference
            {
                Preference = newObligatoryPreference,
                OptionForPreference = answ
            });
        }
        form.Obligatory.Add(newObligatoryPreference);

        await _appDbContext.AddAsync(newObligatoryPreference);
        var res = await _appDbContext.SaveChangesAsync();
        return res > 0;
    }

    public async Task<bool> DeleteForm(string nameOfForm)
    {
        var form = await FindForm(nameOfForm);
        if (form is null)
            throw new ArgumentException("There is no form with provided name!");
        var obligatoryQuestions = _appDbContext.ObligatoryPreferences
            .Where(x => x.FormForWhichCorrespond!.NameOfForm == form.NameOfForm);
        _appDbContext.ObligatoryPreferences.RemoveRange(obligatoryQuestions);
        _appDbContext.Forms.Remove(form);
        var res = await _appDbContext.SaveChangesAsync();
        return res > 0;
    }


    public async Task<bool> DeleteQuestion(string question)
    {
        var oblQuestion = await FindQuestionWithAnswers(question);
        if (oblQuestion is null)
            throw new ArgumentException("There is no question with such name!");
        _appDbContext.OptionsForObligatoryPreferences.RemoveRange(oblQuestion.Options);
        _appDbContext.ObligatoryPreferences.Remove(oblQuestion);
        var res = await _appDbContext.SaveChangesAsync();
        return res > 0;
    }
    
    private async Task<ObligatoryPreference?> FindQuestion(string questionName)
    {
        return await _appDbContext.ObligatoryPreferences
            .FirstOrDefaultAsync(o => o.Name.ToLower() == questionName.ToLower());
    }

    private async Task<ObligatoryPreference?> FindQuestionWithAnswers(string questionName)
    {
        return await _appDbContext.ObligatoryPreferences
            .Include(o => o.Options)
            .FirstOrDefaultAsync(o => o.Name.ToLower() == questionName.ToLower());
    }

    private async Task<Form?> FindForm(string formName)
    {
        return await _appDbContext.Forms
            .FirstOrDefaultAsync(f => f.NameOfForm.ToLower() == formName.ToLower());
    }

    private async Task<Form?> FindFormWithQuestions(string formName)
    {
        return await _appDbContext.Forms
            .Include(f => f.Obligatory)
            .ThenInclude(o => o.Options)
            .FirstOrDefaultAsync(f => f.NameOfForm.ToLower() == formName.ToLower());
    }
}