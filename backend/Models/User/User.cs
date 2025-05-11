using Microsoft.AspNetCore.Identity;
namespace backend.Models.User;

public class User: IdentityUser<int>
{
    public string? FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; } = string.Empty;
    
    public string? StudentIndex{ get; set; } = string.Empty;
    public UserPreferences Preferences { get; set; } = new UserPreferences();
    public StudentStatus StudentStatus { get; set; } = StudentStatus.Active;
}

public enum StudentStatus
{
    Active,
    Inactive,
    Confirmed 
}