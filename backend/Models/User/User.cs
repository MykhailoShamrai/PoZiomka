using Microsoft.AspNetCore.Identity;
namespace backend.Models.User;

public class User: IdentityUser<int>
{
    public string? StudentIndex{ get; set; } = null;
    public UInt64 Preferences { get; set; }
}