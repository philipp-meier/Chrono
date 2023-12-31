﻿using Chrono.Entities;
using Task = Chrono.Entities.Task;

namespace Chrono.Shared.Extensions;

public static class TaskListExtensions
{
    public static void ReorderTaskPositions(this TaskList taskList)
    {
        ArgumentNullException.ThrowIfNull(taskList);

        var allTasks = taskList.Tasks
            .OrderBy(x => x.Position)
            .ToArray();

        EnsureTaskPositionsWithinGroup(allTasks, x => !x.Done);
        EnsureTaskPositionsWithinGroup(allTasks, x => x.Done);
        return;

        void EnsureTaskPositionsWithinGroup(IEnumerable<Task> tasks, Func<Task, bool> taskGroupFilter)
        {
            var filteredTasks = tasks.Where(taskGroupFilter).ToArray();
            for (var i = 0; i < filteredTasks.Length; i++)
            {
                filteredTasks[i].Position = i + 1;
            }
        }
    }

    public static void InsertAt(this TaskList taskList, int position, Task task)
    {
        if (taskList != null && task.List != taskList)
        {
            task.List = taskList;
            taskList.Tasks.Add(task);
        }

        var tasks = taskList!.Tasks
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
