using Chrono.Shared.Extensions;
using Chrono.Shared.Interfaces;
using Chrono.Shared.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace Chrono.Features.Categories;

[Authorize]
[HttpGet("api/categories")]
[Tags("Categories")]
public class GetCategoriesEndpoint(IApplicationDbContext context, ICurrentUserService currentUserService)
    : EndpointWithoutRequest<CategoryDto[]>
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = context.Categories
            .OrderBy(x => x.Name)
            .AsEnumerable()
            .Where(x => x.IsPermitted(currentUserService.UserId))
            .Select(CategoryDto.FromEntity)
            .ToArray();

        await SendOkAsync(result, ct);
    }
}
