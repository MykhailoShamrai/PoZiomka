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
    private readonly IHttpContextAccessor _contextAccessor;
    public ApplicationService(AppDbContext appDbContext, UserManager<User> userManager, IHttpContextAccessor contextAccessor)
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

        ApplicationAnswer answ = new ApplicationAnswer
        {
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

        Application application = new Application
        {
            UserId = user.Id,
            Description = dto.Description,
        };

        _appDbContext.Add(application);
        var res = await _appDbContext.SaveChangesAsync();
        if (res > 0)
            return ErrorCodes.Ok;
        return ErrorCodes.BadRequest;
    }

    public async Task<Tuple<ErrorCodes, List<ApplicationAnswerOutShortDto>>> ReturnAdminsAnswers()
    {
        var email = _contextAccessor.HttpContext?.User?.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (email is null)
            return new Tuple<ErrorCodes, List<ApplicationAnswerOutShortDto>>(ErrorCodes.Unauthorized, new List<ApplicationAnswerOutShortDto>());

        var admin = await _userManager.FindByEmailAsync(email!);
        if (admin is null)
            return new Tuple<ErrorCodes, List<ApplicationAnswerOutShortDto>>(ErrorCodes.Unauthorized, new List<ApplicationAnswerOutShortDto>());

        var answers = await _appDbContext.ApplicationAnswers.Include(a => a.Application).Where(a => a.AdminId == admin.Id).ToListAsync();
        if (answers is null)
            return new Tuple<ErrorCodes, List<ApplicationAnswerOutShortDto>>(ErrorCodes.NotFound, new List<ApplicationAnswerOutShortDto>());

        return new Tuple<ErrorCodes, List<ApplicationAnswerOutShortDto>>(ErrorCodes.Ok, answers.Select(a => new ApplicationAnswerOutShortDto
        {
            ApplicationAnswerId = a.ApplicationAnswerId,
            ApplicationId = a.Application.ApplicationId
        }).ToList());
    }

    public async Task<Tuple<ErrorCodes, ApplicationAnswerOutLongDto>> ReturnInformationAboutSpecificAnswer(int applicationAnswerId)
    {
        // Here I'll check if I can give an Answer for admin. I don't want that admin 1 can have answer from admin 2
        var email = _contextAccessor.HttpContext?.User?.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (email is null)
            return new Tuple<ErrorCodes, ApplicationAnswerOutLongDto>(ErrorCodes.Unauthorized, new ApplicationAnswerOutLongDto());

        var admin = await _userManager.FindByEmailAsync(email!);
        if (admin is null)
            return new Tuple<ErrorCodes, ApplicationAnswerOutLongDto>(ErrorCodes.Unauthorized, new ApplicationAnswerOutLongDto());
        
        var answer = await _appDbContext.ApplicationAnswers.Where(a => a.ApplicationAnswerId == applicationAnswerId).FirstOrDefaultAsync();
        if (answer is null)
            return new Tuple<ErrorCodes, ApplicationAnswerOutLongDto>(ErrorCodes.NotFound, new ApplicationAnswerOutLongDto());

        if (answer.AdminId != admin.Id)
            return new Tuple<ErrorCodes, ApplicationAnswerOutLongDto>(ErrorCodes.Forbidden, new ApplicationAnswerOutLongDto());

        return new Tuple<ErrorCodes, ApplicationAnswerOutLongDto>(ErrorCodes.Ok, new ApplicationAnswerOutLongDto
        {
            ApplicationAnswerid = answer.ApplicationAnswerId,
            Description = answer.Description
        });
    }

    public async Task<Tuple<ErrorCodes, List<ApplicationOutShortDto>>> ReturnUsersApplications()
    {
        var email = _contextAccessor.HttpContext?.User?.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (email is null)
            return new Tuple<ErrorCodes, List<ApplicationOutShortDto>>(ErrorCodes.Unauthorized, new List<ApplicationOutShortDto>());

        var user = await _userManager.FindByEmailAsync(email!);
        if (user is null)
            return new Tuple<ErrorCodes, List<ApplicationOutShortDto>>(ErrorCodes.Unauthorized, new List<ApplicationOutShortDto>());

        var roles = await _userManager.GetRolesAsync(user);
        List<Application> applications = await _appDbContext.Applications
            .Where(a => a.UserId == user.Id).ToListAsync();

        return new Tuple<ErrorCodes, List<ApplicationOutShortDto>>(ErrorCodes.Ok, applications.Select(a => new ApplicationOutShortDto
        {
            ApplicationId = a.ApplicationId,
            UserId = a.UserId,
            Status = a.Status,
            Description = a.Description
        }).ToList());
    }
    
    public async Task<Tuple<ErrorCodes, ApplicationOutLongDto>> ReturnInformationAboutSpecificApplication(int applicationId)
    {
        var email = _contextAccessor.HttpContext?.User?.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (email is null)
            return new Tuple<ErrorCodes, ApplicationOutLongDto>(ErrorCodes.Unauthorized, new ApplicationOutLongDto());

        var user = await _userManager.FindByEmailAsync(email!);
        if (user is null)
            return new Tuple<ErrorCodes, ApplicationOutLongDto>(ErrorCodes.Unauthorized, new ApplicationOutLongDto());

        var application = await _appDbContext.Applications.Where(a => a.ApplicationId == applicationId).FirstOrDefaultAsync();
        if (application is null)
            return new Tuple<ErrorCodes, ApplicationOutLongDto>(ErrorCodes.NotFound, new ApplicationOutLongDto());

        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Contains("Admin") && application.UserId != user.Id)
            return new Tuple<ErrorCodes, ApplicationOutLongDto>(ErrorCodes.Forbidden, new ApplicationOutLongDto());

        return new Tuple<ErrorCodes, ApplicationOutLongDto>(ErrorCodes.Ok, new ApplicationOutLongDto
        {
            ApplicationId = application.ApplicationId,
            UserId = user.Id,
            Description = application.Description,
            Status = application.Status
        });
    }

    public async Task<Tuple<ErrorCodes, List<ApplicationOutShortDto>>> ReturnAllNotConsideredApplications()
    {
        var applications = await _appDbContext.Applications.Where(a => a.Status != ApplicationStatus.Considered).ToListAsync();
        if (applications is null)
            return new Tuple<ErrorCodes, List<ApplicationOutShortDto>>(ErrorCodes.NotFound, new List<ApplicationOutShortDto>());
        return new Tuple<ErrorCodes, List<ApplicationOutShortDto>>(ErrorCodes.Ok, applications.Select(a => new ApplicationOutShortDto
        {
            ApplicationId = a.ApplicationId,
            UserId = a.UserId,
            Status = a.Status
        }).ToList());
    }

    public async Task<Tuple<ErrorCodes, ApplicationAnswerOutLongDto>> ReturnAnswerForSpecificApplication(int applicationId)

    {
        var email = _contextAccessor.HttpContext?.User?.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (email is null)
            return new Tuple<ErrorCodes, ApplicationAnswerOutLongDto>(ErrorCodes.Unauthorized, new ApplicationAnswerOutLongDto());

        var user = await _userManager.FindByEmailAsync(email!);
        if (user is null)
            return new Tuple<ErrorCodes, ApplicationAnswerOutLongDto>(ErrorCodes.Unauthorized, new ApplicationAnswerOutLongDto());

        var application = await _appDbContext.Applications.Include(a => a.Answer).Where(a => a.ApplicationId == applicationId && a.UserId == user.Id && a.Status == ApplicationStatus.Considered).
                                                                FirstOrDefaultAsync();
        if (application is null)
            return new Tuple<ErrorCodes, ApplicationAnswerOutLongDto>(ErrorCodes.NotFound, new ApplicationAnswerOutLongDto());

        return new Tuple<ErrorCodes, ApplicationAnswerOutLongDto>(ErrorCodes.Ok, new ApplicationAnswerOutLongDto
        {
            ApplicationAnswerid = application.Answer!.ApplicationAnswerId,
            Description = application.Answer!.Description
        });

    }
    
    public async Task<Tuple<ErrorCodes, List<ApplicationOutLongDto>>> GetAllApplications()
    {
        var email = _contextAccessor.HttpContext?.User?.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (email is null)
            return new Tuple<ErrorCodes, List<ApplicationOutLongDto>>(ErrorCodes.Unauthorized, new List<ApplicationOutLongDto>());

        var admin = await _userManager.FindByEmailAsync(email!);
        if (admin is null)
            return new Tuple<ErrorCodes, List<ApplicationOutLongDto>>(ErrorCodes.Unauthorized, new List<ApplicationOutLongDto>());

        var roles = await _userManager.GetRolesAsync(admin);
        if (!roles.Contains("Admin"))
            return new Tuple<ErrorCodes, List<ApplicationOutLongDto>>(ErrorCodes.Forbidden, new List<ApplicationOutLongDto>());

        var applications = await _appDbContext.Applications.ToListAsync();
        if (applications is null || applications.Count == 0)
            return new Tuple<ErrorCodes, List<ApplicationOutLongDto>>(ErrorCodes.NotFound, new List<ApplicationOutLongDto>());

        return new Tuple<ErrorCodes, List<ApplicationOutLongDto>>(ErrorCodes.Ok, applications.Select(a => new ApplicationOutLongDto
        {
            ApplicationId = a.ApplicationId,
            UserId = a.UserId,
            Description = a.Description,
            Status = a.Status
        }).ToList());
    }
    
    public async Task<ErrorCodes> UpdateApplicationStatus(UpdateApplicationStatusDto dto)

    {
        var email = _contextAccessor.HttpContext?.User?.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (email is null)
            return ErrorCodes.Unauthorized;

        var admin = await _userManager.FindByEmailAsync(email!);
        if (admin is null)
            return ErrorCodes.Unauthorized;

        var roles = await _userManager.GetRolesAsync(admin);
        if (!roles.Contains("Admin"))
            return ErrorCodes.Forbidden;

        var application = await _appDbContext.Applications
            .Where(a => a.ApplicationId == dto.ApplicationId)
            .FirstOrDefaultAsync();
        if (application is null)
            return ErrorCodes.NotFound;
        
        if (!Enum.IsDefined(typeof(ApplicationStatus), dto.Status))
            return ErrorCodes.BadArgument;
        
        application.Status = (ApplicationStatus)dto.Status;
        
        if (!string.IsNullOrEmpty(dto.Description))
        {
            ApplicationAnswer answer = new ApplicationAnswer
            {
                AdminId = admin.Id,
                Application = application,
                Description = dto.Description
            };
            application.Answer = answer;
            _appDbContext.ApplicationAnswers.Add(answer);
        }


        var res = await _appDbContext.SaveChangesAsync();
        if (res > 0)
            return ErrorCodes.Ok;
        return ErrorCodes.BadRequest;
    }
}
