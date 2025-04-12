namespace backend.Dto;

public class AddObligatoryPreferenceDto
{
    public string NameOfForm { get; set; }
    public string Name { get; set; }
    public IEnumerable<string> Answers { get; set; }
}