using BerberApp_Backend.Domain.Users;
using Microsoft.AspNetCore.Identity;

namespace BerberApp_Backend.WebApi.Extensions;

public static class ExtensionsMiddleware
{
    public static void CreateFirstUser(WebApplication app)
    {
        using (var scoped = app.Services.CreateScope())
        {
            var userManager = scoped.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            if (!userManager.Users.Any(p => p.UserName == "admin"))
            {
                AppUser user = new()
                {
                    UserName = "admin",
                    Email = "admin@admin.com",
                    FirstName = "Emre Can",
                    LastName = "TERKAN",
                    EmailConfirmed = true,
                    CreateAt = DateTime.UtcNow
                };

                user.CreateUserId = user.Id;

                userManager.CreateAsync(user, "1").Wait();
            }
        }
    }
}
