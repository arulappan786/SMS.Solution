using AutoMapper;
using MediatR;
using SMS.Application.DTOs.Core.Students;
using SMS.Domain.Interfaces.Repositories.Core;

namespace SMS.Application.CQRS.Core.Students.Queries.GetById
{
    public class GetStudentByIdQueryHandler(IStudentRepository studentRepository, IMapper mapper) : IRequestHandler<GetStudentByIdQuery, StudentDto>
    {
        public async Task<StudentDto> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
        {
            var student = await studentRepository.GetAsync(request.Id, cancellationToken);
            var mappedStudent = mapper.Map<StudentDto>(student);
            return mappedStudent;
        }
    }
}
