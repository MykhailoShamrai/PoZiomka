using backend.Data;
using backend.Dto;
using backend.Interfaces;

namespace backend.Repositories;

public class RoomRepository : IRoomInterface
{
    private readonly AppDbContext _appDbContext;
    public RoomRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }
    public async Task<ErrorCodes> AddRoom(RoomInDto dto)
    {
        Room newRoom = dto.RoomInDtoToRoom();
        _appDbContext.Rooms.Add(newRoom);
        var res = await _appDbContext.SaveChangesAsync();
        if (res > 0)
            return ErrorCodes.Ok;
        return ErrorCodes.BadRequest;
    }

    public Task<ErrorCodes> ChangeStatusForRoom(RoomStatus status, int roomId)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorCodes> DeleteRoom(int roomId)
    {
        throw new NotImplementedException();
    }

    public Task<Tuple<List<RoomInDto>, ErrorCodes>> GetRooms()
    {
        throw new NotImplementedException();
    }

    public Task<List<int>> GetUserIdsFromRoom(int roomId)
    {
        throw new NotImplementedException();
    }
}