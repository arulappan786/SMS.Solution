using FluentValidation;
using MediatR.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SMS.Application.CQRS.Core.Students.Commands;
using SMS.Application.Services.Implementations.Core;
using SMS.Application.Services.Interfaces.Core;
using SMS.Application.Validations;
using System.Reflection;

namespace SMS.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<StudentCodeSettings>(cfg => configuration.GetSection("StudentSettings"));

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient<IRequestPreProcessor<CreateStudentCommand>, StudentCodeGeneratorPreProcessor>();

            services.AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));
                        
            services.AddScoped<IValidator<CreateStudentCommand>, CreateStudentCommandValidator>();
            services.AddScoped<IValidationService, ValidationService>();

            services.AddScoped<IStudentCodeGeneratorService, StudentCodeGeneratorService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddSingleton<StudentCodeSettings>();

            return services;
        }
    }
}
