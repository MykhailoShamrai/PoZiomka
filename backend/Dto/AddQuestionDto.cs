namespace backend.Dto;

public class AddQuestionDto
{
    public required string FormName { get; set; }
    public required string Name { get; set; }
    public required bool IsObligatory { get; set; }
    public required IEnumerable<string> Answers { get; set; }
}