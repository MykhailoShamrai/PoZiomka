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
    
    public ProposalRepository(AppDbContext appDbContext, UserManager<User> userManager)
    {
        _appDbContext = appDbContext;
        _userManager = userManager;
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
            Room = proposal.Room.RoomToRoomOutDto(),
            Roommates = roommates,
            Statuses = proposal.Statuses,
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
}