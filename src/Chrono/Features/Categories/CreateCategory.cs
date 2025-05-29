using Chrono.Entities;
using Chrono.Shared.Interfaces;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Task = System.Threading.Tasks.Task;

namespace Chrono.Features.Categories;

public record CreateCategory(string Name);

[Authorize]
[HttpPost("api/categories")]
[Tags("Categories")]
public class CreateCategoryEndpoint(IApplicationDbContext context) : Endpoint<CreateCategory, int>
{
    public override async Task HandleAsync(CreateCategory request, CancellationToken ct)
    {
        var entity = new Category { Name = request.Name };
        context.Categories.Add(entity);

        await context.SaveChangesAsync(ct);

        await SendCreatedAtAsync<GetCategoriesEndpoint>(null, entity.Id, cancellation: ct);
    }
}
