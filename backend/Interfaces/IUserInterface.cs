using backend.Dto;
using backend.Models.User;
using backend.Repositories;

namespace backend.Interfaces;

public interface IUserInterface
{
    public Task<ErrorCodes> ChangeUserPreferences(UserPreferences newUserPreferences);

    public Task<Tuple<ErrorCodes, ProfileDisplayDto?>> DisplayUserProfile();
    public Task<ErrorCodes> ChangeUserProfile(UpdateUserDto user);

    public Task<(ErrorCodes, FormDto[]?)> GetUserForms();

    public Task<ErrorCodes> SubmitAnswerForForms(AnswerDto dto);
    public Task<(ErrorCodes, Communication[]?)> GetCurrentUserCommunications();
}