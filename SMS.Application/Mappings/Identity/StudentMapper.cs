using AutoMapper;
using SMS.Application.CQRS.Core.Students.Commands;
using SMS.Application.DTOs.Identity;
using SMS.Domain.Entities.Core;

namespace SMS.Application.Mappings.Identity
{
    public class StudentMapper : Profile
    {
        public StudentMapper() 
        {
            // Map: CreateStudentCommand (Source) -> Student (Destination)
            CreateMap<CreateStudentCommand, Student>()
                .ConstructUsing(src =>                    
                    new Student(src.UserId, src.FullName, src.HomeAddress, src.DateOfBirth, src.Gender, src.Email,
                                src.StudentCode, DateTime.UtcNow)
                    );

            // Read
            CreateMap<Student, StudentDto>();

            // Create
            CreateMap<CreateStudentDto, Student>();
            

            // Update
            CreateMap<UpdateStudentDto, Student>();
        }
    }
}
