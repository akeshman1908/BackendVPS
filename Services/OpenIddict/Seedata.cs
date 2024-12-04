using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace backend.Services.OpenIddict;


public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        // Check if the client already exists
        if (await manager.FindByClientIdAsync("my-client-id") == null)
        {
            var descriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = "my-client-id",
                DisplayName = "My React Frontend",
                ClientType = ClientTypes.Public,
                 // Use Public for clients like React frontend

                // Define the permissions for the client
                Permissions =
                {
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.Password,
                    Permissions.GrantTypes.RefreshToken,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Roles,
                    Permissions.Prefixes.Scope + "api"
                },

                // Redirect URIs are not needed for password flow, but can be set if using authorization code flow
                RedirectUris =
                {
                    new Uri("https://your-frontend-domain.com/callback") // Replace with your frontend's callback URI if needed
                }
            };

            await manager.CreateAsync(descriptor);
        }
    }
}
