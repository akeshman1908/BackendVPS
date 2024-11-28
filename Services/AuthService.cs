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

    public async Task<string?> SignInAsync(string username, string password)
    {
        // Zoek gebruiker in de database
        var user = await _userRepository.GetAllUsersAsync();
        var foundUser = user.FirstOrDefault(u => u.username == username);

        // Controleer of de gebruiker bestaat en het wachtwoord correct is
        if (foundUser == null || !VerifyPassword(password, foundUser.password))
            return null;

        // Genereer JWT-token
        return GenerateJwtToken(foundUser);
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
    
    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes("bC23e45r@dFGkLmNpQrStu1234vWxYz12345"); // Zorg dat dit geheim blijft
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                new Claim(ClaimTypes.Name, user.username),
                new Claim(ClaimTypes.Role, user.rolid.ToString()) // Voeg claims toe indien nodig
            }),
            Expires = DateTime.UtcNow.AddHours(1), // Token geldig voor 1 uur
            Issuer = "http://87.106.130.88:80", // Hetzelfde als in je configuratie
            Audience = "my-app-backend", // Hetzelfde als in je configuratie
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

}