namespace backend.Dto;

public class RoomDto
{
    public int RoomId { get; set; }
    public int Floor { get; set; }
    public int Number { get; set; }
    public int Capacity { get; set; }
    public int AvailablePlaces { get; set; }
    public RoomStatus Status { get; set; }
}