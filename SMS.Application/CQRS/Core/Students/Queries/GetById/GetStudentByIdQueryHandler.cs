using AutoMapper;
using MediatR;
using SMS.Application.DTOs.Core.Students;
using SMS.Application.DTOs.Service;
using SMS.Application.Exceptions;
using SMS.Domain.Interfaces.Repositories.Core;

namespace SMS.Application.CQRS.Core.Students.Queries.GetById
{
    // The handler returns the concrete DTO type: StudentDto
    public class GetStudentByIdQueryHandler(IStudentRepository studentRepository, IMapper mapper)
        : IRequestHandler<GetStudentByIdQuery, ServiceResponse<StudentDto>>
    {
        public async Task<ServiceResponse<StudentDto>> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
        {
            // 1. Retrieve the entity
            var student = await studentRepository.GetAsync(request.Id, cancellationToken);

            // 2. Handle Not Found by throwing an exception
            if (student == null)
            {
                // Throw the custom application-level exception
                // The API controller should catch this and return HTTP 404 Not Found.
                throw new EntityNotFoundException(nameof(Domain.Entities.Core.Student), request.Id);
            }

            // 3. Map and return the concrete DTO
            var mappedStudent = mapper.Map<StudentDto>(student);

            return ServiceResponse<StudentDto>.Success(data: mappedStudent);
        }
    }
}