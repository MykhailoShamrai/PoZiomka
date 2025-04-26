namespace backend.Dto;

public class RoomOutDto
{
    public int Id { get; set; }
    public int Floor { get; set; }
    public int Number { get; set; }
    public int Capacity { get; set; }
    public RoomStatus Status { get; set; } = RoomStatus.Available;
    public List<int> ResidentsIds { get; set; } = new List<int>();
    public int FreePlaces { get; set; }
}