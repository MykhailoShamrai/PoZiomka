using System.ComponentModel;
using System.Threading.Tasks;
using backend.Data;
using backend.Interfaces;
using Microsoft.EntityFrameworkCore;

public class FormService : IFormsInterface
{
    private readonly AppDbContext _appDbContext;
    public FormService(AppDbContext AppDbContext)
    {
        _appDbContext = AppDbContext;
    }

    public Task<bool> CreateNewForm(string NameOfForm, IEnumerable<string> Questions, IEnumerable<int> NumberOfAnswers, IEnumerable<string> Answers)
    {
        // Firstly, chack if number validation of arguments
        if (Questions.Count() != NumberOfAnswers.Count())
        {
            throw new InvalidDataException("Number of questions and collection of number of answers for it have different sizes!");
        }
        // After, check if Form does already exist in a database.
        throw new NotImplementedException();

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

    private bool CheckIfNumberOfAnswersIsSameAsDeclared(IEnumerable<int> NumberOfAnswers, IEnumerable<string> Answers)
    {
        int sum  = 0;
        foreach (int number in NumberOfAnswers)
        {
            sum += number;
        }
        return Answers.Count() == sum;
    }
}