namespace backend.Dto;

public class ProfileDisplayDto
{
    // Fields here have to be nullable due to the fact that user
    // can set his/her preferences so that he/she does not want 
    // to display some information.
    
    // Each of these preferences is a consecutive bit in Uint64 Preferences (User model)
    // Example: user wants only his/her first name displayed, then Preferences is 1UL (default behaviour).
    
    public string? FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } = string.Empty;
}