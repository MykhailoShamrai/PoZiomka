namespace backend.Dto;

public class ProposalAdminOutDto
{
    public RoomOutDto Room { get; set; }
    public List<UserDto> Roommates { get; set; }
    public List<SingleStudentStatus> Statuses { get; set; }
    public AdminStatus AdminStatus { get; set; } 
    public DateTime Timestamp { get; set; }
}