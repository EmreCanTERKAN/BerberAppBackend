using BerberApp_Backend.Domain.Users;

namespace BerberApp_Backend.Application.Services;
public interface IJwtProvider 
{
    public Task<string> CreateTokenAsync(AppUser user, string password, CancellationToken cancellationToken = default);
}
