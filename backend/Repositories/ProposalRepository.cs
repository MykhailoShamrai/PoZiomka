using System.Dynamic;
using System.Security.Claims;
using System.Xml;
using backend.Data;
using backend.Dto;
using backend.Interfaces;
using backend.Mappers;
using backend.Models.Communications;
using backend.Models.User;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories;

public class ProposalRepository : IProposalInterface
{
    private AppDbContext _appDbContext;
    private UserManager<User> _userManager;
    private CommunicationSender _communicationSender;
    private IHttpContextAccessor _httpContextAccessor;
    
    public ProposalRepository(
        AppDbContext appDbContext, 
        UserManager<User> userManager, 
        CommunicationSender communicationSender,
        IHttpContextAccessor httpContextAccessor
        )
    {
        _appDbContext = appDbContext;
        _userManager = userManager;
        _communicationSender = communicationSender;
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
            Timestamp = proposal.Timestamp,
            StatusOfProposal = proposal.WholeStatus
        };
    }

    public async Task<ProposalUserOutDto> ProposalToUserDto(Proposal proposal, int userId)
    {
        var roommates = new List<UserDto>();
        int k = 0;
        for (int i = 0; i < proposal.RoommatesIds.Count; i++)//(var id in proposal.RoommatesIds)
        {
            var id = proposal.RoommatesIds[i];
            if (id == userId)
                k = i;
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
            StatusOfProposal = proposal.WholeStatus,
            Timestamp = proposal.Timestamp,
            StatusForUser = proposal.Statuses[k]
        };
    }

    public async Task<Tuple<List<ProposalAdminOutDto>, ErrorCodes>> ReturnAllProposals()
    {
        var proposals = await _appDbContext.Proposals.Include(p => p.Room).ToListAsync();
        var proposalDtos = new List<ProposalAdminOutDto>();
        foreach (var p in proposals)
        {
            proposalDtos.Add(await ProposalToAdminDto(p));
        }
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
        
        var proposals = await _appDbContext.Proposals
            .Where(p => p.RoommatesIds.Contains(currentUser.Id) && p.WholeStatus == StatusOfProposal.WaitingForRoommates)
            .Include(p => p.Room).ToListAsync();
        if (currentUser is null)
            return new Tuple<List<ProposalUserOutDto>, ErrorCodes>(new List<ProposalUserOutDto>(), ErrorCodes.BadRequest);
        
        var proposalDtos = new List<ProposalUserOutDto>();
        foreach (var p in proposals)
        {
                proposalDtos.Add(await ProposalToUserDto(p, currentUser.Id));
        } 
        if (proposalDtos is null)
            return new Tuple<List<ProposalUserOutDto>, ErrorCodes>(new List<ProposalUserOutDto>(), ErrorCodes.BadRequest);
        
        return new Tuple<List<ProposalUserOutDto>, ErrorCodes>(proposalDtos.ToList(), ErrorCodes.Ok);
    }

    public async Task<ErrorCodes> UserAnswersTheProposal(UserChangesStatusProposalDto dto)
    {
        var proposal = await _appDbContext.Proposals.Where(p => p.Id == dto.ProposalId).FirstOrDefaultAsync();

        if (proposal is null)
            return ErrorCodes.NotFound;
        
        var email = _httpContextAccessor.HttpContext?.User?.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Email);
        
        if (email is null)
            return ErrorCodes.Unauthorized;
        
        var email_val = email?.Value;
        if (email_val is null)
            return ErrorCodes.Unauthorized;

        var currentUser = await _userManager.FindByEmailAsync(email_val);

        if (currentUser is null)
            return ErrorCodes.Unauthorized;
        
        int index = proposal.RoommatesIds.FindIndex(rm => rm == currentUser.Id);
        
        if (index < 0)
            return ErrorCodes.BadArgument;
        
        proposal.Statuses[index] = dto.Status;

        var communication = new CreateCommunicationRequest();
        if (dto.Status == SingleStudentStatus.Rejected)
        {
            proposal.WholeStatus = StatusOfProposal.RejectedByOneOrMoreUsers;
            proposal.AdminStatus = AdminStatus.Pending;
            communication.Type = CommunicationType.FAILURE;
            communication.Description = "Your proposal was rejected by one of the roommates";

        }
        else if(CheckIfAllRoommatesAgree(proposal))
        {
            proposal.WholeStatus = StatusOfProposal.AcceptedByRoommates;
            proposal.AdminStatus = AdminStatus.Pending;
            communication.Type = CommunicationType.SUCCESS;
            communication.Description = "Your proposal was successfully accepted by all roommates";
        }

        _communicationSender.CreateCommunication(communication, proposal.RoommatesIds);
        var res = await _appDbContext.SaveChangesAsync();
        _communicationSender.CreateCommunication(communication, proposal.RoommatesIds);
        if (res > 0)
            return ErrorCodes.Ok;
        return ErrorCodes.BadRequest;
    }

    private bool CheckIfAllRoommatesAgree(Proposal proposal)
    {
        bool areAllAgreed = true;
        foreach (var status in proposal.Statuses)
        {
            if (status != SingleStudentStatus.Accepted)
            {
                areAllAgreed = false;
                break;
            }
        }
        return areAllAgreed;
    }

    
    public async Task<ErrorCodes> AdminChangesStatusTheProposal(AdminChangesStatusProposalDto dto)
    {
        var proposal = await _appDbContext.Proposals.Include(p => p.Room).Where(p => p.Id == dto.ProposalId).FirstOrDefaultAsync();

        if (proposal is null)
            return ErrorCodes.NotFound;
        
        if (proposal.AdminStatus != AdminStatus.Pending)
            return ErrorCodes.BadArgument;

        var communication = new CreateCommunicationRequest();
        if (dto.Status == AdminStatus.Accepted)
        {
            communication.Type = CommunicationType.SUCCESS;
            communication.Description = "Your proposal was successfully accepted";
            proposal.WholeStatus = StatusOfProposal.AcceptedByAdmin;
            // Place for changing status for all roommate, changing status of room and 
            // Making all proposals fot that room unavailable
            var users = await _userManager.Users.Where(u => proposal.RoommatesIds.Contains(u.Id)).ToListAsync();
            foreach (var u in users)
            {
                u.StudentStatus = StudentStatus.Confirmed;
            }
            var proposals = await _appDbContext.Proposals.Include(p => p.Room)
                .Where(p => p.Room.Id == proposal.Room.Id && p.WholeStatus == StatusOfProposal.WaitingForRoommates && p.Id != proposal.Id)
                .ToListAsync();
            foreach (var p in proposals)
            {
                p.AdminStatus = AdminStatus.Unavailable;
                p.WholeStatus = StatusOfProposal.Unavailable;
            }
        }
        else if (dto.Status == AdminStatus.Rejected)
        {
            communication.Type = CommunicationType.FAILURE;
            communication.Description = "Your proposal was rejected";
            proposal.WholeStatus = StatusOfProposal.RejectedByAdmin;
        }
        proposal.AdminStatus = dto.Status;

        _communicationSender.CreateCommunication(communication, proposal.RoommatesIds);
        var res = await _appDbContext.SaveChangesAsync();
        _communicationSender.CreateCommunication(communication, proposal.RoommatesIds);

        if (res > 0)
            return ErrorCodes.Ok;
        return ErrorCodes.BadRequest;
    }
}