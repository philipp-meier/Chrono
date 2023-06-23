using Task = Chrono.Domain.Entities.Task;

namespace Chrono.Application.Common.Dtos;

public class TaskDto
{
    public int Id { get; init; }
    public int ListId { get; init; }
    public int Position { get; init; }
    public string Name { get; init; }
    public string BusinessValue { get; init; }
    public string Description { get; init; }
    public bool Done { get; init; }
    public IReadOnlyCollection<CategoryDto> Categories { get; init; }

    public string LastModifiedBy { get; init; }
    public DateTime LastModified { get; init; }

    public static TaskDto FromEntity(Task task)
    {
        return new TaskDto
        {
            Id = task.Id,
            ListId = task.ListId,
            Position = task.Position,
            Name = task.Name,
            BusinessValue = task.BusinessValue,
            Description = task.Description,
            Done = task.Done,
            Categories = task.Categories
                .Select(CategoryDto.FromEntity)
                .ToArray(),

            LastModified = DateTime.SpecifyKind(task.LastModified, DateTimeKind.Utc),
            LastModifiedBy = task.LastModifiedBy.Name
        };
    }
}
