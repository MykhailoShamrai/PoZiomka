using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class ApplicationAnswer
{
    public int ApplicationAnswerId { get; set; }
    public int AdminId { get; set; }
    public required Application Application { get; set; } 
    [MaxLength]
    public string Description { get; set; } = "";
}