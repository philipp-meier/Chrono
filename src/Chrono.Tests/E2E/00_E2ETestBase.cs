using Chrono.Shared.Services;
using Chrono.Tests.Helper;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        _host = BuildTestApp();

        await _host.StartAsync();

        await InitPlaywright();
    }

    private WebApplication BuildTestApp()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        Environment.SetEnvironmentVariable("ASPNETCORE_HOSTINGSTARTUPASSEMBLIES", "Microsoft.AspNetCore.SpaProxy");
        Environment.SetEnvironmentVariable("ConnectionStrings:DefaultConnection",
            $"Data Source={_config["DatabaseFullPath"]}");

        var builder = Program.CreateBuilder([$"urls={_config["BackendUrl"]!}"]);

        // Disables Authentication for E2E tests (= no 2FA etc.)
        builder.Services.AddScoped<ICurrentUserService, FakeCurrentUserService>();
        builder.Services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();

        return Program.BuildApp(builder);
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
