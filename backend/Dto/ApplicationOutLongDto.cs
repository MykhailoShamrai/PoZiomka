using backend.Models;

namespace backend.Dto;

public class ApplicationOutLongDto
{
    public int ApplicationId { get; set; }
    public int UserId { get; set; }
    public string Description { get; set; } = "";
    public ApplicationStatus Status { get; set; }
}