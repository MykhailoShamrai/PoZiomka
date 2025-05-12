using backend.Mappers;

public class RoomOutDtoMapperTests
{
    [Fact]
    public void RoomToRoomOutDto_ComputesFreePlaces()
    {
        var room = new Room
        {
            Id = 5,
            Floor = 1,
            Number = 12,
            Capacity = 3,
            Status = RoomStatus.Available,
            ResidentsIds = new List<int> { 10, 20 }
        };

        var dto = room.RoomToRoomOutDto();

        Assert.Equal(5, dto.Id);
        Assert.Equal(1, dto.Floor);
        Assert.Equal(12, dto.Number);
        Assert.Equal(3, dto.Capacity);
        Assert.Equal(RoomStatus.Available, dto.Status);
        Assert.Equal(new List<int> { 10, 20 }, dto.ResidentsIds);
        Assert.Equal(1, dto.FreePlaces);
    }
}
