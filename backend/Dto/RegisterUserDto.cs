namespace backend.Dto;

public class RegisterUserDto
{
    public string Email { get; set; } = string.Empty;
    public string HashedPassword { get; set; } = string.Empty;
}