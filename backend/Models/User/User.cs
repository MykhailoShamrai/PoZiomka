using Microsoft.AspNetCore.Identity;
namespace backend.Models.User;

public class User: IdentityUser<int>
{
    public string? FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; } = string.Empty;
    
    public string? StudentIndex{ get; set; } = string.Empty;
    public required UserPreferences Preferences { get; set; }
}