using AutoMapper;
using SMS.Application.CQRS.Accademic.Classes.Commands.Create;
using SMS.Application.DTOs.Accademic.Classes;
using SMS.Domain.Entities.Academic;

namespace SMS.Application.Mappings.Accademic
{
    public class ClassesMapper : Profile
    {
        public ClassesMapper()
        {
            // Create
            CreateMap<CreateClassesCommand, Classes>();

            // Read
            CreateMap<Classes, ClassesDto>();

        }
    }
}
