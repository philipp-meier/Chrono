using Chrono.Domain.Entities;

namespace Chrono.Application.TaskLists.Queries.GetTaskListOptions;

public class TaskListOptionsDto
{
    public bool RequireBusinessValue { get; set; }
    public bool RequireDescription { get; set; }

    public static TaskListOptionsDto FromEntity(TaskListOptions taskListOptions)
    {
        return new TaskListOptionsDto
        {
            RequireBusinessValue = taskListOptions?.RequireBusinessValue ?? true,
            RequireDescription = taskListOptions?.RequireDescription ?? true
        };
    }
}
