using Chrono.Shared.Exceptions;
using Chrono.Shared.Extensions;
using Chrono.Shared.Interfaces;
using Chrono.Shared.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Features.Categories;

public record DeleteCategory(int Id);

[Authorize]
[HttpDelete("api/categories/{id:int}")]
[Tags("Categories")]
public class DeleteCategoryEndpoint(IApplicationDbContext context, ICurrentUserService currentUserService)
    : Endpoint<DeleteCategory, int>
{
    public override async Task HandleAsync(DeleteCategory request, CancellationToken ct)
    {
        var entity = await context.Categories
            .Include(x => x.Tasks)
            .SingleOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity == null)
        {
            throw new NotFoundException($"Category \"{request.Id}\" not found.");
        }

        if (!entity.IsPermitted(currentUserService.UserId))
        {
            throw new ForbiddenAccessException();
        }

        // Ensures only the TaskCategories are getting deleted and not the actual tasks too.
        context.TaskCategories.RemoveRange(entity.Tasks);
        context.Categories.Remove(entity);

        await context.SaveChangesAsync(ct);

        await SendOkAsync(ct);
    }
}
