using System.ComponentModel.DataAnnotations.Schema;

public class Answer
{
    public int AnswerId { get; set; }
    // For which form the answers are filled.
    public required Form CorrespondingForm { get; set;}
    // Answers for choosable preferences
    public int UserId { get; set; }
    public required List<OptionForQuestion> ChosenOptions{ get; set; }
    public AnswerStatus Status { get; set; } = AnswerStatus.Created;
}

public enum AnswerStatus
{
    Created,
    ReadyForFill,
    Editing,
    Saved
}