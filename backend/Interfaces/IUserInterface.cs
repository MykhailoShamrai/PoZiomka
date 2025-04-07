using backend.Dto;

namespace backend.Interfaces;

public interface IUserInterface
{
    Task ChangeUserPreferences();

    Task<ProfileDisplay> DisplayUserProfile();
}