namespace backend.Models.User;

public class DisplayPreferences
{
    public int Id { get; set; }
    
    public bool DisplayFirstName { get; set; }
    
    public bool DisplayLastName { get; set; }
    
    public bool DisplayEmail { get; set; }
    
    public bool DisplayPhoneNumber { get; set; }
}