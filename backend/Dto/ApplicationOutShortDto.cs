using backend.Models;

namespace backend.Dto;

public class ApplicationOutShortDto
{
    public int ApplicationId { get; set; }
    public int UserId { get; set; }
    public ApplicationStatus Status { get; set; }
    public string Description { get; set; } = string.Empty;
}