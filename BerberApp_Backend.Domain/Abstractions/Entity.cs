using BerberApp_Backend.Domain.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BerberApp_Backend.Domain.Abstractions;
public abstract class Entity
{
    public Entity()
    {
        Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }
    public DateTimeOffset CreateAt { get; set; }
    public Guid CreateUserId { get; set; } = default!;
    public string CreateUserName => GetCreateUserName();
    public DateTimeOffset UpdateAt { get; set; }
    public Guid? UpdateUserId { get; set; }
    public string UpdateUserName => GetUpdateUserName();
    public DateTimeOffset DeleteAt { get; set; }
    public Guid? DeleteUserId { get; set; }
    public string DeleteUserName => GetDeleteUserName();
    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; }


    private string GetCreateUserName()
    {
        HttpContextAccessor httpContextAccessor = new();
        var userManager = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<UserManager<AppUser>>();

        AppUser appUser = userManager.Users.First(p => p.Id == CreateUserId);
        return appUser.FirstName + " " + appUser.LastName + " (" + appUser.Email + ")";
    }
    private string GetUpdateUserName()
    {
        if (UpdateUserId is null)
        {
            return string.Empty;
        }   
        HttpContextAccessor httpContextAccessor = new();
        var userManager = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<UserManager<AppUser>>();

        AppUser appUser = userManager.Users.First(p => p.Id == UpdateUserId);
        return appUser.FirstName + " " + appUser.LastName + " (" + appUser.Email + ")";
    }
    private string GetDeleteUserName()
    {
        if (DeleteUserId is null)
        {
            return string.Empty;
        }
        HttpContextAccessor httpContextAccessor = new();
        var userManager = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<UserManager<AppUser>>();

        AppUser appUser = userManager.Users.First(p => p.Id == DeleteUserId);
        return appUser.FirstName + " " + appUser.LastName + " (" + appUser.Email + ")";
    }
}
