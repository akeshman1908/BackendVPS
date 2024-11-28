using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
app.MapControllers();

app.Run();
