public class Question
{
    public int QuestionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Form? FormForWhichCorrespond { get; set;} 
    public List<OptionForQuestion> Options { get; set;} = new List<OptionForQuestion>();
    public bool IsObligatory { get; set; }
}
