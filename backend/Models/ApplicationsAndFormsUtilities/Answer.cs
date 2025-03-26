public class Answer
{
    public int Id { get; set; }
    public List<ChoosablePreference>? Choosable { get; set; }
    public required List<ObligatoryPreference> Obligatory { get; set; }
}
