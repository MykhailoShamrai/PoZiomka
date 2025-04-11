namespace backend.Dto;

public class ObligatoryPreferenceDto
{
    public string Name { get; set; }
    public IEnumerable<string> Answers { get; set; }
}