using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SMS.Application.Configs;
using SMS.Application.CQRS.Accademic.AcademicYears.Commands.Create;
using SMS.Application.CQRS.Accademic.AcademicYears.Commands.Delete;
using SMS.Application.CQRS.Accademic.AcademicYears.Commands.Update;
using SMS.Application.CQRS.Accademic.Classes.Commands.Create;
using SMS.Application.CQRS.Accademic.Classes.Commands.Delete;
using SMS.Application.CQRS.Accademic.Classes.Commands.Update;
using SMS.Application.CQRS.Core.Students.Commands.Create;
using SMS.Application.CQRS.Core.Students.Commands.Delete;
using SMS.Application.CQRS.Core.Students.Commands.Update;
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

            // Validation Services
            services.AddScoped<IValidationService, ValidationService>();

            // Individual Validators
            services.AddScoped<IValidator<CreateStudentCommand>, CreateStudentCommandValidator>();
            services.AddScoped<IValidator<UpdateStudentCommand>, UpdateStudentCommandValidator>();
            services.AddScoped<IValidator<DeleteStudentCommand>, DeleteStudentCommandValidator>();

            services.AddScoped<IValidator<CreateAcademicYearCommand>, CreateAcademicYearCommandValidator>();
            services.AddScoped<IValidator<UpdateAcademicYearCommand>, UpdateAcademicYearCommandValidator>();
            services.AddScoped<IValidator<DeleteAcademicYearCommand>, DeleteAcademicYearCommandValidator>();

            services.AddScoped<IValidator<CreateClassesCommand>, CreateClassesCommandValidator>();
            services.AddScoped<IValidator<UpdateClassesCommand>, UpdateClassesCommandValidator>();
            services.AddScoped<IValidator<DeleteClassesCommand>, DeleteClassesCommandValidator>();


            return services;
        }
    }
}
