using backend.Dto;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace backend.Interfaces;

public interface IApplicationInterface
{
    public Task<ErrorCodes> SendAnApplication(ApplicationInDto dto);
    public Task<ErrorCodes> AnswerToApplication(ApplicationAnswerInDto dto);
}