using NUnit.Framework;
using FluentAssertions;
using Chrono.Domain.Entities;
using Chrono.Domain.Services;
using Task = Chrono.Domain.Entities.Task;

namespace Chrono.Domain.UnitTests.Services;

public class TaskListServiceTests
{
    [Test]
    public void ReorderTaskPositions()
    {
        var taskList = new TaskList
        {
            Title = "Test",
            Tasks = new List<Task>() {
                new Task { Name = "A", Position = 3, Done = false },
                new Task { Name = "B", Position = 5, Done = false },
                new Task { Name = "C", Position = 6, Done = false },
                new Task { Name = "D", Position = 8, Done = false },
                new Task { Name = "E", Position = 5, Done = true },
                new Task { Name = "F", Position = 9, Done = true },
                new Task { Name = "G", Position = 10, Done = true },
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
                new Task { Name = "A", Position = 1 },
                new Task { Name = "B", Position = 2 },
                new Task { Name = "C", Position = 3 },
                new Task { Name = "D", Position = 4 },
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
