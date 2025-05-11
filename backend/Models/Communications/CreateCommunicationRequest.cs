namespace backend.Models.Communications;

public class CreateCommunicationRequest
{
    public CommunicationType Type { get; set; }
    public string Description { get; set; } = string.Empty;
}