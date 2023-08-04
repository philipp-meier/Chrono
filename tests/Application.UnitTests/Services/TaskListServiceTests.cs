using Chrono.Application.Common.Services;
using Chrono.Application.Entities;
using FluentAssertions;
using NUnit.Framework;
using Task = Chrono.Application.Entities.Task;

namespace Chrono.Application.UnitTests.Services;

public class TaskListServiceTests
{
    [Test]
    public void ReorderTaskPositions()
    {
        var taskList = new TaskList
        {
            Title = "Test",
            Tasks = new List<Task>() {
                new() { Name = "A", Position = 3, Done = false },
                new() { Name = "B", Position = 5, Done = false },
                new() { Name = "C", Position = 6, Done = false },
                new() { Name = "D", Position = 8, Done = false },
                new() { Name = "E", Position = 5, Done = true },
                new() { Name = "F", Position = 9, Done = true },
                new() { Name = "G", Position = 10, Done = true },
            }
        };

        TaskListService.ReorderTaskPositions(taskList);

        // Open tasks: Starting from 1 to 4.
        for (var i = 0; i <= 3; i++)
            taskList.Tasks.ElementAt(i).Position.Should().Be(i + 1);

        // Closed tasks: Starting from 1 to 3.
        for (var i = 4; i <= 6; i++)
            taskList.Tasks.ElementAt(i - 4).Position.Should().Be(i - 4 + 1);
    }

    [Test]
    public void InsertAt()
    {
        var taskList = new TaskList
        {
            Title = "Test",
            Tasks = new List<Task>() {
                new() { Name = "A", Position = 1 },
                new() { Name = "B", Position = 2 },
                new() { Name = "C", Position = 3 },
                new() { Name = "D", Position = 4 },
            }
        };

        var newTask = new Task { Name = "Test" };
        TaskListService.InsertAt(position: 2, newTask, targetTaskList: taskList);

        var tasks = taskList.Tasks.OrderBy(x => x.Position).ToArray();
        tasks.ElementAt(0).Name.Should().Be("A");
        tasks.ElementAt(1).Name.Should().Be("Test");
        tasks.ElementAt(2).Name.Should().Be("B");

        TaskListService.InsertAt(position: 4, newTask);
        tasks = taskList.Tasks.OrderBy(x => x.Position).ToArray();
        tasks.ElementAt(0).Name.Should().Be("A");
        tasks.ElementAt(1).Name.Should().Be("B");
        tasks.ElementAt(2).Name.Should().Be("C");
        tasks.ElementAt(3).Name.Should().Be("Test");
        tasks.ElementAt(4).Name.Should().Be("D");
    }
}
