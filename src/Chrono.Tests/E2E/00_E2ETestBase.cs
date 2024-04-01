using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
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
    private IHost _host;
    private IPage _page;

    [OneTimeSetUp]
    public async Task Init()
    {
        _config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("config.json", false)
            .AddJsonFile("config.Local.json", true)
            .Build();

        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        Environment.SetEnvironmentVariable("ASPNETCORE_HOSTINGSTARTUPASSEMBLIES", "Microsoft.AspNetCore.SpaProxy");
        Environment.SetEnvironmentVariable("CHRONO_E2E_TESTING", "true");
        Environment.SetEnvironmentVariable("ConnectionStrings:DefaultConnection",
            $"Data Source={_config["DatabaseFullPath"]}");

        _host = Program.BuildApp([$"urls={_config["BackendUrl"]!}"]);

        await _host.StartAsync();

        await InitPlaywright();
    }

    private async Task InitPlaywright()
    {
        // Ensure the required web driver is installed.
        Microsoft.Playwright.Program.Main(PlaywrightArgs);

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

    [OneTimeTearDown]
    public async Task TearDownWebApplication()
    {
        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }
    }
}
