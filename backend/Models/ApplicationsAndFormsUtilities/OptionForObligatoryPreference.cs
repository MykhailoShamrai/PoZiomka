public class OptionForObligatoryPreference
{
    public int OptionForObligatoryPreferenceId { get; set; }
    public required ObligatoryPreference Preference { get; set; }
    public required string OptionForPreference { get; set; }
    public IEnumerable<Answer> AnswersWhichContains { get; set; } = new List<Answer>();
}