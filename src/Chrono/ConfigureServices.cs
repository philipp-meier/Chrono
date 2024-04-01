using System.Reflection;
using Chrono;
using Chrono.Shared.Behaviors;
using Chrono.Shared.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection.Extensions;

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

        // Only add the user service, if there is no other user service (e.g., "FakeCurrentUserService") registered.
        services.TryAddScoped<ICurrentUserService, CurrentUserService>();

        services.AddControllers()
            .PartManager.ApplicationParts.Add(new AssemblyPart(typeof(Program).Assembly));

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
