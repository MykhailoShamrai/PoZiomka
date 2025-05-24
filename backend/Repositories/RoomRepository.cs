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
    public async Task<ErrorCodes> AddRoom(List<RoomInDto> dtos)
    {
        foreach (var dto in dtos)
        {
            Room newRoom = dto.RoomInDtoToRoom();
            _appDbContext.Rooms.Add(newRoom);
            var res = await _appDbContext.SaveChangesAsync();
            if (res < 0)
                return ErrorCodes.BadRequest;
        }
        return ErrorCodes.Ok;
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

    public async Task<ErrorCodes> AddWholeProposalToARoom(int proposalId)
    {
        var proposal = await _appDbContext.Proposals.Include(pr => pr.Room).FirstOrDefaultAsync();
        if (proposal is null)
            return ErrorCodes.NotFound;

        if (proposal.WholeStatus != StatusOfProposal.AcceptedByAdmin || proposal.Room.Status != RoomStatus.Available)
        {
            return ErrorCodes.BadArgument;
        }

        var room = proposal.Room;

        var users = await _userManager.Users.Where(u => proposal.RoommatesIds.Contains(u.Id)).ToListAsync();
        foreach (var user in users)
        {
            var roomWhereUserLives = await _appDbContext.Rooms.Where(r => r.ResidentsIds.Contains(user.Id) && r.Id != room.Id).FirstOrDefaultAsync();
    
            if (roomWhereUserLives is not null)
            {
                roomWhereUserLives.ResidentsIds.Remove(user.Id);
                if (roomWhereUserLives.Capacity == roomWhereUserLives.ResidentsIds.Count + 1)
                    roomWhereUserLives.Status = RoomStatus.Available;
            }
            room.ResidentsIds.Add(user.Id);
        }

        room.Status = RoomStatus.Unavailable;

        return ErrorCodes.BadRequest;
    }
}