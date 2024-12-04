using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using backend.Services.Interfaces;
using Microsoft.AspNetCore;

namespace backend.Controllers.AuthenticatieControllers;
[ApiController]
[Route("connect/token")]
public class TokenController : Controller
{
    private readonly IAuthService _authService;

    public TokenController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest();

        if (request == null)
            throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        if (request.IsPasswordGrantType())
        {
            var user = await _authService.SignInAsync(request.Username, request.Password);

            if (user == null)
                return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            var claims = new List<Claim>
            {
                new Claim(OpenIddictConstants.Claims.Subject, user.id.ToString()),
                new Claim(OpenIddictConstants.Claims.Username, user.username),
                new Claim(OpenIddictConstants.Claims.Email, user.email)
            };

            // Voeg rollen of andere claims toe indien nodig
            claims.Add(new Claim(ClaimTypes.Role, user.rolid.ToString()));

            var claimsIdentity = new ClaimsIdentity(claims, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Stel scopes in
            claimsPrincipal.SetScopes(new[]
            {
                OpenIddictConstants.Scopes.OpenId,
                OpenIddictConstants.Scopes.Email,
                OpenIddictConstants.Scopes.Profile
            }.Intersect(request.GetScopes()));

            return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        throw new InvalidOperationException("The specified grant type is not supported.");
    }
}
