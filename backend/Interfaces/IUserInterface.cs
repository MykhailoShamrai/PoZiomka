using backend.Dto;
using backend.Models.User;
using backend.Repositories;

namespace backend.Interfaces;

public interface IUserInterface
{
    public Task<ErrorCodes> ChangeUserPreferences(UserPreferences newUserPreferences);

    public Task<Tuple<ErrorCodes, ProfileDisplayDto?>> DisplayUserProfile();
    public Task<ErrorCodes> ChangeUserProfile(UpdateUserDto user);

    public Task<Tuple<ErrorCodes, Form[]?>> GetUserForms();

    public Task<ErrorCodes> SubmitAnswerForForms(AnswerDto dto);
    // public Task<ErrorCodes> SubmitAnswers(AnswerDto answer);
}