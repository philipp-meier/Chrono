using Microsoft.Playwright;
using NUnit.Framework;

namespace Chrono.Tests.E2E;

public partial class E2ETests
{
    [Test] [Order(5)]
    [TestCase("Test Task 1")]
    [TestCase("Test Task 2")]
    public async Task Add_Tasks_To_TaskList(string taskName)
    {
        await _page.GotoAsync(_config["WebAppUrl"]!);
        await _page.Locator("text=Lists").ClickAsync();
        await _page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            Name = "Add Task", Exact = true
        }).ClickAsync();

        await _page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions
        {
            Name = "Name", Exact = true
        }).FillAsync(taskName);

        await _page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions
        {
            Name = "Business value", Exact = true
        }).FillAsync("QA");

        await _page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions
        {
            Name = "Description", Exact = true
        }).FillAsync("Test description");

        await _page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            Name = "Save"
        }).ClickAsync();
    }


    [Test] [Order(6)]
    [TestCase("Test Task 1")]
    public async Task Edit_Task_From_TaskList(string taskName)
    {
        await _page.GotoAsync(_config["WebAppUrl"]!);
        await _page.Locator("text=Lists").ClickAsync();
        await _page.Locator($"text={taskName}").ClickAsync();

        await _page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions
        {
            Name = "Name", Exact = true
        }).FillAsync(taskName + " - edited");

        await _page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            Name = "Save"
        }).ClickAsync();
    }
}
