using backend.Dto;
using backend.Models.User;
using backend.Repositories;

namespace backend.Interfaces;

public interface IUserInterface
{
    public Task<ErrorCodes> ChangeUserPreferences(UserPreferences newUserPreferences);

    public Task<Tuple<ErrorCodes, ProfileDisplayDto?>> DisplayUserProfile(string email);
    public Task<ErrorCodes> ChangeUserProfile(UpdateUserDto user);

    public Task<Tuple<ErrorCodes, Form[]?>> GetUserForms(string email);
    // public Task<ErrorCodes> SubmitAnswers(AnswerDto answer);
}