using backend.Dto;

namespace backend.Interfaces;

public interface IFormsInterface
{   
    // Method for adding new forms with new questions (We assume, that one obligatory question can be present in only one form)
    // First parameter - name of a new form.
    // First collection represents new questions, second - number of answers for a question and last one - options to answer.
    public Task<bool> CreateNewForm(FormDto formDto);

    public Task<bool> AddNewObligatoryQuestionToForm(AddQuestionDto dto);

    public Task<bool> DeleteQuestion(string question);

    public Task<bool> DeleteForm(string nameOfForm);
    public Task<Form[]> GetAll();
    public Task<Form?> FindForm(string formName);
    public Task<Form?> FindForm(int id);
    public Task<List<OptionForQuestion>> FindOptions(List<int> ids);
    public Task<AnswerStatus> FindStatusForAnswer(List<OptionForQuestion> options, Form form);
    
}