using Chrono.Features.Tasks;

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

    public static CategoryDto FromEntity(TaskCategory taskCategory)
    {
        return new CategoryDto
        {
            Id = taskCategory.Category.Id, Name = taskCategory.Category.Name
        };
    }
}
