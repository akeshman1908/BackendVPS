using backend.Domain;

namespace backend.Services.Interfaces;

public interface IAuthService
{
    Task<User?> SignInAsync(string username, string password);
    Task<bool> SignUpAsync(User user);
}