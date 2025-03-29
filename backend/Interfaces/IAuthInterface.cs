using backend.Dto;

namespace backend.Interfaces;

public interface IAuthInterface
{
    public Task<bool> Register(RegisterUserDto dto);
    public Task<bool> Login();
}