public class Room
{
    public int Id { get; set; }
    public int Floor { get; set; }
    public int Number { get; set; }
    public int Capacity { get; set; }
    public List<User> Residents { get; set; } = new List<User>();
}
