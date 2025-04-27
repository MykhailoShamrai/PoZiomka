using backend.Dto;

namespace backend.Mappers;

public static class RoomOutDtoMapper
{
    public static RoomOutDto RoomToRoomOutDto(this Room room)
    {
        return new RoomOutDto
        {
            Id = room.Id,
            Floor = room.Floor,
            Number = room.Number,
            Capacity = room.Capacity,
            Status = room.Status,
            ResidentsIds = room.ResidentsIds,
            FreePlaces = room.Capacity - room.ResidentsIds.Count()
        };
    }
}