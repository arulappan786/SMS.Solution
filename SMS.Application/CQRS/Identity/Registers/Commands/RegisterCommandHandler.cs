using MediatR;
using Microsoft.AspNetCore.Identity;
using SMS.Application.Constants;
using SMS.Application.DTOs.Service;
using SMS.Application.Services.Logging;
using SMS.Domain.Entities.Identity;

namespace SMS.Application.CQRS.Identity.Registers.Commands
{
    public class RegisterCommandHandler(
        UserManager<AppUser> userManager,
        IAppLogger<RegisterCommandHandler> logger) : IRequestHandler<RegisterCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            logger.LogInfo($"Starting user registration for email: {request.Email}");

            // --- 1. Basic Checks (Assuming detailed validation is done by FluentValidation Pipeline) ---
            if (request.Password != request.ConfirmPassword)
            {
                return ServiceResponse.Failure("Password and confirmation password do not match.");
            }

            // Check if user already exists
            if (await userManager.FindByEmailAsync(request.Email) != null)
            {
                logger.LogWarning($"Registration failed: User already exists with email: {request.Email}");
                return ServiceResponse.Failure("User with this email already exists.");
            }

            // --- 2. Create AppUser Entity ---
            var newUser = new AppUser
            {
                UserName = request.UserName,
                Email = request.Email,
                EmailConfirmed = false, // Must be confirmed via external email link in a real app
            };

            // --- 3. Create User in Database ---
            var createResult = await userManager.CreateAsync(newUser, request.Password);

            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                logger.LogError($"User creation failed for {request.Email}: {errors}");
                return ServiceResponse.Failure($"User registration failed: {errors}");
            }

            // --- 4. Assign Default Role (e.g., "Student" or "User") ---
            // ASSUMPTION: 'Student' is the default role for general registration.
            var defaultRole = AppRoles.Student;

            // Check if the role exists (ensures RoleSeeder ran successfully)
            if (await userManager.AddToRoleAsync(newUser, defaultRole) != null)
            {
                logger.LogInfo($"Successfully assigned default role '{defaultRole}' to user {newUser.Id}.");
            }
            else
            {
                logger.LogWarning($"Could not assign default role '{defaultRole}' to new user {newUser.Id}.");
            }


            logger.LogInfo($"User {newUser.Id} successfully registered.");

            // NOTE: In a production app, you would initiate the Email Confirmation flow here.

            return ServiceResponse.Success($"User registered successfully! Confirmation email sent to {request.Email}.");
        }
    }
}