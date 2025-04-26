using backend.Models.User;

public class Room
{
    public int Id { get; set; }
    public int Floor { get; set; }
    public int Number { get; set; }
    public int Capacity { get; set; }
    public List<int> ResidentsIds { get; set; } = new List<int>();
    public RoomStatus Status { get; set; }
}

public enum RoomStatus
{
    Available,
    Unavailable,
    Renovation,
    Cleaning
}
