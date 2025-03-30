using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

internal class AuthDbInitializer
{
    internal static async Task Initialize(AuthDbContext authDbContext, RoleManager<IdentityRole<int>> roleManager)
    {
        if (authDbContext is null)
        {
            throw new ArgumentNullException("AuthDbContext is null");
        }
        var roles = new[] {"Student", "Admin"};
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(role));
            }
        }
    }
}