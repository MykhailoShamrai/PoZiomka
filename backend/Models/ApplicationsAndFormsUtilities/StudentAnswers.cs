public class StudentAnswers
{
    public int StudentAnswersId { get; set; }
    public int UserId { get; set; }
    public required IEnumerable<Answer> Answers { get; set; }
}
