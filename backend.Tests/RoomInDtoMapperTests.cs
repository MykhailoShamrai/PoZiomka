using backend.Dto;

public class RoomInDtoMapperTests
{
    [Fact]
    public void RoomInDtoToRoom_MapsFieldsCorrectly()
    {
        var dto = new RoomInDto
        {
            Floor = 3,
            Number = 101,
            Capacity = 4,
            Status = RoomStatus.Unavailable
        };

        var room = dto.RoomInDtoToRoom();

        Assert.Equal(3, room.Floor);
        Assert.Equal(101, room.Number);
        Assert.Equal(4, room.Capacity);
        Assert.Equal(RoomStatus.Unavailable, room.Status);
        Assert.Empty(room.ResidentsIds);
    }
}
