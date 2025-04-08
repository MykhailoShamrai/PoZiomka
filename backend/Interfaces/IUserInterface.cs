using backend.Dto;
using backend.Models.User;
using backend.Repositories;

namespace backend.Interfaces;

public interface IUserInterface
{
    public Task<UserErrorCodes> ChangeUserPreferences(bool[] displayPreferences);

    public Task<Tuple<UserErrorCodes, ProfileDisplayDto?>> DisplayUserProfile(string email);
}