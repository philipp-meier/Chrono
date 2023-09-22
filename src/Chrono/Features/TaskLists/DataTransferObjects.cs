using Chrono.Entities;
using Task = Chrono.Entities.Task;

namespace Chrono.Features.TaskLists;

public class TaskListDto
{
    public int Id { get; init; }
    public string Title { get; init; }
    public TaskDto[] Tasks { get; init; }

    public static TaskListDto FromEntity(TaskList taskList)
    {
        return new TaskListDto
        {
            Id = taskList.Id,
            Title = taskList.Title,
            Tasks = taskList.Tasks
                .Select(TaskDto.FromEntity)
                .ToArray()
        };
    }
}

public class TaskDto
{
    public int Id { get; init; }
    public int ListId { get; init; }
    public int Position { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public bool Done { get; init; }
    public CategoryDto[] Categories { get; init; }

    public static TaskDto FromEntity(Task task)
    {
        return new TaskDto
        {
            Id = task.Id,
            ListId = task.ListId,
            Position = task.Position,
            Name = task.Name,
            Description = task.Description,
            Done = task.Done,
            Categories = task.Categories
                .Select(CategoryDto.FromEntity)
                .ToArray()
        };
    }
}

public class CategoryDto
{
    public int Id { get; init; }
    public string Name { get; init; }

    public static CategoryDto FromEntity(TaskCategory taskCategory)
    {
        return new CategoryDto
        {
            Id = taskCategory.Category.Id, Name = taskCategory.Category.Name
        };
    }
}
