using backend.Data;
using backend.Dto;
using backend.Interfaces;
using backend.Mappers;
using backend.Models.User;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories;

public class RoomRepository : IRoomInterface
{
    private readonly AppDbContext _appDbContext;
    private readonly UserManager<User> _userManager;
    public RoomRepository(AppDbContext appDbContext, UserManager<User> userManager)
    {
        _appDbContext = appDbContext;
        _userManager = userManager;
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

    public async Task<ErrorCodes> ChangeStatusForRoom(SetStatusToRoomDto dto)
    {
        var room = await _appDbContext.Rooms.FindAsync(dto.RoomId);
        if (room is null)
            return ErrorCodes.NotFound;
        room.Status = dto.Status;
        var res = await _appDbContext.SaveChangesAsync();
        if (res > 0)
            return ErrorCodes.Ok;
        return ErrorCodes.BadRequest;
    }
    
    public async Task<Tuple<List<RoomOutDto>, ErrorCodes>> GetRooms()
    {
        List<Room> rooms = await _appDbContext.Rooms.ToListAsync();
        if (rooms is null)
            return new Tuple<List<RoomOutDto>, ErrorCodes>(new List<RoomOutDto>(), ErrorCodes.BadRequest);
        List<RoomOutDto> resList = rooms.Select(r => r.RoomToRoomOutDto()).ToList();
        return new Tuple<List<RoomOutDto>, ErrorCodes>(resList, ErrorCodes.Ok);
    }

    public async Task<ErrorCodes> ApplyUserToRoom(UserRoomDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.UserEmail);
        if (user is null)
            return ErrorCodes.NotFound;
        
        var room = await _appDbContext.Rooms.FindAsync(dto.RoomId);
        if (room is null)
            return ErrorCodes.NotFound;

        if (room.ResidentsIds.Count < room.Capacity)     
        {    
            if (!room.ResidentsIds.Contains(user.Id))
                room.ResidentsIds.Add(user.Id);
        }
        else
            return ErrorCodes.BadRequest;


        if (room.ResidentsIds.Count == room.Capacity)
            room.Status = RoomStatus.Unavailable;

        var roomWhereUserLives = await _appDbContext.Rooms.Where(r => r.ResidentsIds.Contains(user.Id) && r.Id != dto.RoomId).FirstOrDefaultAsync();

        if (roomWhereUserLives is not null)
        {
            roomWhereUserLives.ResidentsIds.Remove(user.Id);
            if (roomWhereUserLives.Capacity == roomWhereUserLives.ResidentsIds.Count + 1)
                roomWhereUserLives.Status = RoomStatus.Available;
        }
        var res = await _appDbContext.SaveChangesAsync();
        return ErrorCodes.Ok;
    }

    public async Task<ErrorCodes> RemoveUserFromRoom(UserRoomDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.UserEmail);
        if (user is null)
            return ErrorCodes.NotFound;

        var room = await _appDbContext.Rooms.Where(r => r.Id == dto.RoomId && r.ResidentsIds.Contains(user.Id)).FirstOrDefaultAsync();
        if (room is null)
            return ErrorCodes.NotFound;
        
        room.ResidentsIds.Remove(user.Id);

        if (room.Capacity == room.ResidentsIds.Count + 1)
            room.Status = RoomStatus.Available;

        var res = await _appDbContext.SaveChangesAsync();
        if (res > 0)
            return ErrorCodes.Ok;
        return ErrorCodes.BadRequest;
    }
}