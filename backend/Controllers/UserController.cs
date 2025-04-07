using backend.Interfaces;
using backend.Repositories;

public class UserController
{
    private readonly IUserInterface _userInterface;

    public UserController(IUserInterface userInterface)
    {
        _userInterface = userInterface;
    }
}
