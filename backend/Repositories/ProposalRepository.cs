using System.Security.Claims;
using backend.Data;
using backend.Dto;
using backend.Interfaces;
using backend.Mappers;
using backend.Models.User;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories;

public class ProposalRepository : IProposalInterface
{
    private AppDbContext _appDbContext;
    private UserManager<User> _userManager;
    private IHttpContextAccessor _httpContextAccessor;
    
    public ProposalRepository(AppDbContext appDbContext, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _appDbContext = appDbContext;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ErrorCodes> AddTestProposal(ProposalInDto dto)
    {
        int roomId = dto.RoomId;
        var room = await _appDbContext.Rooms.FindAsync(roomId);
        if (room is null)
            return ErrorCodes.NotFound;
        
        Proposal proposal = new Proposal
        {
            Room = room,
            RoommatesIds = dto.RoommatesIds,
            Statuses = dto.RoommatesIds.Select(r => SingleStudentStatus.Pending).ToList()
        };

        _appDbContext.Add(proposal);
        var res = await _appDbContext.SaveChangesAsync();
        if (res > 0)
            return ErrorCodes.Ok;
        return ErrorCodes.BadRequest;
    }

    public async Task<ProposalAdminOutDto> ProposalToAdminDto(Proposal proposal)
    {
        var roommates = new List<UserDto>();
        foreach (var id in proposal.RoommatesIds)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user != null)
            {
                roommates.Add(user.UserToDto());
            }
        }
        return new ProposalAdminOutDto
        {
            Id = proposal.Id,
            Room = proposal.Room.RoomToRoomOutDto(),
            Roommates = roommates,
            Statuses = proposal.Statuses,
            AdminStatus = proposal.AdminStatus,
            Timestamp = proposal.Timestamp
        };
    }

    public async Task<ProposalUserOutDto> ProposalToUserDto(Proposal proposal)
    {
        var roommates = new List<UserDto>();
        foreach (var id in proposal.RoommatesIds)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user != null)
            {
                roommates.Add(user.UserToDto());
            }
        }
        return new ProposalUserOutDto
        {
            Id = proposal.Id,
            Room = proposal.Room.RoomToRoomOutDto(),
            Roommates = roommates,
            AdminStatus = proposal.AdminStatus,
            Timestamp = proposal.Timestamp
        };
    }

    public async Task<Tuple<List<ProposalAdminOutDto>, ErrorCodes>> ReturnAllProposals()
    {
        var proposals = await _appDbContext.Proposals.Include(p => p.Room).ToListAsync();
        var proposalDtos = await Task.WhenAll(proposals.Select(p => ProposalToAdminDto(p)));
        if (proposalDtos is null)
            return new Tuple<List<ProposalAdminOutDto>, ErrorCodes>(new List<ProposalAdminOutDto>(), ErrorCodes.BadRequest);
        if (proposalDtos.Count() == 0)
            return new Tuple<List<ProposalAdminOutDto>, ErrorCodes>(new List<ProposalAdminOutDto>(), ErrorCodes.NotFound);
        return new Tuple<List<ProposalAdminOutDto>, ErrorCodes>(proposalDtos.ToList(), ErrorCodes.Ok);
    }

    public async Task<Tuple<List<ProposalUserOutDto>, ErrorCodes>> ReturnUsersProposals()
    {
        var email = _httpContextAccessor.HttpContext?.User?.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (email is null)
            return new Tuple<List<ProposalUserOutDto>, ErrorCodes>(new List<ProposalUserOutDto>(), ErrorCodes.Unauthorized);

        var currentUser = await _userManager.FindByEmailAsync(email!);
        if (currentUser is null)
            return new Tuple<List<ProposalUserOutDto>, ErrorCodes>(new List<ProposalUserOutDto>(), ErrorCodes.Unauthorized);
        
        var proposals = await _appDbContext.Proposals.Where(p => p.RoommatesIds.Contains(currentUser.Id)).Include(p => p.Room).ToListAsync();
        if (currentUser is null)
            return new Tuple<List<ProposalUserOutDto>, ErrorCodes>(new List<ProposalUserOutDto>(), ErrorCodes.BadRequest);

        var proposalDtos =  await Task.WhenAll(proposals.Select(p => ProposalToUserDto(p)));
        if (proposalDtos is null)
            return new Tuple<List<ProposalUserOutDto>, ErrorCodes>(new List<ProposalUserOutDto>(), ErrorCodes.BadRequest);
        
        return new Tuple<List<ProposalUserOutDto>, ErrorCodes>(proposalDtos.ToList(), ErrorCodes.Ok);
    }
}