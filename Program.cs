using System.Text;
using backend.Repo;
using backend.Repo.DbContext;
using backend.Services;
using backend.Services.Interfaces;
using backend.Services.OpenIddict;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure Kestrel to listen on specific ports and use HTTPS
builder.WebHost.ConfigureKestrel(options =>
{
    // Listen on port 5115 for HTTP (development purposes)
    options.ListenAnyIP(5115);

    // Listen on port 7233 for HTTPS
    options.ListenAnyIP(7233, listenOptions =>
    {
        listenOptions.UseHttps(); // Enable HTTPS
    });
});

// Get the connection string from configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Configure the DbContext with PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));

    // Registreer de OpenIddict entiteiten in het model
    options.UseOpenIddict();
});

// Configureer OpenIddict
builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        // Gebruik Entity Framework Core voor OpenIddict
        options.UseEntityFrameworkCore()
            .UseDbContext<ApplicationDbContext>();
    })
    .AddServer(options =>
    {
        options.SetAuthorizationEndpointUris("/connect/authorize")
            .SetTokenEndpointUris("/connect/token");

        options.AllowPasswordFlow()
            .AllowRefreshTokenFlow();

        options.AddDevelopmentEncryptionCertificate()
            .AddDevelopmentSigningCertificate();

        options.UseAspNetCore()
            .EnableTokenEndpointPassthrough();
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });

// Configure Authentication (handled by OpenIddict)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
});

// Add Authorization
builder.Services.AddAuthorization();

// Add your custom services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<UserRepository>();

// Configure CORS (adjust the origin as needed)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("https://your-frontend-domain.com") // Replace with your React app's domain
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

var app = builder.Build();

// Seed the client application into the database
await SeedData.InitializeAsync(app.Services);

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowSpecificOrigin");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
