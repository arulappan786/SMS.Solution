using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SMS.Application.Services.Common;
using SMS.Application.Services.Core.Students;
using SMS.Application.Services.Identity;
using SMS.Application.Services.Logging;
using SMS.Domain.Entities.Identity;
using SMS.Domain.Interfaces.Repositories.Academic;
using SMS.Domain.Interfaces.Repositories.Common;
using SMS.Domain.Interfaces.Repositories.Core;
using SMS.Infrastructure.Configs;
using SMS.Infrastructure.Persistance.Context;
using SMS.Infrastructure.Repositories.Academic;
using SMS.Infrastructure.Repositories.Core;
using SMS.Infrastructure.Services.Common;
using SMS.Infrastructure.Services.Core.Students;
using SMS.Infrastructure.Services.Identity;
using SMS.Infrastructure.Services.Logging;

namespace SMS.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<GmailSettings>(configuration.GetSection(GmailSettings.SettingsKey));

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    builder => builder.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Identity setup.
            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                // --- Security and Sign-In Policy ---
                options.SignIn.RequireConfirmedEmail = true;
                options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;

                // --- Password Policy ---
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;
            })
                .AddEntityFrameworkStores<AppDbContext>();

            // Common Services
            services.AddScoped(typeof(IAppLogger<>), typeof(SerilogLoggerAdaptor<>));
            services.AddSingleton<IEmailSenderService, EmailSenderService>();

            // Academic Repositories and Services
            services.AddScoped<IAcademicYearRepository, AcademicYearRepository>();
            services.AddScoped<IClassesRepository, ClassesRepository>();
            services.AddScoped<ISubjectRepository, SubjectRepository>();

            // Core Repositories and Services
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<IPasswordGeneratorService, PasswordGeneratorService>();
            services.AddScoped<IStudentCodeGeneratorService, StudentCodeGeneratorService>();
            services.AddScoped<IStudentOnboardingService, StudentOnboardingService>();

            // Identity Services
            services.AddScoped<IUserManagementService, UserManagementService>();
            services.AddScoped<IRoleManagementService, RoleManagementService>();
            services.AddScoped<ITokenManagementService, TokenManagementService>();
            

            return services;
        }
    }
}