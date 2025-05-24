using System.Net;
using backend.Dto;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace backend.Interfaces;

public interface IApplicationInterface
{
    public Task<ErrorCodes> SendAnApplication(ApplicationInDto dto);
    public Task<ErrorCodes> AnswerToApplication(ApplicationAnswerInDto dto);
    public Task<Tuple<ErrorCodes, List<ApplicationOutShortDto>>> ReturnUsersApplications();
    public Task<Tuple<ErrorCodes, ApplicationOutLongDto>> ReturnInformationAboutSpecificApplication(int applicationId);
    public Task<Tuple<ErrorCodes, List<ApplicationAnswerOutShortDto>>> ReturnAdminsAnswers();
    public Task<Tuple<ErrorCodes, ApplicationAnswerOutLongDto>> ReturnInformationAboutSpecificAnswer(int applicationAnswerId);
}