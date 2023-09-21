using Microsoft.Playwright;
using NUnit.Framework;

namespace Chrono.Tests.E2E;

public partial class E2ETests
{
    [Test] [Order(4)]
    public async Task Create_Category()
    {
        await _page.GotoAsync(_config["WebAppUrl"]!);
        await _page.Locator("text=Master Data").ClickAsync();
        await _page.Locator("text=Categories").ClickAsync();
        await _page.Locator("text=Add Category").ClickAsync();
        await _page.GetByRole(AriaRole.Textbox).FillAsync("Test Category");

        await _page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            Name = "Add", Exact = true
        }).ClickAsync();
    }

    [Test] [Order(8)]
    public async Task Delete_Category()
    {
        await _page.GotoAsync(_config["WebAppUrl"]!);
        await _page.Locator("text=Master Data").ClickAsync();
        await _page.Locator("text=Categories").ClickAsync();
        await _page.Locator("button[data-delete='Test Category']").First.ClickAsync();

        await _page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            Name = "OK", Exact = true
        }).ClickAsync();
    }
}
