using backend.Models;

namespace backend.Dto;

public class ApplicationOutShortDto
{
    public int ApplicationId { get; set; }
    public int UserId { get; set; } = -1;
    public ApplicationStatus Status { get; set; }
}