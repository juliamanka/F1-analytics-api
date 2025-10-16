using F1Analytics.Database.Models;

namespace F1Analytics.Services;

public interface IAuthService
{
    public Task<string> GenerateJwtTokenAsync(User user);

}