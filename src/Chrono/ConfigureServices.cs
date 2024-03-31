using System.Reflection;
using Chrono.Shared.Behaviors;
using Chrono.Shared.Services;
using Chrono.Shared.Testing;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.HttpOverrides;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ConfigureServices
{
    public static void AddApplicationServices(this IServiceCollection services, bool isDevelopment)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            if (isDevelopment)
            {
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
            }
        });
    }

    public static void AddWebUiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        services.AddHttpContextAccessor();

        if (Environment.GetEnvironmentVariable("CHRONO_E2E_TESTING") == "true")
        {
            services.AddScoped<ICurrentUserService, FakeCurrentUserService>();
            services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
        }
        else
        {
            services.AddScoped<ICurrentUserService, CurrentUserService>();
        }

        services.AddControllersWithViews();
        services.AddSwaggerGen(options => options.CustomSchemaIds(type => type.FullName));

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        services.AddWebUiSecurityServices(configuration);
    }
}
