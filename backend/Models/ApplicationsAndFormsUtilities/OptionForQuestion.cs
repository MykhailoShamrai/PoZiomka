public class OptionForQuestion
{
    public int OptionForQuestionId { get; set; }
    public required Question Question { get; set; }
    public required string Name { get; set; }
    public List<Answer> AnswersWhichContains { get; set; } = new List<Answer>();
}