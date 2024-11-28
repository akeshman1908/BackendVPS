using System.Text;
using backend.Repo;
using backend.Repo.DbContext;
using backend.Services;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Voeg services toe aan de container
builder.Services.AddControllers();

// Configureer Kestrel om specifieke poorten en HTTPS te gebruiken
builder.WebHost.ConfigureKestrel(options =>
{
    // Luister op poort 5115 voor HTTP
    options.ListenAnyIP(5115);

    // Luister op poort 7233 voor HTTPS
    options.ListenAnyIP(7233, listenOptions =>
    {
        listenOptions.UseHttps(); // HTTPS inschakelen
    });
});
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<UserRepository>();
// Configureer de DbContext met PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)); // Gebruik UseSqlServer voor SQL Server
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "http://87.106.130.88:80", // Vervang door jouw applicatie URL of naam
        ValidAudience = "my-app-backend"
        , // Vervang door jouw client-ID
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("bC23e45r@dFGkLmNpQrStu1234vWxYz12345")) // Sterk geheim
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configureer de HTTP-pijplijn
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Middleware om HTTPS-afdwingen mogelijk te maken
app.UseHttpsRedirection();



// Routing instellen
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
