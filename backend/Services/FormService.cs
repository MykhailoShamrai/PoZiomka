using backend.Data;
using backend.Dto;
using backend.Interfaces;
using backend.Mappers;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class FormService : IFormsInterface
{
    private readonly AppDbContext _appDbContext;
    public FormService(AppDbContext AppDbContext)
    {
        _appDbContext = AppDbContext;
    }

    public async Task<bool> CreateNewForm(FormDto formDto)
    {
        // Error handling is on controller level
        var form = formDto.DtoToForm();
        await _appDbContext.AddAsync(form);
        var res = await _appDbContext.SaveChangesAsync();
        return res > 0;   
    }

    public Task<bool> AddNewObligatoryQuestionToForm(string NameOfForm, string Question, IEnumerable<string> Answers)
    {
        throw new NotImplementedException();
    }


    public Task<bool> DeleteQuestionFromForm(string NameOfForm, string Question)
    {
        throw new NotImplementedException();
    }

    private async Task<Form?> FindForm(string FormName)
    {
        var form = await _appDbContext.Forms.Where(f => f.NameOfForm == FormName).ToListAsync();
        if (form.Count > 0)
        {
            return form[0];
        }
        return null;
    }

}