using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class Application
{
    public int ApplicationId { get; set; }
    public int UserId { get; set; }
    [MaxLength]
    public string Description { get; set; } = string.Empty;
    public ApplicationAnswer? Answer { get; set; }
    public ApplicationStatus Status { get; set; }
}

public enum ApplicationStatus
{
    Sent,
    Considered
}