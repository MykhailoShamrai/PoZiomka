public class OptionForObligatoryPreference
{
    public int Id { get; set; }
    public required ObligatoryPreference Preference { get; set; }
    public required string OptionForPreference { get; set; }
    public required IEnumerable<Answer> AnswersWhichContains { get; set; }
}