using backend.Data;
using backend.Dto;
using backend.Interfaces;
using backend.Mappers;
using Microsoft.EntityFrameworkCore;

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

    private async Task<Room?> FindRoomFromNumberAndFloor(int floor, int number)
    {
        return await _appDbContext.Rooms.Where(r => r.Floor == floor && r.Number == number)
            .SingleAsync();
    }
    public async Task<ErrorCodes> DeleteRoom(RoomInDto dto)
    {
        try 
        {
            var entity = await FindRoomFromNumberAndFloor(dto.Floor, dto.Number);
            _appDbContext.Rooms.Remove(entity!);
            var res = await _appDbContext.SaveChangesAsync();
            if (res > 0)
                return ErrorCodes.Ok;
            return ErrorCodes.BadRequest;
        }
        catch (InvalidOperationException)
        {
            return ErrorCodes.NotFound;
        }
    }

    public Task<ErrorCodes> ChangeStatusForRoom(RoomStatus status, int roomId)
    {
        throw new NotImplementedException();
    }


    public async Task<Tuple<List<RoomOutDto>, ErrorCodes>> GetRooms()
    {
        List<Room> rooms = await _appDbContext.Rooms.ToListAsync();
        if (rooms is null)
            return new Tuple<List<RoomOutDto>, ErrorCodes>(new List<RoomOutDto>(), ErrorCodes.BadRequest);
        List<RoomOutDto> resList = rooms.Select(r => r.RoomToRoomOutDto()).ToList();
        return new Tuple<List<RoomOutDto>, ErrorCodes>(resList, ErrorCodes.Ok);
    }

    public Task<List<int>> GetUserIdsFromRoom(int roomId)
    {
        throw new NotImplementedException();
    }
}