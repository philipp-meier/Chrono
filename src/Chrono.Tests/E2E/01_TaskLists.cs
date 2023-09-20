using Microsoft.Playwright;
using NUnit.Framework;

namespace Chrono.Tests.E2E;

public partial class E2ETests
{
    [Test] [Order(1)]
    public async Task Create_TaskList()
    {
        await _page.GotoAsync(_config["WebAppUrl"]!);
        await _page.Locator("text=Master Data").ClickAsync();
        await _page.Locator("text=Add Task List").ClickAsync();
        await _page.GetByRole(AriaRole.Textbox).FillAsync("Test Task List");

        await _page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            Name = "Add", Exact = true
        }).ClickAsync();
    }
}
