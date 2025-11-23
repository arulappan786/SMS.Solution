using AutoMapper;
using MediatR;
using SMS.Application.Services.Interfaces.Common;
using SMS.Application.Services.Interfaces.Context;
using SMS.Application.Services.Interfaces.Identity;
using SMS.Domain.Entities.Core;
using SMS.Domain.Entities.Identity;
using SMS.Domain.Interfaces.Repositories;

namespace SMS.Application.CQRS.Core.Students.Commands
{
    public class CreateStudentCommandHandler(IStudentRepository repository, IAppDbContext context, IMapper mapper,
                                             IUserManagementService userManagementService, IPasswordGeneratorService passwordGenerator)
        : IRequestHandler<CreateStudentCommand, int>
    {
        public async Task<int> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
        {
            // The StudentCode check acts as a fast-fail validation.
            if (await repository.ExistsByStudentCodeAsync(request.StudentCode))
            {
                throw new ArgumentException($"StudentCode '{request.StudentCode}' is already in use.");
            }

            // --- USER ID & STUDENT CREATION ---
            string newAppUserId;

            try
            {

                AppUser appUser = new AppUser()
                {
                    UserName = request.Email,
                    Email = request.Email,
                    DisplayName = request.FullName.ToString(),
                    PasswordHash = passwordGenerator.GenerateSecurePassword(),                    
                };

                // 1. Create the User Identity
                var userCreationResult = await userManagementService.CreateUserAsync(appUser);

                if (!userCreationResult)
                {
                    throw new Exception($"Failed to create user identity for student: {request.Email}");
                }

                var createdUser = await userManagementService.GetUserByEmailAsync(request.Email);

                newAppUserId = createdUser!.Id;
            }
            catch (Exception ex) when (!(ex is ArgumentException))
            {
                // Catch any underlying exceptions from the Identity service itself (DB connection, timeout, etc.)
                // Re-throw or wrap in a custom application exception for better handling by the presentation layer.
                throw new Exception("A system error occurred during user identity creation.", ex);
            }

            // 2. Create the Student Entity and Link to User ID
            try
            {
                // Use AutoMapper with ConstructUsing to create the Student entity,
                // passing the newAppUserId to the constructor.
                var student = mapper.Map<Student>(request, opt =>
                    opt.AfterMap((src, dest) => dest.UserId = newAppUserId)
                );

                // 3. Persistence: Track the new Student entity
                await repository.AddAsync(student);

                // 4. Unit of Work: Commit both the Identity changes (if they weren't committed by UserManager)
                // and the Student entity changes. Since the UserManager commits immediately, this 
                // ensures the Student entity change is committed.
                await context.SaveChangesAsync(cancellationToken);

                return student.Id;
            }
            catch (Exception ex)
            {
                // In a real application, if persistence of the Student fails here, 
                // you should ideally attempt to delete the user created in Step 1 for cleanup.
                // However, that cleanup often requires more complex transactional setup or saga pattern implementation.
                throw new Exception("Failed to persist student profile after user creation.", ex);
            }
        }
    }
}