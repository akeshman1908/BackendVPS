using backend.Domain;

namespace backend.Services.Interfaces;

public interface IAuthService
{
    Task<string?> SignInAsync(string username, string password);
    Task<bool> SignUpAsync(User user);
}