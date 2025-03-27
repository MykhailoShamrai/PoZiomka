using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

internal static class AuthDbInitializerExtension
{
    public static async Task InitializeAuthContext(this IApplicationBuilder app)
    {
        if (app is null)
        {
            throw new ArgumentNullException("Application builder is null!");
        }       

        using var scope = app.ApplicationServices.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var context = serviceProvider.GetRequiredService<AuthDbContext>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

        await AuthDbInitializer.Initialize(context, roleManager);
    }
}