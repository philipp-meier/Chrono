using System.Reflection;
using Chrono.Application.Common.Behaviors;
using FluentValidation;
using MediatR;

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
}
