public class Communication
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required CommunicationType Type { get; set; }
    public string Description { get; set; } = string.Empty;
}
