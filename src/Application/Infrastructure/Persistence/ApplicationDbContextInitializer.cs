using Chrono.Application.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace Chrono.Application.Infrastructure.Persistence;

public class ApplicationDbContextInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ApplicationDbContextInitializer> _logger;

    public ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger,
        ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task InitializeAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while initializing the database");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            // Default data
            if (!_context.Categories.Any())
            {
                _context.Categories.AddRange(
                    new Category { Name = "Quality Improvement" },
                    new Category { Name = "Security" },
                    new Category { Name = "Performance" },
                    new Category { Name = "Documentation" },
                    new Category { Name = "Time Saving" },
                    new Category { Name = "Mentoring" }
                );

                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while seeding the database");
            throw;
        }
    }
}
