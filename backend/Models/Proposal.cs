using backend.Models.User;

public class Proposal
{
    public int Id { get; set; }
    public required Room Room { get; set;}
    public required List<int> RoommatesIds { get; set; } = new List<int>();
    public required List<SingleStudentStatus> Statuses { get; set; } = new List<SingleStudentStatus>();
    public AdminStatus AdminStatus {get; set; } = AdminStatus.Unavailable;
    public DateTime Timestamp { get; set; } = DateTime.Now.Date;
    public StatusOfProposal WholeStatus { get; set; } = StatusOfProposal.WaitingForRoommates;
}

public enum SingleStudentStatus
{
    Accepted,
    Rejected,
    Pending
}

public enum AdminStatus
{   
    Accepted,
    Rejected,
    Pending,
    Unavailable
}

public enum StatusOfProposal
{
    WaitingForRoommates,
    AcceptedByRoommates,
    RejectedByOneOrMoreUsers,
    AcceptedByAdmin,
    RejectedByAdmin
}