namespace backend.Interfaces;

public interface IAuthInterface
{
    public Task<bool> Register();
    public Task<bool> Login();
}