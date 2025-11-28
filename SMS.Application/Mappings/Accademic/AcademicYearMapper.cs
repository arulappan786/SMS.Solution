using AutoMapper;
using SMS.Application.CQRS.Accademic.AcademicYears.Commands;
using SMS.Application.DTOs.Accademic.AcademicYears;
using SMS.Domain.Entities.Academic;

namespace SMS.Application.Mappings.Accademic
{
    public class AcademicYearMapper : Profile
    {
        public AcademicYearMapper()
        {
            // Create
            CreateMap<CreateAcademicYearCommand, AcademicYear>();

            // Read
            CreateMap<AcademicYear, AcademicYearDto>();

        }
    }
}
