namespace backend.Models.User;

public class UserPreferences
{
    public bool DisplayFirstName { get; set; } = true;
    public bool DisplayLastName { get; set; }
    public bool DisplayEmail { get; set; }
    public bool DisplayPhoneNumber { get; set; }
}