using backend.Repositories;
using backend.Dto;
namespace backend.Interfaces;

public interface IRoomInterface
{
    public Task<Tuple<List<RoomOutDto>, ErrorCodes>> GetRooms();
    public Task<ErrorCodes> AddRoom(List<RoomInDto> dtos);
    public Task<ErrorCodes> DeleteRoom(RoomInDto dto);
    public Task<ErrorCodes> ChangeStatusForRoom(SetStatusToRoomDto dto);
    public Task<ErrorCodes> ApplyUserToRoom(UserRoomDto dto);
    public Task<ErrorCodes> RemoveUserFromRoom(UserRoomDto dto);
}