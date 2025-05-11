namespace backend.Dto;

public class ProposalAdminOutDto
{
    public int Id { get; set; }
    public required RoomOutDto Room { get; set; }
    public required List<UserDto> Roommates { get; set; }
    public required List<SingleStudentStatus> Statuses { get; set; }
    public AdminStatus AdminStatus { get; set; } 
    public DateTime Timestamp { get; set; }
    public StatusOfProposal StatusOfProposal { get; set; }
}

public class ProposalUserOutDto
{
    public int Id { get; set; }
    public required RoomOutDto Room { get; set; }
    public required List<UserDto> Roommates { get; set; }
    public StatusOfProposal StatusOfProposal { get; set; }
    public DateTime Timestamp { get; set; }
    public SingleStudentStatus StatusForUser { get; set; }
}