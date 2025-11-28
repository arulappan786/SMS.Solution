using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SMS.Application.Configs;
using SMS.Application.CQRS.Core.Students.Commands.CreateStudent;
using SMS.Application.Services.Implements.Core;
using SMS.Application.Services.Interfaces.Core;
using SMS.Application.Validations;
using System.Reflection;

namespace SMS.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<StudentSettings>(configuration.GetSection(StudentSettings.SettingsKey));

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));                        
            services.AddScoped<IValidator<CreateStudentCommand>, CreateStudentCommandValidator>();
            services.AddScoped<IValidationService, ValidationService>();
            services.AddScoped<IStudentCodeGeneratorService, StudentCodeGeneratorService>();
            services.AddScoped<IStudentOnboardingService, StudentOnboardingService>();
            services.AddScoped<IStudentService, StudentService>();

            return services;
        }
    }
}
