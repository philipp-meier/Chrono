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

    [Test] [Order(2)]
    public async Task Edit_TaskList()
    {
        await _page.GotoAsync(_config["WebAppUrl"]!);
        await _page.Locator("text=Master Data").ClickAsync();
        await _page.Locator("button[data-edit='Test Task List']").First.ClickAsync();
        await _page.GetByRole(AriaRole.Textbox).FillAsync("Test Task List - edited");

        await _page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            Name = "Save", Exact = true
        }).ClickAsync();
    }

    [Test] [Order(3)]
    public async Task Set_As_Default_TaskList()
    {
        await _page.GotoAsync(_config["WebAppUrl"]!);
        await _page.Locator("text=Master Data").ClickAsync();
        await _page.Locator("button[data-edit='Test Task List - edited']").First.ClickAsync();

        await _page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            Name = "Set as default", Exact = true
        }).ClickAsync();
    }

    [Test] [Order(7)]
    public async Task Unset_Default_TaskList()
    {
        await _page.GotoAsync(_config["WebAppUrl"]!);
        await _page.Locator("text=Master Data").ClickAsync();
        await _page.Locator("button[data-edit='Test Task List - edited']").First.ClickAsync();

        await _page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            Name = "Unset default", Exact = true
        }).ClickAsync();
    }

    [Test] [Order(8)]
    public async Task Delete_TaskList()
    {
        await _page.GotoAsync(_config["WebAppUrl"]!);
        await _page.Locator("text=Master Data").ClickAsync();
        await _page.Locator("button[data-delete='Test Task List - edited']").First.ClickAsync();

        await _page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            Name = "OK", Exact = true
        }).ClickAsync();
    }
}
