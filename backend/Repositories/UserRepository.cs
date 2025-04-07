using backend.Dto;
using backend.Interfaces;

namespace backend.Repositories;

public class UserRepository : IUserInterface
{
    public Task ChangeUserPreferences()
    {
        throw new NotImplementedException();
    }

    public Task<ProfileDisplay> DisplayUserProfile()
    {
        throw new NotImplementedException();
    }
}