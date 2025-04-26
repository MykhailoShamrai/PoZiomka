namespace backend.Dto;

public static class RoomInDtoMapper
{
    public static Room RoomInDtoToRoom(this RoomInDto dto)
    {
        return new Room
        {
            Floor = dto.Floor,
            Number = dto.Number,
            Capacity = dto.Capacity,
            ResidentsIds = new List<int>(),
            Status = dto.Status
        };
    }
}