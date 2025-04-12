    /// <summary>
    /// Class that represents a prototype of form, that means there are different types of such forms.
    /// Student answer must contain answers for specified form.
    /// </summary>
    public class Form
    {
        public int FormId { get; set; }
        public required string NameOfForm { get; set; }
        public required List<Question> Questions { get; set; }
    }
