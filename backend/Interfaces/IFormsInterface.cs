namespace backend.Interfaces;

public interface IFormsInterface
{   
    // Method for adding new forms with new questions (We assume, that one obligatory question can be present in only one form)
    // First parameter - name of a new form.
    // First collection represents new questions, second - number of answers for a question and last one - options to answer.
    public Task<bool> CreateNewForm(string NameOfForm, IEnumerable<string> Questions, IEnumerable<int> NumberOfAnswers, IEnumerable<string> Answers);

    public Task<bool> AddNewObligatoryQuestionToForm(string NameOfForm, string Question, IEnumerable<string> Answers);

    public Task<bool> DeleteQuestionFromForm(string NameOfForm, string Question);
    
}