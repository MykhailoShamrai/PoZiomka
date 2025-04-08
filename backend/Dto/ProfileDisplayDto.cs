namespace backend.Dto;

public class ProfileDisplayDto
{
    // Fields here have to be nullable due to the fact that user
    // can set his/her preferences so that he/she does not want 
    // to display some information.
    public string? FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } = string.Empty;
}