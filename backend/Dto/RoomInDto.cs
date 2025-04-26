namespace backend.Dto;

public class RoomInDto
{
    public int Floor { get; set; }
    public int Number { get; set; }
    public int Capacity { get; set; }
    public RoomStatus Status { get; set; } = RoomStatus.Available;
}