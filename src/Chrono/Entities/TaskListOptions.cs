namespace Chrono.Entities;

public sealed class TaskListOptions
{
    public int Id { get; set; }
    public bool RequireBusinessValue { get; set; } = true;
    public bool RequireDescription { get; set; } = true;

    public int TaskListId { get; set; }
    public TaskList TaskList { get; set; }
}
