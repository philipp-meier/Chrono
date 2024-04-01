using Chrono.Infrastructure.Persistence;
using Serilog;

namespace Chrono;

public class Program
{
    private static async Task Main(string[] args)
    {
        var app = BuildApp(args);

        using (var scope = app.Services.CreateScope())
        {
            var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
            await initializer.InitializeAsync();
        }

        await app.RunAsync();
    }

    public static WebApplication BuildApp(string[] args = default)
    {
        var builder = WebApplication.CreateBuilder(args!);
        builder.Configuration.AddEnvironmentVariables();
        builder.WebHost.UseKestrel(option => option.AddServerHeader = false);

        // Add services to the container.
        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddApplicationServices(builder.Environment.IsDevelopment());
        builder.Services.AddWebUiServices(builder.Configuration);

        // Logging
        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(logger);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            // https://localhost:7151/swagger/index.html
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseForwardedHeaders();

        // Other security headers are appended by the reverse proxy (see OWASP recommendations).
        app.UseHsts();

        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapFallbackToFile("index.html");

        return app;
    }
}
