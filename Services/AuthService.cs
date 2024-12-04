using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using backend.Domain;
using backend.Repo;
using backend.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace backend.Services;

public class AuthService : IAuthService
{
    private readonly UserRepository _userRepository;

    public AuthService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> SignInAsync(string username, string password)
    {
        // Zoek gebruiker in de database
        var users = await _userRepository.GetAllUsersAsync();
        var foundUser = users.FirstOrDefault(u => u.username == username);

        // Controleer of de gebruiker bestaat en het wachtwoord correct is
        if (foundUser == null || !VerifyPassword(password, foundUser.password))
            return null;

        // Retourneer de gevonden gebruiker
        return foundUser;
    }


    public async Task<bool> SignUpAsync(User user)
    {
        user.password = HashPassword(user.password);
        await _userRepository.AddUserAsync(user);
        return true;
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
    
}