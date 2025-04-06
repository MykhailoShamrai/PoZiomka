public class ObligatoryPreference
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public required Form FormForWhichCorrespond { get; set;}
    public required IEnumerable<OptionForObligatoryPreference> Options { get; set;}
}
