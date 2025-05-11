using System.Security.Claims;
using backend.Data;
using backend.Dto;
using backend.Interfaces;
using backend.Models;
using backend.Models.User;
using backend.Repositories;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class ApplicationService : IApplicationInterface
{
    private readonly AppDbContext _appDbContext;
    private readonly UserManager<User> _userManager;
    private readonly HttpContextAccessor _contextAccessor;
    public ApplicationService (AppDbContext appDbContext, UserManager<User> userManager, HttpContextAccessor contextAccessor)
    {
        _appDbContext = appDbContext;
        _userManager = userManager;
        _contextAccessor = contextAccessor;
    }

    public async Task<ErrorCodes> AnswerToApplication(ApplicationAnswerInDto dto)
    {
        var email = _contextAccessor.HttpContext?.User?.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (email is null)
            return ErrorCodes.Unauthorized;

        var admin = await _userManager.FindByEmailAsync(email!);
        if (admin is null)
            return ErrorCodes.Unauthorized;
        
        var application = await _appDbContext.Applications.Where(app => app.ApplicationId == dto.ApplicationId).FirstOrDefaultAsync();
        if (application is null)
            return ErrorCodes.NotFound;
        
        ApplicationAnswer answ = new ApplicationAnswer{
            AdminId = admin.Id,
            Application = application,
            Description = dto.Description
        };
        application.Status = ApplicationStatus.Considered;
        application.Answer = answ;
        _appDbContext.Add(answ);
        var res = await _appDbContext.SaveChangesAsync();
        if (res > 0)
            return ErrorCodes.Ok;
        return ErrorCodes.BadRequest;
    }

    public async Task<ErrorCodes> SendAnApplication(ApplicationInDto dto)
    {
        var email = _contextAccessor.HttpContext?.User?.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (email is null)
            return ErrorCodes.Unauthorized;

        var user = await _userManager.FindByEmailAsync(email!);
        if (user is null)
            return ErrorCodes.Unauthorized;

        Application application = new Application{
            UserId = user.Id,
            Description = dto.Description,
        };

        _appDbContext.Add(application);
        var res = await _appDbContext.SaveChangesAsync();
        if (res > 0)
            return ErrorCodes.Ok;
        return ErrorCodes.BadRequest;
    }
}
