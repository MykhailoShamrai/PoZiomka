namespace backend.Dto;

public class FormCreateDto
{
    public string FormName {get; set;} = string.Empty;
    public IEnumerable<string> Questions {get; set;} = new List<string>();
    public IEnumerable<bool> Obligatoriness {get; set;} = new List<bool>();
    public IEnumerable<int> NumberOfOptions {get; set;} = new List<int>();
    public IEnumerable<string> Options {get; set;} = new List<string>();
}