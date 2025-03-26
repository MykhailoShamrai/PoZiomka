using Microsoft.AspNetCore.Identity;

namespace backend.Models.Users;

public class User: IdentityUser<int>
{
    public string? StudentIndex{ get; set; } = null;
}