public class ObligatoryPreference
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public required List<string> Options { get; set; }
    public int Chosen { get; set; }
}
