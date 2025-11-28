using AutoMapper;
using SMS.Application.CQRS.Core.Students.Commands;
using SMS.Application.DTOs.Core;
using SMS.Domain.Entities.Core;

namespace SMS.Application.Mappings.Core
{
    public class StudentMapper : Profile
    {
        public StudentMapper()
        {
            // Create
            CreateMap<CreateStudentCommand, Student>()
                .ConstructUsing(src =>
                new Student(Guid.NewGuid(), 
                            src.UserId,
                            Guid.Empty,
                            src.FullName,
                            src.HomeAddress,
                            src.DateOfBirth,
                            src.Gender,
                            src.Email,
                            src.StudentCode!,
                            DateTime.UtcNow));

            // Read
            //CreateMap<Student, StudentDto>();
            // Define the mapping from the source entity (Student) to the destination record (StudentDto)
            CreateMap<Student, StudentDto>()
                // This tells AutoMapper exactly which constructor to call and what arguments to pass
                .ConstructUsing(src =>
                new StudentDto(
                    src.Id,              // Assuming the entity's ID maps to StudentId
                    src.UserId ?? Guid.Empty, // Handle nullable Guid
                    src.CurrentClassId ?? Guid.Empty, // Handle nullable Guid
                    src.FullName,         // Maps the FullName Value Object
                    src.HomeAddress,   // Maps the Address Value Object
                    src.DateOfBirth,
                    src.Gender.ToString(),
                    src.Email,
                    src.StudentCode,
                    src.EnrollmentDate
                ));

            // Update
            CreateMap<UpdateStudentDto, Student>()
                // This tells AutoMapper exactly which constructor to call and what arguments to pass
                .ConstructUsing(src =>
                new Student(
                    src.StudentId,
                    src.UserId,
                    src.CurrentClassId,
                    src.FullName,
                    src.HomeAddress,
                    src.DateOfBirth,
                    src.Gender,
                    src.Email,
                    src.StudentCode,
                    src.EnrollmentDate
                ));
        }
    }
}