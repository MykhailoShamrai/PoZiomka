namespace backend.Dto;

public class AddObligatoryPreferenceDto
{
    public required string NameOfForm { get; set; }
    public required string Name { get; set; }
    public required IEnumerable<string> Answers { get; set; }
}