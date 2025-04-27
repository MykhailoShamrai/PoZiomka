namespace backend.Dto;

public class AddRoleToUserDto
{
    public required string Email { get; set; }
    public required string Role {get; set; }

    public static bool CheckIfRoleIsProper(string Role)
    {
        return Role.Equals("Admin") || Role.Equals("Student"); 
    }
}