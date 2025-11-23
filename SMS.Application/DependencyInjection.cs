using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace SMS.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // 1. Register MediatR
            // Scans the current assembly (SMS.Application) for Handlers (Commands/Queries)
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            // 2. Register AutoMapper
            // Scans the current assembly (SMS.Application) for mapping profiles (classes inheriting from Profile)
            services.AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));
            
            // 3. Register Fluent Validators (if used in MediatR pipeline)
            // Example: services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

            return services;
        }
    }
}
