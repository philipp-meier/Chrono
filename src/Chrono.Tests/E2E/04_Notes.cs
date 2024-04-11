using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;

namespace Chrono.Tests.E2E;

public partial class E2ETests
{
    [Test]
    public async Task Add_Edit_Pin_Unpin_Delete_Notes()
    {
        await _page.GotoAsync(_config["WebAppUrl"]!);

        // Add first note
        await _page.GetByText("Notes").ClickAsync();
        await _page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Add Note" }).ClickAsync();

        await _page.GetByPlaceholder("Title").ClickAsync();
        await _page.GetByPlaceholder("Title").FillAsync("Test Note");

        await _page.GetByPlaceholder("Content").ClickAsync();
        await _page.GetByPlaceholder("Content").FillAsync("This is a test note.\n- A\n- B\n- C");

        await _page.GetByText("Preview").ClickAsync();
        await Expect(_page.GetByText("A", new PageGetByTextOptions { Exact = true })).ToBeVisibleAsync();
        await Expect(_page.GetByText("B", new PageGetByTextOptions { Exact = true })).ToBeVisibleAsync();
        await Expect(_page.GetByText("C", new PageGetByTextOptions { Exact = true })).ToBeVisibleAsync();

        await _page.GetByText("Write").ClickAsync();

        await _page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Save" }).ClickAsync();
        await _page.GetByRole(AriaRole.Listbox).ClickAsync();
        await _page.GetByRole(AriaRole.Option, new PageGetByRoleOptions { Name = "Save & Close" }).ClickAsync();

        await Expect(_page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Test Note" }))
            .ToBeVisibleAsync();

        // Add second note
        await _page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Add Note" }).ClickAsync();

        await _page.GetByPlaceholder("Title").ClickAsync();
        await _page.GetByPlaceholder("Title").FillAsync("Test Note 2");

        await _page.GetByPlaceholder("Content").ClickAsync();
        await _page.GetByPlaceholder("Content").FillAsync("#2");

        await _page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Save" }).ClickAsync();
        await _page.GetByRole(AriaRole.Listbox).ClickAsync();
        await _page.GetByText("Save & Close").ClickAsync();

        await Expect(_page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Test Note 2" }))
            .ToBeVisibleAsync();

        await _page.Locator("div").Filter(new LocatorFilterOptions { HasTextRegex = new Regex("^Test Note$") })
            .Locator("i")
            .ClickAsync();

        await Expect(_page.GetByTitle("Unpin")).ToBeVisibleAsync();

        await _page.GetByTitle("Unpin").ClickAsync();

        // Delete second note
        await _page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Test Note 2" }).ClickAsync();
        await _page.GetByRole(AriaRole.Listbox).ClickAsync();
        await _page.GetByRole(AriaRole.Option, new PageGetByRoleOptions { Name = "Delete note" }).ClickAsync();
        await _page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "OK" }).ClickAsync();

        // Delete first note
        await _page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Test Note" }).ClickAsync();
        await _page.GetByRole(AriaRole.Listbox).ClickAsync();
        await _page.GetByText("Delete note").ClickAsync();
        await _page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "OK" }).ClickAsync();
    }
}
