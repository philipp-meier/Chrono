using Microsoft.Playwright;
using NUnit.Framework;

namespace Chrono.Tests.E2E;

public partial class E2ETests
{
    [Test] [Order(3)]
    public async Task Create_Task()
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
        }).FillAsync("Test Task");

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
}
