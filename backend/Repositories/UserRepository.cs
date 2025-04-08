using backend.Data;
using backend.Dto;
using backend.Interfaces;
using backend.Models.User;
using Microsoft.AspNetCore.Identity;

namespace backend.Repositories;

public enum UserErrorCodes
{
    UserNotFound,
    UserAlreadyExists,
    Ok,
}

public class UserRepository : IUserInterface
{
    private readonly AppDbContext _appContext;
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _contextAccessor;

    public UserRepository(AppDbContext appContext, UserManager<User> userManager, IHttpContextAccessor contextAccessor)
    {
        _appContext = appContext;
        _userManager = userManager;
        _contextAccessor = contextAccessor;
    }
    
    public async Task<Tuple<UserErrorCodes, ProfileDisplay>> DisplayUserProfile(string email)
    {
        throw new NotImplementedException();
    }
    
    public async Task<UserErrorCodes> ChangeUserPreferences(DisplayPreferences displayPreferences)
    {
        throw new NotImplementedException();
    }
}