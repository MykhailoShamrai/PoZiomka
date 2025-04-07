using backend.Dto;
using backend.Models.User;

namespace backend.Interfaces;

public interface IUserInterface
{
    Task ChangeUserPreferences(DisplayPreferences displayPreferences);

    Task<ProfileDisplay> DisplayUserProfile(string email);
}