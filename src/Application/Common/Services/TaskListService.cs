using Chrono.Application.Entities;
using Task = Chrono.Application.Entities.Task;

namespace Chrono.Application.Common.Services;

public static class TaskListService
{
    public static void ReorderTaskPositions(TaskList taskList)
    {
        if (taskList == null)
        {
            throw new ArgumentNullException(nameof(taskList));
        }

        var allTasks = taskList.Tasks
            .OrderBy(x => x.Position)
            .ToArray();

        EnsureTaskPositionsWithinGroup(allTasks, x => !x.Done);
        EnsureTaskPositionsWithinGroup(allTasks, x => x.Done);

        void EnsureTaskPositionsWithinGroup(IEnumerable<Task> tasks, Func<Task, bool> taskGroupFilter)
        {
            var filteredTasks = tasks.Where(taskGroupFilter).ToArray();
            for (var i = 0; i < filteredTasks.Length; i++)
            {
                filteredTasks[i].Position = i + 1;
            }
        }
    }

    public static void InsertAt(int position, Task task, TaskList targetTaskList = null)
    {
        var taskList = targetTaskList ?? task?.List;
        if (taskList == null)
        {
            throw new ArgumentNullException(nameof(taskList));
        }

        if (targetTaskList != null && task.List != targetTaskList)
        {
            task.List = targetTaskList;
            targetTaskList.Tasks.Add(task);
        }

        var tasks = taskList.Tasks
            .Where(x => x.Done == task.Done)
            .OrderBy(x => x.Position)
            .ToArray();

        var newPosition = position;
        if (newPosition > tasks.Length)
        {
            newPosition = tasks.Length;
        }
        else if (newPosition < 1)
        {
            newPosition = 1;
        }

        var positionCount = 0;
        foreach (var currentTask in tasks)
        {
            if (currentTask == task)
            {
                continue;
            }

            positionCount++;

            // Skip this position.
            if (positionCount == newPosition)
            {
                positionCount++;
            }

            currentTask.Position = positionCount;
        }

        task.Position = newPosition;
    }
}
