using Microsoft.Playwright;
using NUnit.Framework;

namespace Chrono.Tests.E2E;

public partial class E2ETests
{
    [Test] [Order(2)]
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
}
