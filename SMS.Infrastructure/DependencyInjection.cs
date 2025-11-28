using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SMS.Application.Services.Interfaces.Common;
using SMS.Application.Services.Interfaces.Core.Students;
using SMS.Application.Services.Interfaces.Identity;
using SMS.Application.Services.Interfaces.Logging;
using SMS.Domain.Entities.Identity;
using SMS.Domain.Interfaces.Repositories;
using SMS.Infrastructure.Configs;
using SMS.Infrastructure.Persistance.Context;
using SMS.Infrastructure.Repositories;
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
            
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<IUserManagementService, UserManagementService>();
            services.AddScoped<IRoleManagementService, RoleManagementService>();
            services.AddScoped<ITokenManagementService, TokenManagementService>();
            services.AddScoped<IPasswordGeneratorService, PasswordGeneratorService>();
            services.AddScoped(typeof(IAppLogger<>), typeof(SerilogLoggerAdaptor<>));
            services.AddSingleton<IEmailSenderService, EmailSenderService>();
            services.AddScoped<IStudentCodeGeneratorService, StudentCodeGeneratorService>();
            services.AddScoped<IStudentOnboardingService, StudentOnboardingService>();

            return services;
        }
    }
}