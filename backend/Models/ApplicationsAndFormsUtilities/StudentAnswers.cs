public class StudentAnswers
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required IEnumerable<Answer> Answers { get; set; }
}
