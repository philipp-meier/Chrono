using Chrono.Domain.Entities;

namespace Chrono.Application.Common.Dtos;

public class CategoryDto
{
    public int Id { get; init; }
    public string Name { get; init; }

    public static CategoryDto FromEntity(Category category) => new() { Id = category.Id, Name = category.Name };

    public static CategoryDto FromEntity(TaskCategory taskCategory) => new() { Id = taskCategory.Category.Id, Name = taskCategory.Category.Name };
}
