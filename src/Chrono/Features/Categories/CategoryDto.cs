using Chrono.Entities;

namespace Chrono.Features.Categories;

public class CategoryDto
{
    public int Id { get; init; }
    public string Name { get; init; }

    public static CategoryDto FromEntity(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id, Name = category.Name
        };
    }
}
