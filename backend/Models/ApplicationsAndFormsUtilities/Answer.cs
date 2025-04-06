public class Answer
{
    public int Id { get; set; }
    // For which form the answers are filled.
    public required Form CorrespondingForm { get; set;}
    // Answers for choosable preferences
    public IEnumerable<ChoosablePreference>? Choosable { get; set; }
    public required IEnumerable<OptionForObligatoryPreference> Obligatory { get; set; }
    public AnswerStatus Status { get; set; } = AnswerStatus.Created;
    public StudentAnswers StudentAnswers { get; set; } = null!;
}

public enum AnswerStatus
{
    Created,
    ReadyForFill,
    Editing,
    Saved
}