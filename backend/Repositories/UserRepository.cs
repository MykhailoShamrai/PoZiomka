using backend.Data;
using backend.Dto;
using backend.Interfaces;
using backend.Models.User;
using Microsoft.AspNetCore.Identity;

namespace backend.Repositories;

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
    
    public Task ChangeUserPreferences(DisplayPreferences displayPreferences)
    {
        throw new NotImplementedException();
    }

    public Task<ProfileDisplay> DisplayUserProfile(string email)
    {
        throw new NotImplementedException();
    }
}