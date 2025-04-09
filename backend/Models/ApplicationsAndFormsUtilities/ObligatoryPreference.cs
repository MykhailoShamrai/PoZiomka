public class ObligatoryPreference
{
    public int ObligatoryPreferenceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Form? FormForWhichCorrespond { get; set;} 
    public IEnumerable<OptionForObligatoryPreference> Options { get; set;} = new List<OptionForObligatoryPreference>();
}
