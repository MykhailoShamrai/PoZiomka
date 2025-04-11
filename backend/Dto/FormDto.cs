namespace backend.Dto;

public class FormDto
{
    public string FormName {get; set;} = string.Empty;
    public IEnumerable<string> Questions {get; set;} = new List<string>();
    public IEnumerable<int> NumberOfAnswers {get; set;} = new List<int>();
    public IEnumerable<string> Answers {get; set;} = new List<string>();
}