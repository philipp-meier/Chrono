using Chrono.Domain.Entities;
using Chrono.Application.Common.Dtos;

namespace Chrono.Application.TaskLists.Queries.GetTaskList;

public class TaskListDto
{
    public int Id { get; init; }
    public string Title { get; init; }
    public IReadOnlyCollection<TaskDto> Tasks { get; init; }

    public static TaskListDto FromEntity(TaskList taskList)
    {
        return new TaskListDto
        {
            Id = taskList.Id,
            Title = taskList.Title,
            Tasks = taskList.Tasks.Select(TaskDto.FromEntity).ToArray()
        };
    }
}
