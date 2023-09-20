using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace Chrono.Tests;

[Parallelizable(ParallelScope.None)]
public class EndToEndTests : PlaywrightTest
{
    private const string WebAppUrl = "https://localhost:7151";
    private IBrowser _browser;
    private IConfiguration _config;
    private IPage _page;

    [OneTimeSetUp]
    public async Task Init()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("config.json", false)
            .AddJsonFile("config.Local.json", true);

        _config = builder.Build();

        await InitPlaywright();
        await Login_User();
    }

    private async Task InitPlaywright()
    {
        // Ensure the required web driver is installed.
        Program.Main(new[]
        {
            "install"
        });

        await PlaywrightSetup();

        _browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = _config.GetValue("Options:Headless", false)
        });
        _page = await _browser.NewPageAsync(new BrowserNewPageOptions
        {
            IgnoreHTTPSErrors = _config.GetValue("Options:IgnoreHTTPSErrors", false)
        });
    }

    private async Task Login_User()
    {
        await _page.GotoAsync(WebAppUrl);
        await _page.Locator("text=Login").ClickAsync();

        await _page.GetByLabel("Email address").FillAsync(_config["TestUser:Username"]!);
        await _page.GetByLabel("Password").FillAsync(_config["TestUser:Password"]!);
        await _page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            Name = "Continue"
        }).ClickAsync();
    }

    [Test] [Order(1)]
    public async Task Create_TaskList()
    {
        await _page.GotoAsync(WebAppUrl);
        await _page.Locator("text=Master Data").ClickAsync();
        await _page.Locator("text=Add Task List").ClickAsync();
        await _page.GetByRole(AriaRole.Textbox).FillAsync("Test Task List");

        await _page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            Name = "Add", Exact = true
        }).ClickAsync();
    }

    [Test] [Order(2)]
    public async Task Create_Task()
    {
        await _page.GotoAsync(WebAppUrl);
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
