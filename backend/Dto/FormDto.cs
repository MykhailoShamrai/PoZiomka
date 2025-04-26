using backend.Dto;

public class FormDto
    {
        public int FormId { get; set; }
        public required string NameOfForm { get; set; }
        public required List<Question> Questions { get; set; }
        public AnswerDto? Answers { get; set; }
    }