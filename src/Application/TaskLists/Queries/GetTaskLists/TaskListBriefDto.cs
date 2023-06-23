using Chrono.Domain.Entities;

namespace Chrono.Application.TaskLists.Queries.GetTaskLists;

public class TaskListBriefDto
{
    public int Id { get; init; }
    public string Title { get; init; }

    public static TaskListBriefDto FromEntity(TaskList task) => new() { Id = task.Id, Title = task.Title };
}
