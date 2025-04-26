using backend.Repositories;
using backend.Dto;
namespace backend.Interfaces;

public interface IRoomInterface
{
    public Task<Tuple<List<RoomOutDto>, ErrorCodes>> GetRooms();
    public Task<ErrorCodes> AddRoom(RoomInDto dto);
    public Task<ErrorCodes> DeleteRoom(RoomInDto dto);
    public Task<ErrorCodes> ChangeStatusForRoom(RoomStatus status, int roomId);
    public Task<List<int>> GetUserIdsFromRoom(int roomId);
}