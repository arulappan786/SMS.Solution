using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SMS.Application.Configs;
using SMS.Application.CQRS.Accademic.AcademicYears.Commands.CreateAcademicYear;
using SMS.Application.CQRS.Core.Students.Commands.CreateStudent;
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
            services.AddScoped<IValidator<CreateAcademicYearCommand>, CreateAcademicYearCommandValidator>();
            services.AddScoped<IValidationService, ValidationService>();            

            return services;
        }
    }
}
