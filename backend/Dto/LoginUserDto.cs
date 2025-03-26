namespace backend.Dto;

public class LoginUserDto
{
    public string Email { get; set; } = string.Empty;
    public string HashedPassword { get; set; } = string.Empty;
}