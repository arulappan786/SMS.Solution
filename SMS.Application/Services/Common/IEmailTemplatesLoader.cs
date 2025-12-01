using SMS.Application.CQRS.Core.Students.Commands.Create;

namespace SMS.Application.Services.Common
{
    public interface IEmailTemplatesLoader
    {
        string LoadEmailTemplate(CreateStudentCommand request, string newPassword, string templateName);
    }
}
