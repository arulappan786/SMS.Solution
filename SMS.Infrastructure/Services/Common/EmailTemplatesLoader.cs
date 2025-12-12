using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SMS.Application.CQRS.Core.Students.Commands.Create;
using SMS.Application.Services.Common;
using SMS.Infrastructure.Configs;

namespace SMS.Infrastructure.Services.Common
{
    public class EmailTemplatesLoader(IWebHostEnvironment env, IOptions<ClientSettings> clientSettings) : IEmailTemplatesLoader
    {

        public string LoadEmailTemplate(CreateStudentCommand request, string newPassword, string templateName)
        {
            string templatePath = Path.Combine(env.ContentRootPath, "EmailTemplates", templateName);
            
            if(!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Email template '{templateName}' not found at path: {templatePath}");
            }

            string htmlBody = File.ReadAllText(templatePath);

            htmlBody = htmlBody.Replace("[USERNAME]", request.Email);
            htmlBody = htmlBody.Replace("[PASSWORD]", newPassword);
            htmlBody = htmlBody.Replace("[STUDENT_NAME]", request.FullName.ToString());
            htmlBody = htmlBody.Replace("[LOGIN_URL]", clientSettings.Value.LoginUrl ?? string.Empty);
            htmlBody = htmlBody.Replace("[CURRENT_YEAR]", DateTime.UtcNow.Year.ToString());

            return htmlBody;
        }
    }
}
