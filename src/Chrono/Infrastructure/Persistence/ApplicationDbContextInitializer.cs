using Microsoft.EntityFrameworkCore;

namespace Chrono.Infrastructure.Persistence;

public class ApplicationDbContextInitializer(
    ILogger<ApplicationDbContextInitializer> logger,
    ApplicationDbContext context)
{
    public async Task InitializeAsync()
    {
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occured while initializing the database");
            throw;
        }
    }
}
