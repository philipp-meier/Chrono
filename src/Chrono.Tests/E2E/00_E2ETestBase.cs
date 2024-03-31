using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace Chrono.Tests.E2E;

[Parallelizable(ParallelScope.None)]
[Category("E2E")]
public partial class E2ETests : PlaywrightTest
{
    private static readonly string[] PlaywrightArgs = ["install"];
    private IBrowser _browser;
    private IConfiguration _config;
    private IPage _page;

    [OneTimeSetUp]
    public async Task Init()
    {
        _config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("config.json", false)
            .AddJsonFile("config.Local.json", true)
            .Build();

        await InitPlaywright();
    }

    private async Task InitPlaywright()
    {
        // Ensure the required web driver is installed.
        Program.Main(PlaywrightArgs);

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
}
