using backend.Dto;
using backend.Models.User;
using backend.Repositories;

namespace backend.Interfaces;

public interface IUserInterface
{
    public Task<UserErrorCodes> ChangeUserPreferences(DisplayPreferences displayPreferences);

    public Task<Tuple<UserErrorCodes, ProfileDisplay>> DisplayUserProfile(string email);
}