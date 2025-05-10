namespace backend.Dto;

public class ProposalInDto
{
    public int RoomId { get; set; }
    public List<int> RoommatesIds { get; set; } = new List<int>();
}