using AutoMapper;
using SMS.Application.CQRS.Core.Students.Commands.Create;
using SMS.Application.DTOs.Core.Students;
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
                            src.CurrentClassId,
                            src.FullName,
                            src.HomeAddress,
                            src.DateOfBirth,
                            src.Gender,
                            src.Email,
                            src.StudentCode!,
                            DateTime.UtcNow));

            // Read           
            CreateMap<Student, StudentDto>()
                .ConstructUsing(src =>
                new StudentDto(
                    src.Id,
                    src.UserId,
                    src.CurrentClassId,
                    src.CurrentClass!.Name,
                    src.FullName,
                    src.HomeAddress,
                    src.DateOfBirth,
                    src.Gender.ToString(),
                    src.Email,
                    src.StudentCode,
                    src.EnrollmentDate
                ));
        }
    }
}