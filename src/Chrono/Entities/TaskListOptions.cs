namespace Chrono.Entities;

public class TaskListOptions
{
    public int Id { get; set; }
    public bool RequireBusinessValue { get; set; } = true;
    public bool RequireDescription { get; set; } = true;

    public int TaskListId { get; set; }
    public TaskList TaskList { get; set; }
}
